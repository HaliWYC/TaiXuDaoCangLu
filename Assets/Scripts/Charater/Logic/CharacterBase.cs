using System;
using System.Collections.Generic;
using TXDCL.Combat;
using TXDCL.XiuLian.FuShu;
using TXDCL.XiuLian.GongFa;
using TXDCL.Effect;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TXDCL.Character
{
    [RequireComponent(typeof(GongFaProcessor))]
    [RequireComponent(typeof(CombatMovement))]
    [RequireComponent(typeof(Animator))]
    public class CharacterBase : MonoBehaviour
    {
        public CharacterData templateData;
        public CharacterData CharacterData;
        public Animator animator;
        public List<CharacterBase> Enemies = new();
        public List<EffectData> Effects = new();
        
        public bool isMoving;
        
        private float faceDirection = 0;
        private int previousYear = 1;
        private string JingjieKey => CharacterData != null
            ? CharacterData.Jingjie.miniJingjieLevel.ToString() + CharacterData.Jingjie.JingjieLevel
            : null;
        //TODO：增加装备属性以及功法属性
        
        [Header("Bools")]
        public bool isShenShiHuanSan = false; //是否神识涣散,神识涣散状态将减少40%命中率
        //public bool isCombating;//是否处于战斗状态
        public bool isJingjieFirmed = true;//境界是否稳固,未稳固将减少40%命中率
        
        protected virtual void Awake()
        {
            if (templateData != null)
            {
                CharacterData = Instantiate(templateData);
            }
            GetComponent<GongFaProcessor>().characterData = CharacterData;
            ResetValue();
        }

        private void OnEnable()
        {
            EventHandler.GameDateEvent += OnGameDateEvent;
            EventHandler.CharacterTurnBeginEvent += OnCharacterTurnBegin;
        }
        

        private void OnDisable()
        {
            EventHandler.GameDateEvent -= OnGameDateEvent;
            EventHandler.CharacterTurnBeginEvent -= OnCharacterTurnBegin;
        }
        private void OnGameDateEvent(int day, int month, int year)
        {
            CharacterData.currentAge += year - previousYear;
            previousYear = year;
        }
        private void OnCharacterTurnBegin(CharacterBase character)
        {
            if(character != this) return;
                UpdateEffectList();
        }

        private void Start()
        {
            UpdateLevel();
        }

        #region Combat

        public void TakeDamage(CharacterData attacker, CharacterData defender, int damage)
        {
            //TODO:结算必定闪避的影响因素，如法术效果为下次攻击必定闪避
            var isDodge = false;
            if (CheckDodge(attacker.Jingjie.JingjieLevel, defender.Jingjie.JingjieLevel, isDodge)) return;
            defender.currentHealth = Mathf.Max(0, defender.currentHealth - damage);
        }

        /// <summary>
        /// 判断攻击是否闪避
        /// </summary>
        /// <param name="attacker"></param>
        /// <param name="defender"></param>
        /// <returns></returns>
        public bool CheckDodge(JingjieLevel attacker, JingjieLevel defender, bool isDodge)
        {
            if (isDodge)
            {
                return true;
            }
            var dif = attacker - defender;//计算境界差
            var dodgeRate = Random.Range(-0.05f + dif, 0.05f + dif) * 0.5f;//0.05f为修正值，0.5f为每个境界相差命中率
            //如果境界未稳固，则丢失40%命中率
            if (!isJingjieFirmed)
            {
                dodgeRate += 0.4f;
            }
            //如果神识涣散，则丢失40%命中率
            if (isShenShiHuanSan)
            {
                dodgeRate += 0.4f;
            }
            return Random.Range(0f, 1f) < dodgeRate;
        }

        public bool CheckDodge()
        {
            var dodgeRate = 1f;
            //如果境界未稳固，则丢失40%命中率
            if (!isJingjieFirmed)
            {
                dodgeRate -= 0.4f;
            }
            //如果神识涣散，则丢失40%命中率
            if (isShenShiHuanSan)
            {
                dodgeRate -= 0.4f;
            }
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
        
        public void SetPlayerFacingDirection(float direction)
        {
            faceDirection = direction switch 
            {
                > 0 => 1,
                < 0 => -1,
                _ => (int)transform.localScale.x
            };
            //Debug.Log(direction);
            transform.localScale = new Vector3(faceDirection, transform.localScale.y, transform.localScale.z);
        }

        private void UpdateEffectList()
        {
            for (var i = 0; i < Effects.Count; i++)
            {
                if (Effects[i].effectDuration != EffectDuration.Sustainable) return;
                Effects[i].round--;
                if ( Effects[i].round <= 0)
                {
                    Effects[i].OnEffectEnd(this);
                    Effects.RemoveAt(i);
                    i--;
                    break;
                }
                Effects[i].OnEffectExecute();
            }
        }
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
