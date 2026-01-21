using System;
using System.Collections;
using System.Collections.Generic;
using TXDCL.Combat;
using TXDCL.XiuLian.FuShu;
using TXDCL.XiuLian.GongFa;
using TXDCL.Inventory;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

namespace TXDCL.Character
{
    [RequireComponent(typeof(GongFaProcessor))]
    [RequireComponent(typeof(CombatMovement))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class CharacterBase : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("CharacterData")]
        [SerializeField] private CharacterData templateData;
        public CharacterData CharacterData;
        public CharacterData CharacterJingjieData;//角色境界属性
        public CharacterData CharacterEquipmentData;//角色装备属性
        public CharacterData CharacterGongFaData;//角色功法属性
        public CharacterData CharacterEffectsData;//角色Buff属性
        private string JingjieKey => CharacterData != null
            ? CharacterData.Jingjie.JingjieLevel.ToString() + CharacterData.Jingjie.miniJingjieLevel : null;

        [Header("FaShu&GongFa")] 
        protected GongFaProcessor gongFaProcessor => GetComponent<GongFaProcessor>();
        public readonly List<FaShuData> currentFaShuList = new();//不可编辑的装备上的法术列表，只用于法术的调用
        [SerializeField] private List<FaShuData> tempFaShuList = new();//可编辑的装备上的法术列表，用于初始化角色法术列表
        public List<FaShuData> PotentialFaShuList = new();//习得的所有法术
        
        [Header("Components")]
        public Animator animator;
        public BoxCollider2D Collider;
        private float faceDirection;
        private int previousYear = 1;
        
        [Header("Animation")]
        private static readonly int IsMoving = Animator.StringToHash("isMoving");
        private static readonly int IsHurt = Animator.StringToHash("isHurt");
        private static readonly int IsDead = Animator.StringToHash("isDead");
        protected Rigidbody2D rigidBody2D => GetComponent<Rigidbody2D>();
        
        [Header("Bools")] 
        public bool isIconFacingLeft;//角色素材朝向是否为左
        public bool isMoving;//是否正在移动
        public bool isHurt;//是否受伤
        public bool isDead;//是否死亡
        public bool isZouHuoRuMo;//是否走火入魔
        public bool isShenShiHuanSan;//是否神识涣散,神识涣散状态将减少40%命中率
        public bool isJingjieUnstable;//境界是否稳固,未稳固将减少40%命中率
        public bool isShenShiPenetrated;//是否被神识洞穿,被洞穿的目标可以实时查看基础属性
        
        [Header("Combat")] 
        public List<CharacterBase> Allies = new();
        public List<CharacterBase> Enemies = new();
        protected CombatMovement combatMovement => GetComponent<CombatMovement>();
        
        [Header("Inventory")]
        [SerializeField]private InventoryBag templateInventoryBag;
        public InventoryBag InventoryBag;
        
        protected virtual void Awake()
        {
            if (templateData != null)
            {
                CharacterData = Instantiate(templateData);
                CharacterJingjieData = Instantiate(templateData);
                CharacterEquipmentData = Instantiate(templateData);
                CharacterGongFaData = Instantiate(templateData);
                CharacterEffectsData = Instantiate(templateData);
            }
            gongFaProcessor.InitializeGongFa(CharacterData, CharacterGongFaData);
            animator = GetComponent<Animator>();
            Collider = Collider == null ? GetComponent<BoxCollider2D>() : Collider;
            if (templateInventoryBag != null)
            {
                InventoryBag = Instantiate(templateInventoryBag);
                InventoryBag.InitializeData();
            }
            currentFaShuList.Clear();
            foreach (var FaShu in tempFaShuList)
            {
                currentFaShuList.Add(Instantiate(FaShu));
            }
        }

        private void OnEnable()
        {
            EventHandler.GameDateEvent += OnGameDateEvent;
            EventHandler.BeforeCombatBeginEvent += OnBeforeCombatBeginEvent;
            //EventHandler.CharacterTurnBeginEvent += OnCharacterTurnBeginEvent;
        }
        private void OnDisable()
        {
            EventHandler.GameDateEvent -= OnGameDateEvent;
            EventHandler.BeforeCombatBeginEvent -= OnBeforeCombatBeginEvent;
            //EventHandler.CharacterTurnBeginEvent -= OnCharacterTurnBeginEvent;
        }

