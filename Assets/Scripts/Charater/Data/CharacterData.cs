using System;
using UnityEngine;

namespace TXDCL.Character
{
    [CreateAssetMenu(fileName = "CharacterData", menuName = "Character/CharacterData")]
    public class CharacterData : ScriptableObject
    {
        [Header("Basic Infor")] public string characterName; //角色名字
        public int currentAge; //年龄
        public int maxAge; //寿元
        public int currentExp; //当前经验
        public int maxExp; //升级所需经验
        public Jingjie Jingjie;

        [Header("Basic Combat")] 
        public int currentHealth; //当前气血
        public int maxHealth; //最大气血
        public int currentMana;//释放法术所消耗资源
        public int maxMana;//最大法力
        public int Attack; //攻击
        public int Reaction;//反应
        public int Speed;//速度
        public int maxMovementPerTurn; //每回合行动力

        [Header("Daocang")] public int maxDaocangPerTurn; //每回合总道藏
        [Range(0f, 1f)] public float MetalLingGen;//锐金灵根
        [Range(0f, 1f)] public float WoodLingGen;//灵木灵根
        [Range(0f, 1f)] public float WaterLingGen;//弱水灵根
        [Range(0f, 1f)] public float FireLingGen;//离火灵根
        [Range(0f, 1f)] public float EarthLingGen;//厚土灵根

        [Header("Shenshi")] 
        public int ShenShi;//神识
        public int ShenShiStrength;//最大神识
        public int JingShenLi;//释放神识行动所消耗资源

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
                    MetalLingGen += property.value;
                    break;
                case PropertyType.WoodLingGen:
                    WoodLingGen += property.value;
                    break;
                case PropertyType.WaterLingGen:
                    WaterLingGen += property.value;
                    break;
                case PropertyType.FireLingGen:
                    FireLingGen += property.value;
                    break;
                case PropertyType.EarthLingGen:
                    EarthLingGen += property.value;
                    break;
                case PropertyType.ShenShi:
                    ShenShi += (int)property.value;
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
                    MetalLingGen -= property.value;
                    break;
                case PropertyType.WoodLingGen:
                    WoodLingGen -= property.value;
                    break;
                case PropertyType.WaterLingGen:
                    WaterLingGen -= property.value;
                    break;
                case PropertyType.FireLingGen:
                    FireLingGen -= property.value;
                    break;
                case PropertyType.EarthLingGen:
                    EarthLingGen -= property.value;
                    break;
                case PropertyType.ShenShi:
                    ShenShi -= (int)property.value;
                    break;
                case PropertyType.ShenShiStrength:
                    ShenShiStrength -= (int)property.value;
                    break;
            }

        }
    }
}
