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
        public int currentDanDu;//当前丹毒,丹毒超过一定比例会获得Debuff,食用丹药将积累丹毒
        public int maxDanDu;//最大丹毒
        public int currentShaQi;//当前煞气,煞气超过一定比例将会获得Debuff，短时间内大量杀死生灵或其他特殊情况将积累煞气
        public int maxShaQi;//最大煞气

        [Header("Basic Combat")] 
        public int currentHealth; //当前气血
        public int maxHealth; //最大气血
        public int currentStamina;//当前体力，体力用于释放近战法术
        public int maxStamina;//最大体力
        public int currentMana;//当前发力释放法术所消耗资源
        public int maxMana;//最大法力
        public int Attack; //攻击
        public int Reaction;//反应
        public int Speed;//速度
        public int currentMovement;//当前剩余移动力
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
        
        public List<EffectData> TemporaryEffects;//暂时性效果如战斗中即战斗后持续状态
        public List<EffectData> PermanentEffects;//永久性效果如天赋、能力等

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
                    Attack += (int)property.value;
                    break;
                case PropertyType.Reaction:
                    Reaction += (int)property.value;
                    break;
                case PropertyType.Speed:
                    Speed += (int)property.value;
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
                    Attack -= (int)property.value;
                    break;
                case PropertyType.Reaction:
                    Reaction -= (int)property.value;
                    break;
                case PropertyType.Speed:
                    Speed -= (int)property.value;
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
            }
        }
    }
}