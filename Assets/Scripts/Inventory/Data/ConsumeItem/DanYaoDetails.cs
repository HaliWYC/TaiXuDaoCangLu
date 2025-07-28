using System.Collections.Generic;
using TXDCL.Effect;
using TXDCL.Inventory;
using UnityEngine;

namespace TXDCL.Inventory
{
    [CreateAssetMenu(fileName = "DanYaoDetails", menuName = "Inventory/ConsumeItem/DanYao")]
    public class DanYaoDetails : ConsumeItemDetails
    {
        public List<Property> Properties;
        public List<EffectData> Effects;
    }
}
