using UnityEngine;

namespace TXDCL.XiuLian.ShenTong
{
    [CreateAssetMenu(fileName = "ShenTongData", menuName = "XiuLian/ShenTong")]
    public class ShenTongData : ScriptableObject
    {
        [Header("Basic Data")]
        public string Name;
        public Sprite Icon;
        [TextArea]
        public string Description;

        public int DurationTime;//(如果非一次性或永久神通，则有持续时间)以日为单位
        public ShenTongDurationType DurationType;
        
        //TODO：神通进化和退化条件
        public ShenTongData LowerShenTongData;//退化神通,如受到致命伤或神通影响导致神通降级
        public ShenTongData UpperShenTongData;//进化神通,如集齐残页或获得大机缘导致神通进化
    }
}

