using System;
using TXDCL.XiuLian.FuShu;
using TXDCL.XiuLian.GongFa;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TXDCL.Character
{
    [RequireComponent(typeof(GongFaProcessor))]
    [RequireComponent(typeof(FaShuProcessor))]
    [RequireComponent(typeof(ShenTongProcessor))]
    public class CharacterBase : MonoBehaviour
    {
        public CharacterData templateData;
        public CharacterData CharacterData;

        private string JingjieKey => CharacterData != null
            ? CharacterData.Jingjie.miniJingjieLevel.ToString() + CharacterData.Jingjie.JingjieLevel
            : null;
        //TODO：增加装备属性以及功法属性
        
        [Header("Bools")]
        public bool isShenShiHuanSan; //是否神识涣散
        public bool isCombating;//是否处于战斗状态
        
        protected virtual void Awake()
        {
            if (templateData != null)
            {
                CharacterData = Instantiate(templateData);
            }
            GetComponent<GongFaProcessor>().characterData = CharacterData;
            ResetValue();
        }

        private void Start()
        {
            UpdateLevel();
        }

        #region Combat

        public void TakeDamage(CharacterData attacker, CharacterData defender, FaShuData faShu)
        {
            var isDodge = false;
            //TODO:检测必定闪避的影响事件
            if (CheckDodge(attacker.Jingjie.JingjieLevel, defender.Jingjie.JingjieLevel, isDodge)) return;
            switch (faShu.FaShuType)
            {
                case FaShuType.Normal:
                    defender.currentHealth = math.max(0,
                        defender.currentHealth - faShu.NormalValue - (int)(faShu.AdditionalValue * attacker.Attack));
                    break;
                case FaShuType.ShenShi:
                    defender.ShenShi = math.max(0,
                        defender.ShenShi - faShu.NormalValue - (int)(faShu.AdditionalValue * attacker.ShenShi));
                    break;
                case FaShuType.ShenTong:
                    break;
                case FaShuType.MiShu:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// 判断攻击是否闪避
        /// </summary>
        /// <param name="attacker"></param>
        /// <param name="defender"></param>
        /// <returns></returns>
        private bool CheckDodge(JingjieLevel attacker, JingjieLevel defender, bool isDodge)
        {
            if (isDodge)
            {
                return true;
            }
            var dif = attacker - defender;//计算境界差
            var dodgeRate = Random.Range(-0.05f + dif, 0.05f + dif) * 0.5f;//修正值
            return Random.Range(0f, 1f) > dodgeRate;
        }

        #endregion

        

        #region XiuLian

        public void UpdateLevel()
        {
            CharacterData.Jingjie = CharacterManager.Instance.GetJingjie(JingjieKey);
            var data = CharacterData.Jingjie.JingjieData;
            CharacterData.nextExp = data.NextEXP;
            CharacterData.maxAge = data.MaxAge;
            CharacterData.maxHealth = data.MaxHealth;
            CharacterData.maxMana = data.MaxMana;
            CharacterData.Attack = data.Attack;
            CharacterData.Reaction = data.Reaction;
            CharacterData.maxMovementPerTurn = data.MaxMovementPerTurn;
            CharacterData.ShenShiStrength = data.ShenShiStrength;
            CharacterData.maxDaocangPerTurn = data.MaxDaocangPerTurn;
        }

        public void CheckUpGrade()
        {
            while (CharacterData.currentExp >= CharacterData.nextExp)
            {
                CharacterData.currentExp -= CharacterData.nextExp;
                if (CharacterData.Jingjie.miniJingjieLevel + 1 > MiniJingjieLevel.大圆满)
                {
                    CharacterData.Jingjie.miniJingjieLevel = 0;
                    CharacterData.Jingjie.JingjieLevel++;
                }
                else
                {
                    CharacterData.Jingjie.miniJingjieLevel += 1;
                }

                CharacterData.Jingjie = CharacterManager.Instance.GetJingjie(JingjieKey);
            }

            UpdateLevel();
        }

        public void ResetValue()
        {
            CharacterData.currentHealth = CharacterData.maxHealth;
            CharacterData.currentMana = CharacterData.maxMana;
            CharacterData.ShenShi = CharacterData.ShenShiStrength;
        }

        #endregion
    }
    
}

    
    // private int CalculateBasicDamage(CharacterData attacker, CharacterData defender)
    // {
    //     var normalAttack = Random.Range(attacker.minAttack, attacker.maxAttack);
    //     bool isCritical = false, isFatal = false;
    //     WuxingMultiAttack wuxingAttack;
    //     //1.判定进攻方是否碾压
    //     if (attacker.perfectAccuracyRate > Random.Range(0f, 1f))
    //     {
    //         Debug.Log("Crush");
    //         //碾压，无视防御，必定命中，必定暴击或重创
    //         var attackType = AttackTableTheory(attacker.CriticalRate, attacker.FatalRate);
    //         if (attackType == AttackType.Critical)
    //         {
    //             isCritical = true;
    //         }
    //         else if(attackType == AttackType.Fatal)
    //         {
    //             isFatal = true;
    //         }
    //         wuxingAttack = SelectWuxingAttack(attacker, isCritical, isFatal, false);
    //     }
    //     else
    //     {
    //         //未碾压
    //         //2.判定防守方是否闪避
    //         if (defender.DodgeRate > Random.Range(0f, 1f)) return 0;
    //         Debug.Log("Not Dodge");
    //         //未闪避
    //         //3.判定进攻方是否穿透
    //         //4.计算是否精准
    //         bool isAbrasion = false, isHuixin = false, isHuiyi =false;
    //         var isAccurate = attacker.AccurateRate > Random.Range(0f, 1f);
    //         if (isAccurate)
    //         {
    //             var attackType = AttackTableTheory(attacker.CriticalRate, attacker.FatalRate);
    //             if (attackType == AttackType.Critical)
    //             {
    //                 isCritical = true;
    //             }
    //             else if(attackType == AttackType.Fatal)
    //             {
    //                  isFatal = true;
    //             }
    //         }
    //         else
    //         {
    //             Debug.Log("Not Accurate");
    //             isAbrasion = 1 - attacker.AccurateRate < Random.Range(0f, Settings.Abrasion);
    //             isFatal = AttackTableTheory(0, attacker.FatalRate) == AttackType.Fatal;
    //         }
    //         wuxingAttack = SelectWuxingAttack(attacker, isCritical, isFatal, isAbrasion);
    //         Debug.Log(wuxingAttack.Value);
    //         if (!(attacker.PenetrateRate < Random.Range(0f, 1f)))
    //             //穿透，仅计算攻击
    //             return (int)((normalAttack + wuxingAttack.Value) * CharacterManager.Instance.JingjieCalculation(attacker.Jingjie));
    //         Debug.Log("Not Penetrate");
    //         //未穿透，计算攻击和防御
    //         //4.计算是否坚守
    //         var isTenacious = attacker.TenaciousRate > Random.Range(0f, 1f);
    //         var normalDefense= Random.Range(defender.minDefence, defender.maxDefence);
    //         if (isTenacious)
    //         {
    //             var defenseType = DefenseTableTheory(defender.HuiXinRate, defender.HuiYiRate);
    //             if (defenseType == DefenseType.HuiXin)
    //             {
    //                 isHuixin = true;
    //             }
    //             else if(defenseType == DefenseType.HuiYi)
    //             {
    //                 isHuiyi = true;
    //             }
    //         }
    //         else
    //         {
    //             Debug.Log("Not Tenacious");
    //             isHuiyi = DefenseTableTheory(0, defender.HuiYiRate) == DefenseType.HuiYi;
    //         }
    //         var wuxingDefense = SelectWuxingDefense(defender, isHuixin, isHuiyi);
    //         WuxingDamage(wuxingAttack,wuxingDefense);
    //         if (wuxingDefense == null)
    //             return (int)((normalAttack * ((normalAttack - normalDefense) / (normalDefense + Settings.Defense)) +
    //                           wuxingAttack.Value) * CharacterManager.Instance.JingjieCalculation(attacker.Jingjie));
    //         Debug.Log(wuxingDefense.Value);
    //         return (int)(normalAttack * ((normalAttack - normalDefense) / (normalDefense + Settings.Defense)) *
    //             CharacterManager.Instance.JingjieCalculation(attacker.Jingjie) + wuxingAttack.Value *
    //             ((wuxingAttack.Value - wuxingDefense.Value) / (wuxingDefense.Value + Settings.WuxingDefense)) *
    //             CharacterManager.Instance.JingjieCalculation(defender.Jingjie));
    //     }
    //     return (int)((normalAttack + wuxingAttack.Value) * CharacterManager.Instance.JingjieCalculation(attacker.Jingjie));
    // }
    
    ///// <summary>
    ///// 通过计算五行混合攻击和五行混合防御进行结算
    ///// </summary>
    ///// <param name="wuxingMultiAttack">五行混合攻击</param>
    ///// <param name="wuxingMultiDefense">五行混合防御</param>
    // private void WuxingDamage(WuxingMultiAttack wuxingMultiAttack, WuxingMultiDefense wuxingMultiDefense)
    // {
    //     if (wuxingMultiDefense == null)
    //     {
    //         wuxingMultiAttack.Value = (int)(wuxingMultiAttack.Value * Settings.WuxingCounterWuxing);
    //         return;
    //     }
    //     switch (wuxingMultiAttack.wuxings.Count)
    //     {
    //         case 0:
    //             wuxingMultiDefense.Value = (int)(wuxingMultiDefense.Value * Settings.WuxingCounteredWuxing);
    //             break;
    //         case 1:
    //             switch (wuxingMultiDefense.wuxings.Count)
    //             {
    //                 case 0:
    //                     wuxingMultiAttack.Value = (int)(wuxingMultiAttack.Value * Settings.WuxingCounterWuxing);
    //                     break;
    //                 case 1:
    //                     if(wuxingMultiAttack.wuxings[0].counterWuXing == wuxingMultiDefense.wuxings[0].currentWuXing)
    //                         wuxingMultiAttack.Value = (int)(wuxingMultiAttack.Value * Settings.WuxingCounterWuxing);
    //                     break;
    //                 case 2:
    //                     if (wuxingMultiAttack.wuxings[0].counterWuXing == wuxingMultiDefense.wuxings[0].currentWuXing &&
    //                         wuxingMultiAttack.wuxings[0].currentWuXing != wuxingMultiDefense.wuxings[1].counterWuXing ||
    //                         wuxingMultiAttack.wuxings[0].counterWuXing == wuxingMultiDefense.wuxings[1].currentWuXing &&
    //                         wuxingMultiAttack.wuxings[0].currentWuXing != wuxingMultiDefense.wuxings[0].counterWuXing)
    //                         wuxingMultiAttack.Value = (int)(wuxingMultiAttack.Value * Settings.WuxingCounterWuxing);
    //                     break;
    //             }
    //             break;
    //         case 2:
    //             switch (wuxingMultiDefense.wuxings.Count)
    //             {
    //                 case 0:
    //                     wuxingMultiAttack.Value = (int)(wuxingMultiAttack.Value * Settings.WuxingCounterWuxing);
    //                     break;
    //                 case 1:
    //                     if (wuxingMultiAttack.wuxings[0].counterWuXing == wuxingMultiDefense.wuxings[0].currentWuXing &&
    //                         wuxingMultiAttack.wuxings[1].currentWuXing != wuxingMultiDefense.wuxings[0].counterWuXing ||
    //                         wuxingMultiAttack.wuxings[0].counterWuXing == wuxingMultiDefense.wuxings[0].currentWuXing &&
    //                         wuxingMultiAttack.wuxings[1].currentWuXing != wuxingMultiDefense.wuxings[0].counterWuXing)
    //                         wuxingMultiAttack.Value = (int)(wuxingMultiAttack.Value * Settings.WuxingCounterWuxing);
    //                     break;
    //             }
    //             break;
    //     }
    // }
    
    // /// <summary>
    // /// 计算五行攻击
    // /// </summary>
    // /// <param name="attacker">攻击者</param>
    // /// <param name="isCritical">是否暴击</param>
    // /// <param name="isFatal">是否重创</param>
    // /// <param name="isAbrasion">是否擦伤</param>
    // /// <returns>返回五行混合攻击</returns>
    // private WuxingMultiAttack SelectWuxingAttack(CharacterData attacker, bool isCritical, bool isFatal, bool isAbrasion)
    // {
    //     metalAttack.Value = Random.Range(attacker.Metal.minAttack, attacker.Metal.maxAttack);
    //     woodAttack.Value = Random.Range(attacker.Wood.minAttack, attacker.Wood.maxAttack);
    //     waterAttack.Value = Random.Range(attacker.Water.minAttack, attacker.Water.maxAttack);
    //     fireAttack.Value = Random.Range(attacker.Fire.minAttack, attacker.Fire.maxAttack);
    //     earthAttack.Value = Random.Range(attacker.Earth.minAttack, attacker.Earth.maxAttack);
    //     List<WuxingAttack> wuxingAttacks = new() { metalAttack, woodAttack, waterAttack, fireAttack, earthAttack };
    //     var minValue = Mathf.Min(metalAttack.Value, woodAttack.Value, waterAttack.Value, fireAttack.Value,
    //         earthAttack.Value);
    //     var maxValue = Mathf.Max(metalAttack.Value, woodAttack.Value, waterAttack.Value, fireAttack.Value,
    //         earthAttack.Value);
    //     wuxingAttacks = wuxingAttacks.FindAll(w=>w.Value == maxValue);
    //     List<Wuxing> wuxings = new();
    //     foreach (var wuxing in wuxingAttacks)
    //     {
    //         wuxings.Add(wuxing);
    //     }
    //
    //     if (isAbrasion)
    //     {
    //         return new WuxingMultiAttack{wuxings = wuxings, Value = minValue};
    //     }
    //     else if (isCritical)
    //     {
    //         maxValue = (int)(maxValue * (1 + attacker.criticalMultiplier));
    //     }
    //     else if (isFatal)
    //     {
    //         maxValue = (int)(maxValue * (1 + attacker.fatalMultiplier));
    //     }
    //     return new WuxingMultiAttack{wuxings = wuxings, Value = maxValue};
    // }
    //
    // /// <summary>
    // /// 计算五行防御
    // /// </summary>
    // /// <param name="defender">防守方</param>
    // /// <param name="isHuiXin">是否会心</param>
    // /// <param name="isHuiYi">是否会意</param>
    // /// <returns>返回五行混合防御</returns>
    // private WuxingMultiDefense SelectWuxingDefense(CharacterData defender, bool isHuiXin, bool isHuiYi)
    // {
    //     metalDefense.Value = Random.Range(defender.Metal.minDefence, defender.Metal.maxDefence);
    //     woodDefense.Value = Random.Range(defender.Wood.minDefence, defender.Wood.maxDefence);
    //     waterDefense.Value = Random.Range(defender.Water.minDefence, defender.Water.maxDefence);
    //     fireDefense.Value = Random.Range(defender.Fire.minDefence, defender.Fire.maxDefence);
    //     earthDefense.Value = Random.Range(defender.Earth.minDefence, defender.Earth.maxDefence);
    //     List<WuxingDefense> wuxingDefenses = new() { metalDefense, woodDefense, waterDefense, fireDefense, earthDefense };
    //     var maxValue = Mathf.Max(metalDefense.Value, woodDefense.Value, waterDefense.Value, fireDefense.Value,
    //         earthDefense.Value);
    //     wuxingDefenses = wuxingDefenses.FindAll(w=>w.Value == maxValue);
    //     List<Wuxing> wuxings = new();
    //     foreach (var wuxing in wuxingDefenses)
    //     {
    //         wuxings.Add(wuxing);
    //     }
    //     if (isHuiXin)
    //     {
    //         maxValue = (int)(maxValue * (1 + defender.huiXinMultiplier));
    //     }
    //     else if (isHuiYi)
    //     {
    //         maxValue = (int)(maxValue * (1 + defender.huiYiMultiplier));
    //     }
    //     return new WuxingMultiDefense(){wuxings = wuxings, Value = maxValue};
    // }
    // /// <summary>
    // /// 攻击圆桌理论
    // /// </summary>
    // /// <param name="first">第一个数值</param>
    // /// <param name="second">第二个数值</param>
    // /// <returns>返回攻击类型：普通，暴击重创</returns>
    // private AttackType AttackTableTheory(float first, float second)
    // {
    //     if (first + second > 1)
    //     {
    //         var common = first / second;
    //         second = 1 / (1 + common);
    //         first = 1 - second;
    //         return Random.Range(0f, 1f) < first ? AttackType.Critical : AttackType.Fatal;
    //     }
    //
    //     var value = Random.Range(0, 1);
    //     if (value - first <= 0)
    //         return AttackType.Critical;
    //     if (value - first - second <= 0)
    //         return AttackType.Fatal;
    //     return AttackType.Normal;
    // }
    // /// <summary>
    // /// 防守圆桌理论
    // /// </summary>
    // /// <param name="first">第一个值</param>
    // /// <param name="second">第二个值</param>
    // /// <returns>返回防守类型：普通，会心，会意</returns>
    // private DefenseType DefenseTableTheory(float first, float second)
    // {
    //     if (first + second > 1)
    //     {
    //         var common = first / second;
    //         second = 1 / (1 + common);
    //         first = 1 - second;
    //         return Random.Range(0f, 1f) < first ? DefenseType.HuiXin : DefenseType.HuiYi;
    //     }
    //
    //     var value = Random.Range(0, 1);
    //     if (value - first <= 0)
    //         return DefenseType.HuiXin;
    //     if (value - first - second <= 0)
    //         return DefenseType.HuiYi;
    //     return DefenseType.Normal;
    // }
