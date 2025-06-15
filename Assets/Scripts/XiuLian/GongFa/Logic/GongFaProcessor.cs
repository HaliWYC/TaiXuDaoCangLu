using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TXDCL.Character;

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
        private int previousDay;
        private int previousMonth;
        private int previousYear;

        private void Awake()
        {
            //Test
            //TODO:后续根据数据保存系统储存时间
            previousDay = 1;
            previousMonth = 1;
            previousYear = 1;
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
            EventHandler.gameDateEvent += OnGameDateEvent;
        }

        private void OnDisable()
        {
            EventHandler.gameDateEvent -= OnGameDateEvent;
        }

        private void OnGameDateEvent(int day, int month, int year)
        {
            var dayDiff = day - previousDay;
            var monthDiff = month - previousMonth;
            var yearDiff = year - previousYear;
            previousDay = day;
            previousMonth = month;
            previousYear = year;
            
            var time = dayDiff + monthDiff * 30 + yearDiff * 360;
            characterData.currentExp += time * XiuLianSpeed;
            if(characterData.currentExp >= characterData.maxExp)
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

