using UnityEngine;

namespace TXDCL.XiuLian.FuShu
{
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
        public ShuXing ShuXing ;//法术属性
        //TODO：触发退化和进化条件需要写成泛型
        public NormalFaShuData LowerNormalFaShuData;//退化法术，如受到致命伤或神通影响导致法术降级
        public NormalFaShuData UpperNormalFaShuData;//进化法术，如集齐残页或获得大机缘导致法术进化
    }
}
