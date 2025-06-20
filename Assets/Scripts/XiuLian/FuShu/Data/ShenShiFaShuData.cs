using UnityEngine;
namespace TXDCL.XiuLian.FuShu
{
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
}
