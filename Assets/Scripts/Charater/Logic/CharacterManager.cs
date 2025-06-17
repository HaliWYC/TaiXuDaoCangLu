using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TXDCL.Character
{
    public class CharacterManager : Singleton<CharacterManager>
    {
        private Dictionary<string, Jingjie> JingjieDataList = new();
        [SerializeField] private TextAsset JingjieTextAsset;

        protected override void Awake()
        {
            base.Awake();
            CSVToJingjieData();
        }

        private void CSVToJingjieData()
        {
            JingjieDataList.Clear();
            //根据换行符分隔，移除空白行
            var lines = JingjieTextAsset.text.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            var data = lines.Where(line => line[0] != '#').ToList();
            for (var i = 1; i < data.Count; i++)
            {
                //根据逗号分隔，移除空白字段
                var value = data[i].Split(',', StringSplitOptions.RemoveEmptyEntries);
                Enum.TryParse(value[0], out JingjieLevel jingjieLevel);
                Enum.TryParse(value[1], out MiniJingjieLevel miniJingjieLevel);
                var key = miniJingjieLevel + jingjieLevel.ToString();
                var jingjieData = JingjieDataList.TryGetValue(key, out var JingJie)
                    ? JingJie.JingjieData
                    : ScriptableObject.CreateInstance<JingjieData>();
                jingjieData.NextEXP = int.Parse(value[2].Trim());
                jingjieData.MaxAge = int.Parse(value[3].Trim());
                jingjieData.MaxHealth = int.Parse(value[4].Trim());
                jingjieData.MaxMana = int.Parse(value[5].Trim());
                jingjieData.Attack = int.Parse(value[6].Trim());
                jingjieData.Reaction = int.Parse(value[7].Trim());
                jingjieData.MaxMovementPerTurn = int.Parse(value[8].Trim());
                jingjieData.ShenShiStrength = int.Parse(value[9].Trim());
                jingjieData.MaxDaocangPerTurn = int.Parse(value[10].Trim());
                var jingjie = new Jingjie
                    { miniJingjieLevel = miniJingjieLevel, JingjieLevel = jingjieLevel, JingjieData = jingjieData };
                if (GetJingjie(key) != null)
                {
                    JingjieDataList[key] = jingjie;
                }
                else
                {
                    JingjieDataList.Add(key, jingjie);
                }
            }
        }

        public Jingjie GetJingjie(string key)
        {
            return JingjieDataList.GetValueOrDefault(key);
        }
    }
}