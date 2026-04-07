using System;
using System.Collections.Generic;
using TXDCL.Effect;
using UnityEngine;

namespace TXDCL.Character
{
    [CreateAssetMenu(fileName = "CharacterData", menuName = "Character/CharacterData")]
    public class CharacterData : ScriptableObject
    {
        [Header("Basic Information")] 
        public string characterName; //角色名字
        public Sprite characterSprite;
        public int currentAge; //年龄
        public int maxAge; //寿元
        public int currentExp; //当前经验
        public int nextExp; //升级所需经验
        public Jingjie Jingjie;
        public int currentVigor;//当前精力，用于进行非战斗类活动，如顿悟功法/法术，炼丹，炼器等
        public int maxVigor;//最大精力
        public int currentDuSu;//当前毒素,毒素超过最大毒素一定比例会获得Debuff,食用丹药或中毒将积累毒素
        public int maxDuSu;//最大毒素
        public int currentShaQi;//当前煞气,煞气超过一定比例将会获得Debuff，短时间内大量杀死生灵或其他特殊情况将积累煞气
        public int maxShaQi;//最大煞气
        public int MainGongFaBasicSpeed;
        public int SubGongFaBasicSpeed;
        public float MainGongFaAdditionalSpeed;
        
        [Header("Basic Combat")] 
        public int currentHealth; //当前气血
        public int maxHealth; //最大气血
        public int currentMana;//当前发力释放法术所消耗资源
        public int maxMana;//最大法力
        public int currentStamina;//当前体力，体力用于释放近战法术
        public int maxStamina;//最大体力
        public int attack; //攻击
        public int defense; //防御
        public float criticalRate;//暴击率，触发暴击时将攻击乘以暴击效果
        public float criticalMultiple;//暴击效果，触发暴击时最终攻击的倍率
        public float criticalResistance;//化劲效果，即防暴率，由暴击率减去防暴率得出最终暴击率
        public float accuracy;//命中率，在法术释放时判断是否命中，由命中率减去闪避率之后计算是否命中
        public float dodgeRate;//闪避率，在被法术命中时判定是否闪避，由命中率减去闪避率之后计算是否命中
        public int Reaction;//反应，战斗中反应越高则越快进入下一个回合
        public int currentSpeed;//当前速度，日常移动速度，战斗中只影响动画速度
        public int maxSpeed;//最大速度
        public int currentMovement;//当前剩余移动力，当前回合中剩余的可移动力
        public int maxMovementPerTurn; //每回合行动力
        

        [Header("Daocang")] 
        public int maxDaocangPerTurn; //每回合总道藏
        public int currentMetalDaocang;//当前剩余锐金道藏
        public int currentWoodDaocang;//当前剩余灵木道藏
        public int currentWaterDaocang;//当前剩余弱水道藏
        public int currentFireDaocang;//当前剩余离火道藏
        public int currentEarthDaocang;//当前剩余厚土道藏
        public int MetalLingGen;//锐金灵根
        public int WoodLingGen;//灵木灵根
        public int WaterLingGen;//弱水灵根
        public int FireLingGen;//离火灵根
        public int EarthLingGen;//厚土灵根

        [Header("Shenshi")] 
        public int ShenShi;//神识会按一定比例在每回合转化为精神力
        public int ShenShiStrength;//神识强度：最大神识
        public int JingShenLi;//释放神识行动所消耗资源
        
        [Header("Effects")]
        public List<EffectData> TemporaryEffects;//暂时性效果如战斗中即战斗后持续状态
        public List<EffectData> PermanentEffects;//永久性效果如天赋、能力等
        