        private void OnGameDateEvent(int day, int month, int year)
        {
            CharacterData.currentAge += year - previousYear;
            previousYear = year;
        }
        protected virtual void OnBeforeCombatBeginEvent()
        {
            Allies.Clear();
            Enemies.Clear();
            ResetFaShuCoolDown_PrepareTurns();
        }
        protected virtual void OnCharacterTurnBeginEvent(CharacterBase character)
        {
            if(character != this) return;
            CharacterData.JingShenLi = CharacterData.ShenShi / 100;
            CharacterData.currentStamina =
                Mathf.Min(CharacterData.currentStamina + (int)(CharacterData.maxStamina * 0.05f),
                    CharacterData.maxStamina);
            CharacterData.currentMovement = CharacterData.maxMovementPerTurn;
            foreach (var fashu in currentFaShuList)
            {
                fashu.CurrentCoolDownTime--;
            }
            DistributeDaoCangs();
            UpdateEffectList();
        }

        private void Start()
        {
            UpdateData();
            ResetCharacterData();
        }
        protected virtual void Update()
        {
            SwitchAnimation();
        }
        
        protected virtual void SwitchAnimation()
        {
            animator.SetBool(IsMoving, isMoving);
            animator.SetBool(IsDead, isDead);
            if (isHurt)
            {
                animator.SetTrigger(IsHurt);
                isHurt = false;
            }
        }
        #region Combat

        public void TakeDamage(CharacterBase attacker, CharacterBase defender, int damage)
        {
            //TODO:结算必定闪避的影响因素，如法术效果为下次攻击必定闪避
            var isDodge = false;
            if (CheckDodge(attacker, defender, isDodge)) return;
            //Debug.Log($"{defender.CharacterData.characterName}'s Health: {defender.CharacterData.currentHealth}");
            defender.CharacterData.currentHealth = Mathf.Max(0, defender.CharacterData.currentHealth - damage);
            defender.isHurt = true;
            if (defender.CharacterData.currentHealth <= 0)
            {
                defender.CharacterData.currentHealth = 0;
                defender.isDead = true;
            }
            //TODO:后面把UI显示放在Buff部分，因为每个NPC也需要实时结算属性UI
            if (defender == GameManager.Instance.Player)
                StartCoroutine(CharacterStatsPanel.Instance.UpdateCharacterStats(GameManager.Instance.Player));
            //Debug.Log($"{defender.CharacterData.characterName}'s Health: {defender.CharacterData.currentHealth}");
        }
        /// <summary>
        /// 判断攻击是否闪避
        /// </summary>
        /// <param name="attacker"></param>
        /// <param name="defender"></param>
        /// <returns></returns>
        private bool CheckDodge(CharacterBase attacker, CharacterBase defender, bool isDodge)
        {
            if (isDodge)
            {
                return true;
            }
            if (attacker == defender) return false;
            var dif = attacker.CharacterData.Jingjie.JingjieLevel - defender.CharacterData.Jingjie.JingjieLevel;//计算大境界差
            var miniDif = attacker.CharacterData.Jingjie.miniJingjieLevel - defender.CharacterData.Jingjie.miniJingjieLevel;//计算小境界差
            var accurateRate = Random.Range(-0.05f + dif * 0.5f + miniDif * 0.1f + 1, 0.05f + dif * 0.5f + miniDif * 0.1f + 1);//0.05f为修正值，0.5f为每个大境界相差命中率，0.1f为每个小境界差值
            
            //如果境界未稳固，则丢失40%命中率
            if (attacker.isJingjieUnstable)
            {
                accurateRate -= accurateRate * 0.4f;
            }
            //如果神识涣散，则丢失40%命中率
            if (attacker.isShenShiHuanSan)
            {
                accurateRate -= accurateRate * 0.4f;
            }
            //随机值需大于精准率才触发闪避
            return Random.Range(0f, 1f) > accurateRate;
        }
        
        /// <summary>
        /// 根据输入的数值设置角色的朝向
        /// </summary>
        /// <param name="direction"></param>
        public void SetCharacterFacingDirection(float direction)
        {
            direction = isIconFacingLeft ? -direction : direction;
            faceDirection = direction switch 
            {
                > 0 => 1,
                < 0 => -1,
                _ => (int)transform.localScale.x
            };
            transform.localScale = new Vector3(faceDirection, transform.localScale.y, transform.localScale.z);
        }
        
