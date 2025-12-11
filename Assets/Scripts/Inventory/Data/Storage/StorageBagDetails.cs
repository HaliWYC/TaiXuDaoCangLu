using System.Collections.Generic;
using UnityEngine;

namespace TXDCL.Inventory
{
    [CreateAssetMenu(fileName = "NEW StorageBagDetails", menuName = "Inventory/Storage")]
    public class StorageBagDetails : ItemDetails
    {
        public StorageBagType storageBagType;
        public int maxCapacity;
        public List<Item> items;
        public List<Property> properties;
    }

    [System.Serializable]
    public class Item
    {
        public ItemDetails itemDetails;
        public int amount;
    }
}
