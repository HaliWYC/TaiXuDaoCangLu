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

    [CreateAssetMenu(fileName = "FaShuData", menuName = "XiuLian/FaShu/NormalFaShuData")]
    public class NormalFaShuData : FaShuData
    {
        //普通法术，一般消耗法力和五行道藏释放
        //法术强度与法术自身强度和施法者攻击有关
        [Header("Combat")]
        public int MetalCost;//锐金道藏消耗
        public int WoodCost;//灵木道藏消耗
        public int WaterCost;//弱水道藏消耗
        public int FireCost;//离火道藏消耗
        public int EarthCost;//厚土道藏消耗
        public int SameCost;//相同道藏消耗
        public int DifCost;//不同道藏消耗
        public TianGanWuXing TianGanWuXing;//法术属性
        //TODO：触发退化和进化条件需要写成泛型
        public NormalFaShuData LowerNormalFaShuData;//退化法术，如受到致命伤或神通影响导致法术降级
        public NormalFaShuData UpperNormalFaShuData;//进化法术，如集齐残页或获得大机缘导致法术进化
    }
    [CreateAssetMenu(fileName = "FaShuData", menuName = "XiuLian/FaShu/ShenShiFaShuData")]
    public class ShenShiFaShuData : FaShuData
    {
        //神识法术，一般消耗法力和精神力释放
        //法术强度与法术自身强度和施法者神识大小有关
        [Header("Combat")]
        public int JingShenLiCost;//精神力消耗
        //TODO：触发退化和进化条件需要写成泛型
        public ShenShiFaShuData LowerShenShiFaShuData;//退化法术，如受到致命伤或神通影响导致法术降级
        public ShenShiFaShuData UpperShenShiFaShuData;//进化法术，如集齐残页或获得大机缘导致法术进化
    }
    
    [CreateAssetMenu(fileName = "FaShuData", menuName = "XiuLian/FaShu/MiShuData")]
    public class MiShuFaShuData : FaShuData
    {
        //秘术，一般消耗大量法力、气血、五行道藏等资源释放，通常有无与伦比的效果
        //秘术强度与法术强度和施法者攻击有关
        [Header("Combat")] 
        public int HealthCost;//气血消耗
        public int MetalCost;//锐金道藏消耗
        public int WoodCost;//灵木道藏消耗
        public int WaterCost;//弱水道藏消耗
        public int FireCost;//离火道藏消耗
        public int EarthCost;//厚土道藏消耗
        public int SameCost;//相同道藏消耗
        public int DifCost;//不同道藏消耗
        public int JingShenLiCost;//精神力消耗
        public TianGanWuXing TianGanWuXing;//法术属性
        //TODO：触发退化和进化条件需要写成泛型
        public MiShuFaShuData LowerMiShuData;//退化法术，如受到致命伤或神通影响导致法术降级
        public MiShuFaShuData UpperMiShuData;//进化法术，如集齐残页或获得大机缘导致法术进化
    }
    
    [CreateAssetMenu(fileName = "FaShuData", menuName = "XiuLian/FaShu/ShenTongData")]
    public class ShenTongFaShuData : FaShuData
    {
        //神通是极强的法术，一般伴随着大量消耗和特殊释放条件
        //神通强度跟多个因素有关
        [Header("Combat")] 
        public int HealthCost;//气血消耗
        public int MetalCost;//锐金道藏消耗
        public int WoodCost;//灵木道藏消耗
        public int WaterCost;//弱水道藏消耗
        public int FireCost;//离火道藏消耗
        public int EarthCost;//厚土道藏消耗
        public int SameCost;//相同道藏消耗
        public int DifCost;//不同道藏消耗
        public int JingShenLiCost;//精神力消耗
        public TianGanWuXing TianGanWuXing;//法术属性
        //TODO:增加施法条件
        //TODO：触发退化和进化条件需要写成泛型
        public ShenTongFaShuData LowerShenTongData;//退化法术，如受到致命伤或神通影响导致法术降级
        public ShenTongFaShuData UpperShenTongData;//进化法术，如集齐残页或获得大机缘导致法术进化

    }
    
}

