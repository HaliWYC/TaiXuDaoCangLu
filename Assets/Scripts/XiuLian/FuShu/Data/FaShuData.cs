using System.Collections.Generic;
using TXDCL.Effect;
using UnityEngine;

namespace TXDCL.XiuLian.FuShu
{
    [CreateAssetMenu(fileName = "FaShuData", menuName = "XiuLian/FaShu/FaShuData")]
    public class FaShuData : ScriptableObject
    {
        [Header("Basic Data")]
        public string Name;
        public int ID;
        public Sprite FaShuIcon;
        public MiniRarity MiniRarity;
        public Rarity Rarity;
        public FaShuLevel FaShuLevel;//法术层数
        public FaShuProficiency FaShuProficiency;//法术熟练度
        [TextArea]
        public string Description;
        [Header("Type")]
        //普通法术，一般消耗法力和五行道藏释放
        //神识法术，一般消耗法力和精神力释放
        //秘术，一般消耗大量法力、气血、五行道藏等资源释放，通常有无与伦比的效果
        //神通是极强的法术，一般伴随着大量消耗和特殊释放条件
        public FaShuType FaShuType;//法术类型，如常规法术、神识法术、武功、密术、神通等
        public FaShuTarget FaShuTarget;//法术目标，如自身、敌人、友军,任意等
        public FaShuDuration FaShuDuration;//法术持续，如单次、持续(可在下个回合主动停止，否则再次消耗道藏)等
        public ShuXing ShuXing;//法术属性，如五行、魔道等
        
        [Header("Combat")] 
        public int currentPrepareTurns;//当前准备回合
        public int MaxPrepareTurns;//需要准备回合
        public int CurrentCoolDownTime;
        public int MaxCoolDownTime;
        public int ReleaseRange;//施法范围，以周身一格为1
        public int Range;//法术覆盖范围，以周身一格为1
        public int HealthCost;//气血消耗
        public int StaminaCost;//体力消耗
        public int ManaCost;//法力消耗
        public int JingShenLiCost;//精神力消耗
        public List<WuxingDaoCang> DaoCangCosts;//道藏消耗
        public int SameCost;//相同道藏消耗
        public List<EffectData> BasicEffectDatas;//法术基础效果
        public List<EffectData> PromotionEffectDatas;//法术使用道藏为相生时额外附带效果
        public List<EffectData> CounterEffectDatas;//法术使用道藏为相克时额外附带效果
        //public List<FaShuData> FaShuList = new List<FaShuData>();
        //TODO:增加施法条件
        //TODO：触发退化和进化条件需要写成泛型
        public FaShuData LowerShenTongData;//退化法术，如受到致命伤或神通影响导致法术降级
        public FaShuData UpperShenTongData;//进化法术，如集齐残页或获得大机缘导致法术进化
    }
}

