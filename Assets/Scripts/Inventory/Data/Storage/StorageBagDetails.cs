using System.Collections.Generic;
using UnityEngine;

namespace TXDCL.Inventory
{
    [CreateAssetMenu(fileName = "NEW StorageBagDetails", menuName = "Inventory/Storage")]
    [System.Serializable]
    public class StorageBagDetails : ItemDetails
    {
        public StorageBagType storageBagType;
        public int maxCapacity;
        public List<InventoryItem> items;
        public List<Property> properties;
        
        public override void InitializeData()
        {
            base.InitializeData();
            if (items.Count >= maxCapacity) return;
            var count = items.Count;
            for(var i = 0; i< maxCapacity - count; i++)
                items.Add(new InventoryItem());
            for (var i = 0; i < count; i++)
            {
                if (items[i].itemDetails != null)
                {
                    items[i] = new InventoryItem { itemDetails = Instantiate(items[i].itemDetails), itemAmount = items[i].itemAmount };
                }
            }
        }
    }
}
