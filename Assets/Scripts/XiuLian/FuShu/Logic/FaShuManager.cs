using System;
using System.Collections.Generic;
using System.Linq;
using TXDCL.Character;
using TXDCL.Combat;
using UnityEngine;

namespace TXDCL.XiuLian.FuShu
{
    public class FaShuManager : Singleton<FaShuManager>
    {
        [SerializeField]private FaShuDataList FaShuDataList;
        public  Dictionary<int, FaShuData> FaShuDataDict = new();

        protected override void Awake()
        {
            base.Awake();
            InitializeData();
        }

        private void InitializeData()
        {
            if (FaShuDataList == null) return;
            foreach (var FaShu in FaShuDataList.FaShuDatas)
            {
                FaShuDataDict.Add(FaShu.ID, FaShu);
            }
        }

        public FaShuData GetFaShuData(int id)
        {
            return FaShuDataDict.GetValueOrDefault(id);
        }

        /// <summary>
        /// 根据法术信息执行法术动画
        /// </summary>
        /// <param name="FaShuData"></param>
        /// <param name="from"></param>
        /// <param name="targetCharacters"></param>
        public void ReleaseFaShu(FaShuData FaShuData,Vector3 targetPosition,CharacterBase from, List<CharacterBase> targetCharacters)
        {
            PoolTool.Instance.GetFaShuDerivativeFromPool(FaShuData, targetPosition, from, targetCharacters);
        }
        /// <summary>
        /// 根据法术信息结算法术
        /// </summary>
        /// <param name="FaShu">法术信息</param>
        /// <param name="from">施法者</param>
        /// <param name="targetCharacters">目标人群</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void ExecuteFaShu(FaShuData FaShu, CharacterBase from, List<CharacterBase> targetCharacters)
        {
            if (targetCharacters.Count <= 0) return;
            switch (FaShu.FaShuTarget)
            {
                case FaShuTarget.Self:
                    foreach (var effect in FaShu.BasicEffectDatas)
                    {
                        effect.OnEffectCreate(from, from);
                    }
                    break;
                case FaShuTarget.Enemy:
                    foreach (var character in targetCharacters.Where(character => from.Enemies.Contains(character)))
                    {
                        foreach (var effect in FaShu.BasicEffectDatas)
                        {
                            effect.OnEffectCreate(from, character);
                        }
                    }
                    break;
                case FaShuTarget.Ally:
                    foreach (var character in targetCharacters.Where(character => from.Allies.Contains(character)))
                    {
                        foreach (var effect in FaShu.BasicEffectDatas)
                        {
                            effect.OnEffectCreate(from, character);
                        }
                    }
                    break;
                case FaShuTarget.Any:
                    foreach (var character in targetCharacters)
                    {
                        foreach (var effect in FaShu.BasicEffectDatas)
                        {
                            effect.OnEffectCreate(from, character);
                        }
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// 检测当前角色属性及道藏是否满足释放目标法术
        /// </summary>
        /// <param name="characterData">当前角色属性及道藏信息</param>
        /// <param name="faShuData">目标法术信息</param>
        /// <returns></returns>
        public bool CheckReleaseFaShuConditions(CharacterData characterData, FaShuData faShuData, bool isPlayer)
        {
            //检测法术冷却和准备时间
            if (faShuData.CurrentCoolDownTime > 0 || faShuData.currentPrepareTurns < faShuData.MaxPrepareTurns)
                return false;
            //检测法术基础消耗
            var baseConditions = faShuData.HealthCost < characterData.currentHealth &&
                                 faShuData.StaminaCost <= characterData.currentStamina &&
                                 faShuData.ManaCost <= characterData.currentMana &&
                                 faShuData.JingShenLiCost <= characterData.JingShenLi;
            //检测法术道藏消耗
            var DaoCangConditions = CheckReleaseFaShuDaoCangCosts(characterData, faShuData,isPlayer);
            return baseConditions && DaoCangConditions;
        }

        private bool CheckReleaseFaShuDaoCangCosts(CharacterData characterData, FaShuData faShuData, bool isPlayer)
        {
            //检测基础道藏
            var selectDaoCangs = new List<int>
            {
                characterData.currentMetalDaocang,
                characterData.currentWoodDaocang,
                characterData.currentWaterDaocang,
                characterData.currentFireDaocang,
                characterData.currentEarthDaocang
            };
            foreach (var DaoCang in faShuData.DaoCangCosts)
            {
                var enoughDaoCang = false;
                switch (DaoCang.Wuxing.currentWuXing)
                {
                    case WuXing.锐金:
                        if (characterData.currentMetalDaocang >= DaoCang.DaoCang)
                        {
                            enoughDaoCang = true;
                            selectDaoCangs[0] -= DaoCang.DaoCang;
                        }

                        break;
                    case WuXing.灵木:
                        if (characterData.currentWoodDaocang >= DaoCang.DaoCang)
                        {
                            enoughDaoCang = true;
                            selectDaoCangs[1] -= DaoCang.DaoCang;
                        }

                        break;
                    case WuXing.弱水:
                        if (characterData.currentWaterDaocang >= DaoCang.DaoCang)
                        {
                            enoughDaoCang = true;
                            selectDaoCangs[2] -= DaoCang.DaoCang;
                        }

                        break;
                    case WuXing.离火:
                        if (characterData.currentFireDaocang >= DaoCang.DaoCang)
                        {
                            enoughDaoCang = true;
                            selectDaoCangs[3] -= DaoCang.DaoCang;
                        }

                        break;
                    case WuXing.厚土:
                        if (characterData.currentEarthDaocang >= DaoCang.DaoCang)
                        {
                            enoughDaoCang = true;
                            selectDaoCangs[4] -= DaoCang.DaoCang;
                        }

                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (!enoughDaoCang) return false;
            }

            //判断是否是玩家，非玩家的角色不需要检测相同道藏
            if (!isPlayer) return true;

            var enoughSameDaoCang = false;
            //检测相同道藏
            if (faShuData.SameCost > 0)
            {
                foreach (var DaoCang in selectDaoCangs.Where(DaoCang => DaoCang >= faShuData.SameCost))
                {
                    enoughSameDaoCang = true;
                }
            }
            else
            {
                enoughSameDaoCang = true;
            }

            return enoughSameDaoCang;
        }
        public void UpdateFaShuCost(CharacterBase character, FaShuData faShuData)
        {
            character.CharacterData.currentHealth -= faShuData.HealthCost;
            character.CharacterData.currentStamina -= faShuData.StaminaCost;
            character.CharacterData.currentMana -= faShuData.ManaCost;
            character.CharacterData.JingShenLi -= faShuData.JingShenLiCost;
            faShuData.CurrentCoolDownTime = faShuData.MaxCoolDownTime;
            if (character == GameManager.Instance.Player && character.CompareTag("Player"))
            {
                DaoCangPanelUI.Instance.UpdateDaoCangCost();
                DaoCangPanelUI.Instance.ResetDaoCangPanelUI();
            }
            else
            {
                foreach (var DaoCang in faShuData.DaoCangCosts)
                {
                    switch (DaoCang.Wuxing.currentWuXing)
                    {
                        case WuXing.锐金:
                            character.CharacterData.currentMetalDaocang -= DaoCang.DaoCang;
                            break;
                        case WuXing.灵木:
                            character.CharacterData.currentWoodDaocang -=  DaoCang.DaoCang;
                            break;
                        case WuXing.弱水:
                            character.CharacterData.currentWaterDaocang -= DaoCang.DaoCang;
                            break;
                        case WuXing.离火:
                            character.CharacterData.currentFireDaocang -=  DaoCang.DaoCang;
                            break;
                        case WuXing.厚土:
                            character.CharacterData.currentEarthDaocang -=  DaoCang.DaoCang;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }
    }
}
