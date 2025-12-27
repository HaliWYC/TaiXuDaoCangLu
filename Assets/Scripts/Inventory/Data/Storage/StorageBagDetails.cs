using System.Collections.Generic;
using UnityEngine;

namespace TXDCL.Inventory
{
    [CreateAssetMenu(fileName = "NEW StorageBagDetails", menuName = "Inventory/Storage")]
    public class StorageBagDetails : ItemDetails
    {
        public StorageBagType storageBagType;
        public int maxCapacity;
        public List<InventoryItem> items;
        public List<Property> properties;

        public void Initialize()
        {
            if (items.Count >= maxCapacity) return;
            var count = items.Count;
            for(var i = 0; i< maxCapacity - count; i++)
                items.Add(new InventoryItem());
        }
    }
}
