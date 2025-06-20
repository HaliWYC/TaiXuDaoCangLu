using UnityEngine;

namespace TXDCL.XiuLian.FuShu
{
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
        public ShuXing ShuXing;//法术属性
        //TODO:增加施法条件
        //TODO：触发退化和进化条件需要写成泛型
        public ShenTongFaShuData LowerShenTongData;//退化法术，如受到致命伤或神通影响导致法术降级
        public ShenTongFaShuData UpperShenTongData;//进化法术，如集齐残页或获得大机缘导致法术进化
    }
}

