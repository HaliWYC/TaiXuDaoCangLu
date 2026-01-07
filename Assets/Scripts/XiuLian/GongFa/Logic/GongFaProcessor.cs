using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TXDCL.Character;
using Unity.Mathematics;

namespace TXDCL.XiuLian.GongFa
{
    public class GongFaProcessor : MonoBehaviour
    {
        private CharacterData characterData;
        private CharacterData gongFaData;
        public List<GongFaData> MainGongFas = new();
        public List<GongFaData> SubGongFas = new();
        public int XiuLianSpeed;
        [Header("Time")] 
        private int previousMonth;
        private int previousYear;

        private bool isReachLimit;
        private void Awake()
        {
            //TODO:后续根据数据保存系统储存时间
            previousMonth = 1;
            previousYear = 1;
            isReachLimit = false;
        }

        public void InitializeGongFa(CharacterData CharacterData, CharacterData GongFaData)
        {
            characterData = CharacterData;
            gongFaData = GongFaData;
            UpdateProperty();
        }
        public void UpdateProperty()
        {
            if(characterData == null || gongFaData == null) return;
            gongFaData.ResetProperty();
            gongFaData.MainGongFaBasicSpeed += MainGongFas.Sum(GongFa => GongFa.BasicXiuLianSpeed);
            gongFaData.SubGongFaBasicSpeed += SubGongFas.Sum(GongFa => GongFa.BasicXiuLianSpeed);
            gongFaData.MainGongFaAdditionalSpeed += MainGongFas.Sum(GongFa => GongFa.AdditionalXiuLianSpeed);
            foreach (var property in MainGongFas.SelectMany(MainGF => MainGF.PropertyList))
            {
                gongFaData.AddProperty(property);
            }
            foreach (var property in SubGongFas.SelectMany(SubGongFa => SubGongFa.PropertyList))
            {
                gongFaData.AddProperty(property);
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
            if (characterData.currentExp >= characterData.nextExp && characterData.nextExp != 0)
                GetComponent<CharacterBase>().CheckLevelUp();
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
                GetComponent<CharacterBase>().CheckLevelUp();
        }
    }
}