        public void ResetProperty()
        {
            maxAge = 0;
            maxVigor = 0;
            maxDuSu = 0;
            maxShaQi = 0;
            MainGongFaBasicSpeed = 0;
            SubGongFaBasicSpeed = 0;
            MainGongFaAdditionalSpeed = 0;
            maxHealth = 0;
            maxMana = 0;
            maxStamina = 0;
            attack = 0;
            defense = 0;
            criticalRate = 0;
            criticalMultiple = 0;
            criticalResistance = 0;
            accuracy = 0;
            dodgeRate = 0;
            Reaction = 0;
            maxSpeed = 0;
            maxMovementPerTurn = 0;
            maxDaocangPerTurn = 0;
            MetalLingGen = 0;
            WoodLingGen = 0;
            WaterLingGen = 0;
            FireLingGen = 0;
            EarthLingGen = 0;
            ShenShiStrength = 0;
        }
        public void AddProperty(Property property)
        {
            switch (property.propertyType)
            {
                case PropertyType.MaxAge:
                    maxAge += (int)property.value;
                    break;
                case PropertyType.MaxHealth:
                    maxHealth += (int)property.value;
                    break;
                case PropertyType.MaxMana:
                    maxMana += (int)property.value;
                    break;
                case PropertyType.Attack:
                    attack += (int)property.value;
                    break;
                case PropertyType.Defense:
                    defense += (int)property.value;
                    break;
                case PropertyType.CriticalRate:
                    criticalRate += property.value;
                    break;
                case PropertyType.CriticalMultiple:
                    criticalMultiple += property.value;
                    break;
                case PropertyType.CriticalResistance:
                    criticalResistance += property.value;
                    break;
                case PropertyType.Accuracy:
                    accuracy += property.value;
                    break;
                case PropertyType.DodgeRate:
                    dodgeRate += property.value;
                    break;
                case PropertyType.Reaction:
                    Reaction += (int)property.value;
                    break;
                case PropertyType.Speed:
                    maxSpeed += (int)property.value;
                    break;
                case PropertyType.MaxMovementPerTurn:
                    maxMovementPerTurn += (int)property.value;
                    break;
                case PropertyType.MaxDaocangPerTurn:
                    maxDaocangPerTurn += (int)property.value;
                    break;
                case PropertyType.MetalLingGen:
                    MetalLingGen += (int)property.value;
                    break;
                case PropertyType.WoodLingGen:
                    WoodLingGen += (int)property.value;
                    break;
                case PropertyType.WaterLingGen:
                    WaterLingGen += (int)property.value;
                    break;
                case PropertyType.FireLingGen:
                    FireLingGen += (int)property.value;
                    break;
                case PropertyType.EarthLingGen:
                    EarthLingGen += (int)property.value;
                    break;
                case PropertyType.ShenShiStrength:
                    ShenShiStrength += (int)property.value;
                    break;
                case PropertyType.MainGongFaBasicSpeed:
                    MainGongFaBasicSpeed += (int)property.value;
                    break;
                case PropertyType.SubGongFaBasicSpeed:
                    SubGongFaBasicSpeed += (int)property.value;
                    break;
                case PropertyType.MainGongFaAdditionalSpeed:
                    MainGongFaAdditionalSpeed += property.value;
                    break;
            }
        }

        public void SubtractProperty(Property property)
        {
            switch (property.propertyType)
            {
                case PropertyType.MaxAge:
                    maxAge -= (int)property.value;
                    break;
                case PropertyType.MaxHealth:
                    maxHealth -= (int)property.value;
                    break;
                case PropertyType.MaxMana:
                    maxMana -= (int)property.value;
                    break;
                case PropertyType.Attack:
                    attack -= (int)property.value;
                    break;
                case PropertyType.Defense:
                    defense -= (int)property.value;
                    break;
                case PropertyType.CriticalRate:
                    criticalRate -= property.value;
                    break;
                case PropertyType.CriticalMultiple:
                    criticalMultiple -= property.value;
                    break;
                case PropertyType.CriticalResistance:
                    criticalResistance -= property.value;
                    break;
                case PropertyType.Accuracy:
                    accuracy -= property.value;
                    break;
                case PropertyType.DodgeRate:
                    dodgeRate -= property.value;
                    break;
                case PropertyType.Reaction:
                    Reaction -= (int)property.value;
                    break;
                case PropertyType.Speed:
                    maxSpeed -= (int)property.value;
                    break;
                case PropertyType.MaxMovementPerTurn:
                    maxMovementPerTurn -= (int)property.value;
                    break;
                case PropertyType.MaxDaocangPerTurn:
                    maxDaocangPerTurn -= (int)property.value;
                    break;
                case PropertyType.MetalLingGen:
                    MetalLingGen -= (int)property.value;
                    break;
                case PropertyType.WoodLingGen:
                    WoodLingGen -= (int)property.value;
                    break;
                case PropertyType.WaterLingGen:
                    WaterLingGen -= (int)property.value;
                    break;
                case PropertyType.FireLingGen:
                    FireLingGen -= (int)property.value;
                    break;
                case PropertyType.EarthLingGen:
                    EarthLingGen -= (int)property.value;
                    break;
                case PropertyType.ShenShiStrength:
                    ShenShiStrength -= (int)property.value;
                    break;
                case PropertyType.MainGongFaBasicSpeed:
                    MainGongFaBasicSpeed -= (int)property.value;
                    break;
                case PropertyType.SubGongFaBasicSpeed:
                    SubGongFaBasicSpeed -= (int)property.value;
                    break;
                case PropertyType.MainGongFaAdditionalSpeed:
                    MainGongFaAdditionalSpeed -= property.value;
                    break;
            }
        }
    }
}