        /// <summary>
        /// 根据战斗中的回合变化，结算临时buff
        /// </summary>
        private void UpdateEffectList()
        {
            for (var i = 0; i < CharacterData.TemporaryEffects.Count; i++)
            {
                if (CharacterData.TemporaryEffects[i].effectDuration != EffectDuration.Sustainable) return;
                CharacterData.TemporaryEffects[i].round--;
                if (CharacterData.TemporaryEffects[i].round <= 0)
                {
                    CharacterData.TemporaryEffects[i].OnEffectEnd(this);
                    CharacterData.TemporaryEffects.RemoveAt(i);
                    i--;
                    break;
                }
                CharacterData.TemporaryEffects[i].OnEffectExecute();
            }
        }
        /// <summary>
        /// 根据角色的最大单回合获取的道藏数量和灵根比例分配道藏
        /// </summary>
        private void DistributeDaoCangs()
        {
            var modifier = 1f / (CharacterData.MetalLingGen + CharacterData.WoodLingGen + CharacterData.WaterLingGen +
                                   CharacterData.FireLingGen + CharacterData.EarthLingGen);
            var totalDaoCang = CharacterData.maxDaocangPerTurn;
            if (totalDaoCang <= 0) return;
            var LingGens = new[]
            {
                CharacterData.MetalLingGen * modifier,
                CharacterData.WoodLingGen * modifier,
                CharacterData.WaterLingGen * modifier,
                CharacterData.FireLingGen * modifier,
                CharacterData.EarthLingGen * modifier
            };
            var DaoCangs = new List<int> { 0, 0, 0, 0, 0 };
            //设置最大循环数防止陷入死循环
            var LoopMaxCount = 0;
            while (totalDaoCang > 0 && LoopMaxCount < 9999)
            {
                var value = Random.Range(0f, 1f);
                for (var i = 0; i < LingGens.Length; i++)
                {
                    value -= LingGens[i];
                    if (!(value < 0)) continue;
                    DaoCangs[i] += 1;
                    totalDaoCang -= 1;
                    break;
                }
                LoopMaxCount++;
                if(LoopMaxCount >= 9999) Debug.Log("Invalid Daocang Amount");
            }
            CharacterData.currentMetalDaocang = DaoCangs[0];
            CharacterData.currentWoodDaocang = DaoCangs[1];
            CharacterData.currentWaterDaocang = DaoCangs[2];
            CharacterData.currentFireDaocang = DaoCangs[3];
            CharacterData.currentEarthDaocang = DaoCangs[4];
        }

