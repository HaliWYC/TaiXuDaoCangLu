using System;
using System.Collections.Generic;
using DG.Tweening;
using TXDCL.Character;
using UnityEngine;

namespace TXDCL.XiuLian.FuShu
{
    public class FaShuDerivative : MonoBehaviour
    {
        private FaShuData FaShuData;
        private static readonly int Arrived = Animator.StringToHash("Arrived");
        private Animator animator;
        private CharacterBase from;
        private List<CharacterBase> targets;
        public FaShuDerivativeTrackType TrackType;
        public bool isFacingLeft;//衍生物图片攻击朝向是否向左
        public Vector3 specificPositionModifier;// 在特定位置的位置修正，如在目标位置基础上加上修正值为特效起始位置
        public Vector3 startPositionModifier;// 衍生物起始位置的修正，如在起始位置基础上加上修正值为特效起始位置
        public Vector3 targetPositionModifier;// 衍生物到达目标位置的修正，如在目标位置基础上加上修正值为特效终点
        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        public void Setup(FaShuData faShuData,Vector3 targetPos,CharacterBase fromCharacter, List<CharacterBase> targetCharacters)
        {
            var derivative = faShuData.FaShuDerivative;
            animator.runtimeAnimatorController = derivative.GetComponent<Animator>().runtimeAnimatorController;
            FaShuData = faShuData;
            from = fromCharacter;
            targets = targetCharacters;
            transform.localScale = Vector3.one;
            TrackType = derivative.TrackType;
            isFacingLeft = derivative.isFacingLeft;
            specificPositionModifier = derivative.specificPositionModifier;
            startPositionModifier = derivative.startPositionModifier;
            targetPositionModifier = derivative.targetPositionModifier;
            //设置衍生物初始坐标以及方向
            var startPos = from.transform.position;
            var angle = startPos != targetPos
                ? Mathf.Atan(Mathf.Abs(startPos.y - targetPos.y) / Mathf.Abs(startPos.x - targetPos.x)) * Mathf.Rad2Deg
                : 0;
            if (isFacingLeft && startPos.x < targetPos.x || !isFacingLeft && startPos.x > targetPos.x)
            {
                transform.localScale = new Vector3(-1, 1, 1);
                transform.DORotateQuaternion(startPos.y < targetPos.y ? Quaternion.Euler(0, 0, angle) : Quaternion.Euler(0, 0, -angle), 0.1f);
            }
            else
            {
                transform.DORotateQuaternion(startPos.y < targetPos.y ? Quaternion.Euler(0, 0, -angle) : Quaternion.Euler(0, 0, angle), 0.1f);
            }
            switch (TrackType)
            {
                case FaShuDerivativeTrackType.ReleaserMobile:
                    transform.position = startPos + startPositionModifier;
                    transform.DOMove(targetPos + targetPositionModifier, 0.5f, false).SetEase(Ease.Linear).onComplete = () =>
                    {
                        animator.SetTrigger(Arrived);
                    };
                    break;
                case FaShuDerivativeTrackType.ReleaserFixed:
                    transform.position = startPos + startPositionModifier;
                    animator.Play("Idle");
                    break;
                case FaShuDerivativeTrackType.SpecificMobile:
                    transform.position = targetPos + specificPositionModifier;
                    transform.DOMove(targetPos + targetPositionModifier, 0.5f, false).SetEase(Ease.Linear).onComplete = () =>
                    {
                        animator.SetTrigger(Arrived);
                    };
                    break;
                case FaShuDerivativeTrackType.SpecificFixed:
                    transform.position = targetPos + specificPositionModifier;
                    animator.Play("Idle");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            //结算法术消耗,不管是否命中目标都消耗道藏、法力等资源
            FaShuManager.Instance.UpdateFaShuCost(from, faShuData);
            StartCoroutine(CharacterStatsPanel.Instance.UpdateCharacterStats(GameManager.Instance.Player));
            CombatUI.Instance.FaShuPanelUI.SetUpFaShuSlots(from);
        }

        private void ExecuteFaShuEffects()
        {
            FaShuManager.Instance.ExecuteFaShu(FaShuData, from, targets);
        }

        private void Finish()
        {
            PoolTool.Instance.FaShuDerivativePool.Release(gameObject);
            EventHandler.CallAfterFaShuReleasedEvent(FaShuData);
        }
        
    }
}
