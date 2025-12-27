using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TXDCL.Inventory
{
    [CreateAssetMenu(fileName = "New Inventory Bag", menuName = "Inventory/Inventory Bag")]
    public class InventoryBag : ScriptableObject
    {
        public int LingShiAmount;//灵石数量
        
        [Header("StorageBag")]
        [Range(0,10)]
        public int storageBagsCapacity;
        public List<StorageBagDetails> storageBags ;
        
        [Header("ItemBag")]
        public int basicFaBaoCapacity;
        public List<InventoryItem> basicFaBaoList;
        public StorageBagDetails FaBaoStorageBag;
        public int basicConsumablesCapacity;
        public List<InventoryItem> basicConsumablesList;
        public StorageBagDetails ConsumablesStorageBag;
        public int basicQuestItemCapacity;
        public List<InventoryItem> basicQuestItemList;
        public StorageBagDetails QuestItemStorageBag;
        public int basicOtherItemCapacity;
        public List<InventoryItem> basicOtherItemList;
        public StorageBagDetails OtherItemStorageBag;
        
        [Header("FaBaoBag")]
        public List<InventoryItem> wearingFaBaoList;
        public List<InventoryItem> carryOnItems;

        public void InitializeData()
        {
            if (basicFaBaoList.Count < basicFaBaoCapacity)
            {
                var count = basicFaBaoList.Count;
                for(var i = 0; i< basicFaBaoCapacity -count; i++)
                    basicFaBaoList.Add(new InventoryItem());
            }
            if (basicConsumablesList.Count < basicConsumablesCapacity)
            {
                var count = basicConsumablesList.Count;
                for(var i = 0; i< basicConsumablesCapacity - count; i++)
                    basicConsumablesList.Add(new InventoryItem());
            }
            if (basicQuestItemList.Count < basicQuestItemCapacity)
            {
                var count = basicQuestItemList.Count;
                for(var i = 0; i< basicQuestItemCapacity - count; i++)
                    basicQuestItemList.Add(new InventoryItem());
            }
            if (basicOtherItemList.Count < basicOtherItemCapacity)
            {
                var count = basicOtherItemList.Count;
                for(var i = 0; i< basicOtherItemCapacity - count; i++)
                    basicOtherItemList.Add(new InventoryItem());
            }
            if (storageBags.Count < storageBagsCapacity)
            {
                var count = storageBags.Count;
                for(var i = 0; i < storageBagsCapacity - count; i++)
                    storageBags.Add(null);
            }
            if (wearingFaBaoList.Count < 8)
            {
                var count = wearingFaBaoList.Count;
                for (var i = 0; i < 8 - count; i++)
                {
                    wearingFaBaoList.Add(new InventoryItem());
                }
            }
            if (carryOnItems.Count < 6)
            {
                var count = carryOnItems.Count;
                for (var i = 0; i < 6 - count; i++)
                {
                    carryOnItems.Add(new InventoryItem());
                }
            }
            if (FaBaoStorageBag != null) FaBaoStorageBag.Initialize();
            if (ConsumablesStorageBag != null) ConsumablesStorageBag.Initialize();
            if (QuestItemStorageBag != null) QuestItemStorageBag.Initialize();
            if (OtherItemStorageBag != null) OtherItemStorageBag.Initialize();
            foreach (var bag in storageBags.Where(bag => bag != null))
            {
                bag.Initialize();
            }
        }
    }
}
