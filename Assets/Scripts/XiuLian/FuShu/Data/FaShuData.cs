using System.Collections.Generic;
using UnityEngine;

namespace TXDCL.XiuLian.FuShu
{
    [CreateAssetMenu(fileName = "FaShuData", menuName = "XiuLian/FaShu/FaShuData")]
    public class FaShuData : ScriptableObject
    {
        [Header("Basic Data")]
        public string Name;
        public Sprite FaShuIcon;
        public MiniQuality MiniQuality;
        public QualityType QualityType;
        [TextArea]
        public string Description;

        [Header("Combat")] 
        public int currentPrepareTurns;//当前准备回合
        public int PrepareTurns;//需要准备回合
        public int Range;//（如果为范围法术，则有距离）以周身一格为1
        public int ManaCost;//法力消耗
        public int DurationTime;//（如果非一次性法术，则为持续时间）以日为单位
        public FaShuType FaShuType;
        public FaShuPurposeType PurposeType;
        public FaShuTargetType TargetType;
        public FaShuTargetRangeType TargetRangeType;
        public FaShuDurationType DurationType;
        public int NormalValue;//法术自身强度
        public float AdditionalValue;//施法者强度
        //TODO:一种法术可以同时对多个数值进行更改，如减少气血同时减少神识，需要多一个List
        //public List<FaShuData> FaShuList = new List<FaShuData>();
    }
}

