using System.Collections.Generic;
using System.Linq;
using TXDCL.Astar;
using TXDCL.Character;
using TXDCL.Combat;
using TXDCL.XiuLian.FuShu;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NPCController : CharacterBase
{
    private static readonly int CastFaShu = Animator.StringToHash("CastFaShu");
    private List<FaShuData> currentFaShuInTurn = new();
    private CharacterBase currentEnemy;
    private Vector2Int currentPosition;
    private Vector2Int targetPosition;
    private Stack<MovementStep> movementSteps = new();

    private void OnEnable()
    {
        EventHandler.BeforeCombatBeginEvent += OnBeforeCombatBeginEvent;
        EventHandler.AfterCombatBeginEvent += OnAfterCombatBeginEvent;
        EventHandler.CharacterTurnBeginEvent += OnCharacterTurnBeginEvent;
    }

    private void OnDisable()
    {
        EventHandler.BeforeCombatBeginEvent -= OnBeforeCombatBeginEvent;
        EventHandler.AfterCombatBeginEvent -= OnAfterCombatBeginEvent;
        EventHandler.CharacterTurnBeginEvent -= OnCharacterTurnBeginEvent;
    }

    protected override void OnBeforeCombatBeginEvent()
    {
        base.OnBeforeCombatBeginEvent();
        CombatManager.Instance.RegisterEnemySide(this);
    }
    private void OnAfterCombatBeginEvent()
    {
        Allies.AddRange(CombatManager.Instance.EnemySides);
        Enemies.AddRange(CombatManager.Instance.PlayerSides);
    }

    protected override void OnCharacterTurnBeginEvent(CharacterBase character)
    {
        base.OnCharacterTurnBeginEvent(character);
        if(character!= this) return;
        SelectFaShu();
    }

    private void SelectFaShu()
    {
        currentFaShuInTurn.Clear();
        if (Enemies.Count == 0) return;
        currentEnemy = Enemies[Random.Range(0, Enemies.Count)];
        //选择法术
        SelectPotentialFaShu();
        // if(currentFaShuInTurn.Count == 0) return;
        // Debug.Log(currentFaShuInTurn.Count);
    }

    private void SelectPotentialFaShu()
    {
        var maxLoopCount = 0;
        while (maxLoopCount < 999)
        {
            maxLoopCount++;
            //获得随机序号
            var FaShuIndex = Random.Range(0, currentFaShuList.Count);
            //寻找法术并验证是否可以释放
            if (!FaShuManager.Instance.CheckReleaseFaShuConditions(CharacterData, currentFaShuList[FaShuIndex],
                    false) || !CheckEnoughDistance(currentFaShuList[FaShuIndex].ReleaseRange)) continue;
            //释放法术
            ReleaseFaShu(currentFaShuList[FaShuIndex]);
            break;
        }
    }

    private void ReleaseFaShu(FaShuData faShuData)
    {
        //执行移动
        //执行法术
        SetCharacterFacingDirection(currentEnemy.transform.position.x - transform.position.x);
        animator.SetTrigger(CastFaShu);
        FaShuManager.Instance.ReleaseFaShu(faShuData, currentEnemy.transform.position,this,
            CombatGridManager.Instance.GetAllGridInCombatDict(CombatGridManager.Instance.FindPotentialPath(
                CombatGridManager.Instance.CharacterPositionsInCombatDict[currentEnemy], faShuData.Range, true)));
    }

    private void CharacterMove(int FaShuReleaseRange)
    {
        AStar.Instance.BuildPath(SceneManager.GetActiveScene().name, currentPosition, targetPosition, movementSteps);
        var moveDis = FaShuReleaseRange;
        while (moveDis > 0)
        {
            var step = movementSteps.Pop();
            moveDis--;
        }
    }
    
    /// <summary>
    /// 检测当前角色企图攻击的目标是否在自己最大可移动范围加上法术范围内
    /// </summary>
    /// <param name="enemy">攻击目标</param>
    /// <param name="FaShuReleaseRange">法术释放范围</param>
    /// <returns></returns>
    private bool CheckEnoughDistance(int FaShuReleaseRange)
    {
        var maxDistance = (FaShuReleaseRange + CharacterData.currentMovement) * 10;
        currentPosition = CombatGridManager.Instance.CharacterPositionsInCombatDict[this];
        targetPosition = CombatGridManager.Instance.CharacterPositionsInCombatDict[currentEnemy];
        var TargetDistance = AStar.Instance.GetDistance(new AStarNode(currentPosition), new AStarNode(targetPosition));
        // Debug.Log("TargetDistance: " + TargetDistance);
        // Debug.Log("MaxDistance:" + maxDistance);
        return TargetDistance <= maxDistance;
    }
}
