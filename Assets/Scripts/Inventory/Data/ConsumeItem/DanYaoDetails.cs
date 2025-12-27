using System.Collections.Generic;
using TXDCL.Effect;
using TXDCL.Inventory;
using UnityEngine;

namespace TXDCL.Inventory
{
    [CreateAssetMenu(fileName = "DanYaoDetails", menuName = "Inventory/ConsumeItem/DanYao")]
    public class DanYaoDetails : ConsumablesDetails
    {
        public int DanDu;//丹毒
        public int currentEatenAmount;//当前摄入该丹药数量
        public int maximumEatenAmount;//最多可摄入该丹药数量
        public List<Property> Properties;//丹药所赋予的属性
        public List<EffectData> Effects;//丹药所赋予的Buff
    }
}
