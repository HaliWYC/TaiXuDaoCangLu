using UnityEngine;

namespace TXDCL.XiuLian.FuShu
{
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
        public ShuXing ShuXing;//法术属性
        //TODO：触发退化和进化条件需要写成泛型
        public MiShuFaShuData LowerMiShuData;//退化法术，如受到致命伤或神通影响导致法术降级
        public MiShuFaShuData UpperMiShuData;//进化法术，如集齐残页或获得大机缘导致法术进化
    }
}
