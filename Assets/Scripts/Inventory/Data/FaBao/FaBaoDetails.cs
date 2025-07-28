using System.Collections.Generic;
using TXDCL.Effect;
using UnityEngine;

namespace TXDCL.Inventory
{
    [CreateAssetMenu(fileName = "FaBaoDetails", menuName = "Inventory/FaBaoDetails")]
    public class FaBaoDetails : ItemDetails
    {
        public FaBaoType faBaoType;
        public List<Property> properties;
        public List<EffectData> effects;
    }
}

