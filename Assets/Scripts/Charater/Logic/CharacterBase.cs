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
        public CharacterData CharacterData;//角色总属性,为角色基础、境界、装备以及功法总和
        public CharacterData CharacterBasicData;//角色基础属性
        public CharacterData CharacterJingjieData;//角色境界属性
        public CharacterData CharacterEquipmentData;//角色装备属性
        public CharacterData CharacterGongFaData;//角色功法属性
        //public CharacterData CharacterEffectsData;//角色Buff属性
        private string JingjieKey => CharacterData != null
            ? CharacterData.Jingjie.JingjieLevel.ToString() + CharacterData.Jingjie.miniJingjieLevel : null;

        [Header("FaShu&GongFa")] 
        protected GongFaProcessor gongFaProcessor => GetComponent<GongFaProcessor>();
        public List<FaShuData> currentFaShuList = new();//不可编辑的装备上的法术列表，只用于法术的调用
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
            if (CharacterBasicData != null)
            {
                CharacterData = Instantiate(CharacterBasicData);
                CharacterJingjieData = Instantiate(CharacterManager.Instance.characterTemplateData);
                CharacterEquipmentData = Instantiate(CharacterManager.Instance.characterTemplateData);
                CharacterGongFaData = Instantiate(CharacterManager.Instance.characterTemplateData);
                //CharacterEffectsData = Instantiate(CharacterManager.Instance.characterTemplateData);
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
            ResetDaoCang();
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
                fashu.CurrentCoolDownTime = Mathf.Max(0, fashu.CurrentCoolDownTime - 1);
            }
            DistributeDaoCangs();
            UpdateEffectList();
        }

        private void Start()
        {
            //TODO:后面根据存档读取数据
            UpdateCharacterData();
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
            //TODO:结算必定命中的影响因素，如法术效果为下次攻击必定命中
            //TODO:结算必定闪避的影响因素，如法术效果为下次攻击必定闪避
            var absoluteAccurate = false;
            var absoluteDodge = false;
            if (CheckDodge(attacker, defender, absoluteAccurate, absoluteDodge)) return;
            //TODO:结算必定暴击的影响因素，如法术效果为下次攻击必定暴击
            var absoluteCritical = false;
            var isCritical = CheckCritical(attacker, defender, absoluteCritical);
            //Debug.Log(isCritical);
            if (isCritical)
            {
                damage = (int)(damage *attacker.CharacterData.criticalMultiple / 100f);
            }
            //Debug.Log($"{defender.CharacterData.characterName}'s Health: {defender.CharacterData.currentHealth}");
            var finalDamage = CalculateFinalDamage(attacker, defender, damage);
            defender.CharacterData.currentHealth = Mathf.Max(0, defender.CharacterData.currentHealth - finalDamage);
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
        private bool CheckDodge(CharacterBase attacker, CharacterBase defender, bool absoluteAccurate, bool absoluteDodge)
        {
            if (absoluteAccurate)
            {
                return false;
            }
            if (absoluteDodge)
            {
                return true;
            }
            if (attacker == defender) return false;
            var dif = attacker.CharacterData.Jingjie.JingjieLevel - defender.CharacterData.Jingjie.JingjieLevel;//计算大境界差
            var miniDif = attacker.CharacterData.Jingjie.miniJingjieLevel - defender.CharacterData.Jingjie.miniJingjieLevel;//计算小境界差
            var JingJieModifier = Random.Range(-5f + dif * 50f + miniDif * 10f, 5f + dif * 50f + miniDif * 10f);//5f为修正值，50f为每个大境界相差命中率，10f为每个小境界差值
            var accurateRate = attacker.CharacterData.accuracy - defender.CharacterData.dodgeRate;
            //Debug.Log("AccurateRate:" + accurateRate);
            //Debug.Log("JingjieModifier:" + JingJieModifier);
            //最终命中为攻击方命中率减去防御方闪避率加上境界修正（每一个大境界相差50%，小境界相差10%，以及5%的随机修正值）
            var finalAccurate = accurateRate + JingJieModifier;
            //Debug.Log("FinalAccurate:" + finalAccurate);
            //随机值需大于精准率才触发闪避
            return Random.Range(0f, 100f) > finalAccurate;
        }

        private bool CheckCritical(CharacterBase attacker, CharacterBase defender, bool absoluteCritical)
        {
            if (absoluteCritical)
                return true;
            if (attacker == defender) return false;
            var dif = attacker.CharacterData.Jingjie.JingjieLevel - defender.CharacterData.Jingjie.JingjieLevel;//计算大境界差
            var miniDif = attacker.CharacterData.Jingjie.miniJingjieLevel - defender.CharacterData.Jingjie.miniJingjieLevel;//计算小境界差
            var JingJieModifier = Random.Range(-5f + dif * 50f + miniDif * 10f, 5f + dif * 50f + miniDif * 10f);//5f为修正值，50f为每个大境界相差命中率，10f为每个小境界差值
            var criticalRate = attacker.CharacterData.criticalRate - defender.CharacterData.criticalResistance;
            //Debug.Log("criticalRate:" + criticalRate);
            //Debug.Log("CJingJieModifier:" + JingJieModifier);
            //最终暴击率为攻击方暴击率减去防御方化劲率加上境界修正（每一个大境界相差50%，小境界相差10%，以及5%的随机修正值）
            var finalCritical = criticalRate + JingJieModifier;
            //Debug.Log("finalCritical:" + finalCritical);
            //暴击率需要大于随机值才触发暴击
            return finalCritical > Random.Range(0f, 100f);
        }

        private int CalculateFinalDamage(CharacterBase attacker, CharacterBase defender, int damage)
        {
            //计算减伤率，由防御方的防御值/防御值+减伤常数，并且最大值为80%
            var damageReductionRate = Mathf.Min((float)defender.CharacterData.defense / (defender.CharacterData.defense + Settings.DamageReductionConstant), 0.8f);
            //Debug.Log(damage);
            //Debug.Log((int)(damage * (1 - damageReductionRate)));
            return (int)(damage * (1 - damageReductionRate));
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
            var totalDaoCang = CharacterData.maxDaocangPerTurn;//TODO：后续加上上回合容纳的道藏
            if (totalDaoCang <= 0)
            {
                ResetDaoCang();
                return;
            }
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
        private void ResetFaShuCoolDown_PrepareTurns()
        {
            foreach (var fashu in currentFaShuList)
            {
                fashu.CurrentCoolDownTime = 0;
                fashu.currentPrepareTurns = 0;
            }
        }

        private void ResetDaoCang()
        {
            CharacterData.currentMetalDaocang = 0;
            CharacterData.currentWoodDaocang = 0;
            CharacterData.currentWaterDaocang = 0;
            CharacterData.currentFireDaocang = 0;
            CharacterData.currentEarthDaocang = 0;
        }
        #endregion

        
        #region XiuLian
        /// <summary>
        /// 根据角色的境界结算境界属性数据
        /// </summary>
        public void UpdateLevel()
        {
            var jingjie = CharacterJingjieManager.Instance.GetJingjie(JingjieKey);
            if (jingjie == null) return;
            var data = jingjie.JingjieData;
            CharacterData.nextExp = data.NextEXP;
            CharacterJingjieData.maxAge = data.MaxAge;
            CharacterJingjieData.Agility = data.Agility;
            CharacterJingjieData.Fitness = data.Fitness;
            CharacterJingjieData.Intelligence = data.Intelligence;
            CharacterJingjieData.Meridian = data.Meridian;
            CharacterJingjieData.Strength = data.Strength;
            CharacterJingjieData.Tenacity = data.Tenacity;
            CharacterJingjieData.maxHealth = data.MaxHealth;
            CharacterJingjieData.maxStamina = data.MaxStamina;
            CharacterJingjieData.maxMana = data.MaxMana;
            CharacterJingjieData.maxSpeed = data.MaxSpeed;
            CharacterJingjieData.reaction = data.Reaction;
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
                //CharacterData.Jingjie = CharacterJingjieManager.Instance.GetJingjie(JingjieKey);
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
        public void UpdateCharacterData()
        {
            UpdateLevel();
            if (InventoryBag != null)
                InventoryBag.UpdateProperty(CharacterEquipmentData);
            gongFaProcessor.UpdateProperty();
            //CharacterEffectsData.ResetProperty();
            AddCharacterData(CharacterJingjieData);
            AddCharacterData(CharacterEquipmentData);
            AddCharacterData(CharacterGongFaData);
            //AddCharacterData(CharacterEffectsData);
            CheckCharacterData();
            CharacterData.MainAttributeToCharacterData();
            CheckCharacterDataOverflow();
            gongFaProcessor.XiuLianSpeed = (int)((CharacterData.MainGongFaBasicSpeed + CharacterData.SubGongFaBasicSpeed) * (1 + CharacterData.MainGongFaAdditionalSpeed));
            CharacterData.currentSpeed = CharacterData.maxSpeed;
        }
        private void AddCharacterData(CharacterData NewData)
        {
            //Main Attribute
            CharacterData.Agility += NewData.Agility;
            CharacterData.Fitness += NewData.Fitness;
            CharacterData.Intelligence += NewData.Intelligence;
            CharacterData.Meridian += NewData.Meridian;
            CharacterData.Strength += NewData.Strength;
            CharacterData.Tenacity += NewData.Tenacity;
            //Basic Combat Attribute
            CharacterData.maxAge += NewData.maxAge;
            CharacterData.maxVigor += NewData.maxVigor;
            CharacterData.maxDuSu += NewData.maxDuSu;
            CharacterData.maxShaQi += NewData.maxShaQi;
            CharacterData.maxHealth += NewData.maxHealth;
            CharacterData.maxMana += NewData.maxMana;
            CharacterData.maxStamina += NewData.maxStamina;
            CharacterData.attack += NewData.attack;
            CharacterData.defense += NewData.defense;
            CharacterData.criticalRate += NewData.criticalRate;
            CharacterData.criticalMultiple += NewData.criticalMultiple;
            CharacterData.criticalResistance += NewData.criticalResistance;
            CharacterData.accuracy += NewData.accuracy;
            CharacterData.dodgeRate += NewData.dodgeRate;
            CharacterData.reaction += NewData.reaction;
            CharacterData.maxSpeed += NewData.maxSpeed;
            CharacterData.maxMovementPerTurn += NewData.maxMovementPerTurn;
            CharacterData.maxDaocangPerTurn += NewData.maxDaocangPerTurn;
            CharacterData.ShenShiStrength += NewData.ShenShiStrength;
            CharacterData.MainGongFaBasicSpeed += NewData.MainGongFaBasicSpeed;
            CharacterData.MainGongFaAdditionalSpeed += NewData.MainGongFaAdditionalSpeed;
            CharacterData.SubGongFaBasicSpeed += NewData.SubGongFaBasicSpeed;
        }
        
        /// <summary>
        /// 判断当前角色的基础数据是否大于最大数据，如当前生命值是否大于最大生命值等，若大于则重置
        /// </summary>
        private void CheckCharacterDataOverflow()
        {
            CharacterData.currentAge = CharacterData.currentAge < CharacterData.maxAge ? CharacterData.currentAge : CharacterData.maxAge;
            CharacterData.currentVigor = CharacterData.currentVigor < CharacterData.maxVigor ? CharacterData.currentVigor : CharacterData.maxVigor;
            CharacterData.currentDuSu = CharacterData.currentDuSu < CharacterData.maxDuSu ? CharacterData.currentDuSu : CharacterData.maxDuSu;
            CharacterData.currentShaQi = CharacterData.currentShaQi < CharacterData.maxShaQi ? CharacterData.currentShaQi : CharacterData.maxShaQi;
            CharacterData.currentHealth = CharacterData.currentHealth < CharacterData.maxHealth ? CharacterData.currentHealth : CharacterData.maxHealth;
            CharacterData.currentMana = CharacterData.currentMana < CharacterData.maxMana ? CharacterData.currentMana : CharacterData.maxMana;
            CharacterData.currentStamina = CharacterData.currentStamina < CharacterData.maxStamina ? CharacterData.currentStamina : CharacterData.maxStamina;
            CharacterData.currentSpeed = CharacterJingjieData.currentSpeed < CharacterData.maxSpeed ? CharacterData.currentSpeed : CharacterData.maxSpeed;
        }

        private void CheckCharacterData()
        {
            //Main Attribute
            CharacterData.Strength = Mathf.Max(CharacterData.Strength, 0);
            CharacterData.Agility = Mathf.Max(CharacterData.Agility, 0);
            CharacterData.Intelligence = Mathf.Max(CharacterData.Intelligence, 0);
            CharacterData.Fitness = Mathf.Max(CharacterData.Fitness, 0);
            CharacterData.Tenacity = Mathf.Max(CharacterData.Tenacity, 0);
            CharacterData.Meridian = Mathf.Max(CharacterData.Meridian, 0);
            //Basic Combat Attribute
            CharacterData.maxAge = Mathf.Max(CharacterData.maxAge, 0);
            CharacterData.maxVigor = Mathf.Max(CharacterData.maxVigor, 0);
            CharacterData.maxDuSu = Mathf.Max(CharacterData.maxDuSu, 0);
            CharacterData.maxShaQi = Mathf.Max(CharacterData.maxShaQi, 0);
            CharacterData.maxHealth = Mathf.Max(CharacterData.maxHealth, 0);
            CharacterData.maxMana = Mathf.Max(CharacterData.maxMana, 0);
            CharacterData.maxStamina = Mathf.Max(CharacterData.maxStamina, 0);
            CharacterData.attack = Mathf.Max(CharacterData.attack, 0);
            CharacterData.defense = Mathf.Max(CharacterData.defense, 0);
            CharacterData.criticalRate = Mathf.Max(CharacterData.criticalRate, 0);
            CharacterData.criticalMultiple = Mathf.Max(CharacterData.criticalMultiple, 0);
            CharacterData.criticalResistance = Mathf.Max(CharacterData.criticalResistance, 0);
            CharacterData.accuracy = Mathf.Max(CharacterData.accuracy, 0);
            CharacterData.dodgeRate = Mathf.Max(CharacterData.dodgeRate, 0);
            CharacterData.reaction = Mathf.Max(CharacterData.reaction, 0);
            CharacterData.maxSpeed = Mathf.Max(CharacterData.maxSpeed, 0);
            CharacterData.maxMovementPerTurn = Mathf.Max(CharacterData.maxMovementPerTurn, 0);
            CharacterData.maxDaocangPerTurn = Mathf.Max(CharacterData.maxDaocangPerTurn, 0);
            CharacterData.ShenShiStrength = Mathf.Max(CharacterData.ShenShiStrength, 0);
            CharacterData.MainGongFaBasicSpeed = Mathf.Max(CharacterData.MainGongFaBasicSpeed, 0);
            CharacterData.MainGongFaAdditionalSpeed = Mathf.Max(CharacterData.MainGongFaAdditionalSpeed, 0);
            CharacterData.SubGongFaBasicSpeed = Mathf.Max(CharacterData.SubGongFaBasicSpeed, 0);
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