        /// <summary>
        /// 重置法术冷却以及准备回合
        /// </summary>
        public void ResetFaShuCoolDown_PrepareTurns()
        {
            foreach (var fashu in currentFaShuList)
            {
                fashu.CurrentCoolDownTime = 0;
                fashu.currentPrepareTurns = 0;
            }
        }
        #endregion

        
        #region XiuLian
        /// <summary>
        /// 根据角色的境界结算境界属性数据
        /// </summary>
        public void UpdateLevel()
        {
            var jingjie = CharacterManager.Instance.GetJingjie(JingjieKey);
            if (jingjie == null) return;
            var data = jingjie.JingjieData;
            CharacterData.nextExp = data.NextEXP;
            CharacterJingjieData.maxAge = data.MaxAge;
            CharacterJingjieData.maxHealth = data.MaxHealth;
            CharacterJingjieData.maxStamina = data.MaxStamina;
            CharacterJingjieData.maxMana = data.MaxMana;
            CharacterJingjieData.Attack = data.Attack;
            CharacterJingjieData.Reaction = data.Reaction;
            CharacterJingjieData.maxMovementPerTurn = data.MaxMovementPerTurn;
            CharacterJingjieData.ShenShiStrength = data.ShenShiStrength;
            CharacterJingjieData.maxDaocangPerTurn = data.MaxDaocangPerTurn;
        }
        /// <summary>
        /// 判断是否角色境界是否突破
        /// </summary>
        public void CheckLevelUp()
        {
            while (CharacterData.currentExp >= CharacterData.nextExp && CharacterData.nextExp != 0)
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
        /// <summary>
        /// 重置角色基础数据，一般用于非真实的战斗后刷新
        /// </summary>
        public void ResetCharacterData()
        {
            if (CharacterData == null) return;
            CharacterData.currentHealth = CharacterData.maxHealth;
            CharacterData.currentMana = CharacterData.maxMana;
            CharacterData.currentStamina = CharacterData.maxStamina;
            CharacterData.ShenShi = CharacterData.ShenShiStrength;
        }
        /// <summary>
        /// 按照步骤分别更新三个角色数据，首先更新境界带来的角色属性数据，然后是装备，最后为功法
        /// </summary>
        public void UpdateData()
        {
            UpdateLevel();
            if (InventoryBag != null)
                InventoryBag.UpdateProperty(CharacterEquipmentData);
            gongFaProcessor.UpdateProperty();
            CharacterData.maxAge = CharacterJingjieData.maxAge + CharacterEquipmentData.maxAge + CharacterGongFaData.maxAge + CharacterEffectsData.maxAge;
            CharacterData.maxVigor = CharacterJingjieData.maxVigor + CharacterEquipmentData.maxVigor + CharacterGongFaData.maxVigor + CharacterEffectsData.maxVigor;
            CharacterData.maxDanDu = CharacterJingjieData.maxDanDu + CharacterEquipmentData.maxDanDu + CharacterGongFaData.maxDanDu + CharacterEffectsData.maxDanDu;
            CharacterData.maxShaQi = CharacterJingjieData.maxShaQi + CharacterEquipmentData.maxShaQi + CharacterGongFaData.maxShaQi + CharacterEffectsData.maxShaQi;
            CharacterData.maxHealth = CharacterJingjieData.maxHealth + CharacterEquipmentData.maxHealth + CharacterGongFaData.maxHealth + CharacterEffectsData.maxHealth;
            CharacterData.maxMana = CharacterJingjieData.maxMana + CharacterEquipmentData.maxMana + CharacterGongFaData.maxMana + CharacterEffectsData.maxMana;
            CharacterData.maxStamina = CharacterJingjieData.maxStamina + CharacterEquipmentData.maxStamina + CharacterGongFaData.maxStamina + CharacterEffectsData.maxStamina;
            CharacterData.Attack = CharacterJingjieData.Attack + CharacterEquipmentData.Attack + CharacterGongFaData.Attack + CharacterEffectsData.Attack;
            CharacterData.Reaction = CharacterJingjieData.Reaction + CharacterEquipmentData.Reaction + CharacterGongFaData.Reaction + CharacterEffectsData.Reaction;
            CharacterData.Speed = CharacterJingjieData.Speed + CharacterEquipmentData.Speed  + CharacterGongFaData.Speed  + CharacterEffectsData.Speed ;
            CharacterData.maxMovementPerTurn = CharacterJingjieData.maxMovementPerTurn + CharacterEquipmentData.maxMovementPerTurn + CharacterGongFaData.maxMovementPerTurn + CharacterEffectsData.maxMovementPerTurn;
            CharacterData.maxDaocangPerTurn = CharacterJingjieData.maxDaocangPerTurn + CharacterEquipmentData.maxDaocangPerTurn + CharacterGongFaData.maxDaocangPerTurn + CharacterEffectsData.maxDaocangPerTurn;
            CheckCharacterDataOverflow();
            gongFaProcessor.XiuLianSpeed = (int)((CharacterData.MainGongFaBasicSpeed + CharacterData.SubGongFaBasicSpeed) * (1 + CharacterData.MainGongFaAdditionalSpeed));
        }
        
        /// <summary>
        /// 判断当前角色的基础数据是否大于最大数据，如当前生命值是否大于最大生命值等，若大于则重置
        /// </summary>
        public void CheckCharacterDataOverflow()
        {
            CharacterData.currentAge = CharacterData.currentAge < CharacterData.maxAge ? CharacterData.currentAge : CharacterData.maxAge;
            CharacterData.currentVigor = CharacterData.currentVigor < CharacterData.maxVigor ? CharacterData.currentVigor : CharacterData.maxVigor;
            CharacterData.currentDanDu = CharacterData.currentDanDu < CharacterData.maxDanDu ? CharacterData.currentDanDu : CharacterData.maxDanDu;
            CharacterData.currentShaQi = CharacterData.currentShaQi < CharacterData.maxShaQi ? CharacterData.currentShaQi : CharacterData.maxShaQi;
            CharacterData.currentHealth = CharacterData.currentHealth < CharacterData.maxHealth ? CharacterData.currentHealth : CharacterData.maxHealth;
            CharacterData.currentMana = CharacterData.currentMana < CharacterData.maxMana ? CharacterData.currentMana : CharacterData.maxMana;
            CharacterData.currentStamina = CharacterData.currentStamina < CharacterData.maxStamina ? CharacterData.currentStamina : CharacterData.maxStamina;
        }
        #endregion
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!CombatManager.Instance.isCombating || !CharacterStatsPanel.Instance.CharaterStats.activeInHierarchy || !isShenShiPenetrated || !CombatGridManager.Instance.canDisplayCharacterStats) return;
            StartCoroutine(ShowCharacterStats());
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!CombatManager.Instance.isCombating || !CharacterStatsPanel.Instance.CharaterStats.activeInHierarchy || !isShenShiPenetrated || !CombatGridManager.Instance.canDisplayCharacterStats) return;
            StopAllCoroutines();
        }

        private IEnumerator ShowCharacterStats()
        {
            yield return new WaitForSeconds(2);
            StartCoroutine(CharacterStatsPanel.Instance.UpdateCharacterStats(this));
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
