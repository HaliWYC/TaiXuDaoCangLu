using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TXDCL.Character;
using Unity.Mathematics;

namespace TXDCL.XiuLian.GongFa
{
    public class GongFaProcessor : MonoBehaviour
    {
        public CharacterData characterData;
        public List<GongFaData> MainGongFas = new();
        public List<GongFaData> SubGongFas = new();
        private int XiuLianSpeed;

        private int MainGongFaBasicSpeed;
        private int SubGongFaBasicSpeed;
        private float MainGongFaAdditionalSpeed;
        
        [Header("Time")] 
        private int previousMonth;
        private int previousYear;

        private bool isReachLimit;
        private void Awake()
        {
            //Test
            //TODO:后续根据数据保存系统储存时间
            previousMonth = 1;
            previousYear = 1;
            isReachLimit = false;
        }

        private void Start()
        {
            //Test
            MainGongFaBasicSpeed += MainGongFas.Sum(GongFa => GongFa.BasicXiuLianSpeed);
            SubGongFaBasicSpeed += SubGongFas.Sum(GongFa => GongFa.BasicXiuLianSpeed);
            MainGongFaAdditionalSpeed += MainGongFas.Sum(GongFa => GongFa.AdditionalXiuLianSpeed);
            XiuLianSpeed = (int)((MainGongFaBasicSpeed + SubGongFaBasicSpeed) * MainGongFaAdditionalSpeed);
            foreach (var property in MainGongFas.SelectMany(MainGF => MainGF.PropertyList))
            {
                characterData.AddProperty(property);
                AddProperty(property);
            }

            foreach (var property in SubGongFas.SelectMany(SubGongFa => SubGongFa.PropertyList))
            {
                characterData.AddProperty(property);
                AddProperty(property);
            }
        }

        private void OnEnable()
        {
            EventHandler.GameDateEvent += OnGameDateEvent;
        }

        private void OnDisable()
        {
            EventHandler.GameDateEvent -= OnGameDateEvent;
        }

        private void OnGameDateEvent(int day, int month, int year)
        {
            var monthDiff = month - previousMonth;
            var yearDiff = year - previousYear;
            previousMonth = month;
            previousYear = year;
            
            var time = monthDiff + yearDiff * 12;
            characterData.currentAge = math.max(0, characterData.currentAge + yearDiff);
            characterData.currentExp = math.max(0, characterData.currentExp + time * XiuLianSpeed);
            if (isReachLimit)
            {
                characterData.currentExp = characterData.nextExp;
                return;
            }
            if(characterData.currentExp >= characterData.nextExp)
                GetComponent<CharacterBase>().CheckUpGrade();
        }

        [ContextMenu("Cheat")]
        private void CheatEXP()
        {
            characterData.currentExp += XiuLianSpeed;
            if (isReachLimit)
            {
                characterData.currentExp = characterData.nextExp;
                return;
            }
            if(characterData.currentExp >= characterData.nextExp)
                GetComponent<CharacterBase>().CheckUpGrade();
        }

        private void AddProperty(Property property)
        {
            switch (property.propertyType)
            {
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
        
        private void SubtractProperty(Property property)
        {
            switch (property.propertyType)
            {
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

