using System.Collections.Generic;
using TXDCL.Effect;
using UnityEngine;

namespace TXDCL.Inventory
{
    [CreateAssetMenu(fileName = "FaBaoDetails", menuName = "Inventory/FaBaoDetails")]
    public class FaBaoDetails : ItemDetails
    {
        public FaBaoType faBaoType;
        public float currentEndurance;
        public float maxEndurance;
        public bool ConstantEndurance;
        public List<Property> properties;
        public List<EffectData> effects;
    }
}

