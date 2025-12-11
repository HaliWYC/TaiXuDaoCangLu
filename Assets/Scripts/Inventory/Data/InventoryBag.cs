using System.Collections.Generic;
using UnityEngine;

namespace TXDCL.Inventory
{
    [CreateAssetMenu(fileName = "New Inventory Bag", menuName = "Inventory/Inventory Bag")]
    public class InventoryBag : ScriptableObject
    {
        public int LingShiAmount;//灵石数量
        
        public int basicFaBaoCapacity;
        public List<Item> basicFaBaoList;
        public StorageBagDetails FaBaoStorageBag;
        public int basicConsumablesCapacity;
        public List<FaBaoDetails> basicConsumablesList;
        public StorageBagDetails ConsumablesStorageBag;
        public int basicQuestItemCapacity;
        public List<FaBaoDetails> basicQuestItemList;
        public StorageBagDetails QuestItemStorageBag;
        public int basicOtherItemCapacity;
        public List<FaBaoDetails> basicOtherItemList;
        public StorageBagDetails OtherItemStorageBag;
        public List<StorageBagDetails> storageBags ;
        
        public List<FaBaoDetails> wearingFaBaoList;
        public List<ItemDetails> carryOnItems;
    }
    
}
