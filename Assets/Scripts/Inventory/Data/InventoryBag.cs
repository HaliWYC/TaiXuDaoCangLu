using System;
using System.Collections.Generic;
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

        /// <summary>
        /// 开局时执行一遍，如果有保存的数据则直接读取保存的数据而不调用
        /// </summary>
        public void InitializeData()
        {
            if (basicFaBaoList.Count < basicFaBaoCapacity)
            {
                var count = basicFaBaoList.Count;
                for(var i = 0; i< basicFaBaoCapacity -count; i++)
                    basicFaBaoList.Add(new InventoryItem());
            }
            for (var i = 0; i < basicFaBaoList.Count; i++)
            {
                if (basicFaBaoList[i].itemDetails != null)
                {
                    basicFaBaoList[i] = new InventoryItem 
                    { 
                        itemDetails = Instantiate(basicFaBaoList[i].itemDetails), 
                        amount = basicFaBaoList[i].amount 
                    };
                }
            }
            if (basicConsumablesList.Count < basicConsumablesCapacity)
            {
                var count = basicConsumablesList.Count;
                for(var i = 0; i< basicConsumablesCapacity - count; i++)
                    basicConsumablesList.Add(new InventoryItem());
            }
            for (var i = 0; i < basicConsumablesList.Count; i++)
            {
                if (basicConsumablesList[i].itemDetails != null)
                {
                    basicConsumablesList[i] = new InventoryItem
                    {
                        itemDetails = Instantiate(basicConsumablesList[i].itemDetails),
                        amount = basicConsumablesList[i].amount
                    };
                }
            }
            if (basicQuestItemList.Count < basicQuestItemCapacity)
            {
                var count = basicQuestItemList.Count;
                for(var i = 0; i< basicQuestItemCapacity - count; i++)
                    basicQuestItemList.Add(new InventoryItem());
            }
            for (var i = 0; i < basicQuestItemList.Count; i++)
            {
                if (basicQuestItemList[i].itemDetails != null)
                {
                    basicQuestItemList[i] = new InventoryItem
                    {
                        itemDetails = Instantiate(basicQuestItemList[i].itemDetails),
                        amount = basicQuestItemList[i].amount
                    };
                }
            }
            if (basicOtherItemList.Count < basicOtherItemCapacity)
            {
                var count = basicOtherItemList.Count;
                for(var i = 0; i< basicOtherItemCapacity - count; i++)
                    basicOtherItemList.Add(new InventoryItem());
            }

            for (var i = 0; i < basicOtherItemList.Count; i++)
            {
                if (basicOtherItemList[i].itemDetails != null)
                {
                    basicOtherItemList[i] = new InventoryItem
                    {
                        itemDetails = Instantiate(basicOtherItemList[i].itemDetails),
                        amount = basicOtherItemList[i].amount
                    };
                }
            }
            if (storageBags.Count < storageBagsCapacity)
            {
                var count = storageBags.Count;
                for(var i = 0; i < storageBagsCapacity - count; i++)
                    storageBags.Add(null);
            }
            for (var i = 0; i < storageBags.Count; i++)
            {
                if (storageBags[i] == null) continue;
                storageBags[i] = Instantiate(storageBags[i]);
                storageBags[i].Initialize();
            }
            if (wearingFaBaoList.Count < 8)
            {
                var count = wearingFaBaoList.Count;
                for (var i = 0; i < 8 - count; i++)
                {
                    wearingFaBaoList.Add(new InventoryItem());
                }
            }
            for (var i = 0; i < wearingFaBaoList.Count; i++)
            {
                if (wearingFaBaoList[i].itemDetails != null)
                {
                    wearingFaBaoList[i] = new InventoryItem
                    {
                        itemDetails = Instantiate(wearingFaBaoList[i].itemDetails), amount = wearingFaBaoList[i].amount
                    };
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
            for (var i = 0; i < carryOnItems.Count; i++)
            {
                if (carryOnItems[i].itemDetails != null)
                {
                    carryOnItems[i] = new InventoryItem
                    {
                        itemDetails = Instantiate(carryOnItems[i].itemDetails), amount = carryOnItems[i].amount
                    };
                }
            }
            if (FaBaoStorageBag != null)
            {
                FaBaoStorageBag = Instantiate(FaBaoStorageBag);
                FaBaoStorageBag.Initialize();
            }
            if (ConsumablesStorageBag != null)
            {
                ConsumablesStorageBag = Instantiate(ConsumablesStorageBag);
                ConsumablesStorageBag.Initialize();
            }
            if (QuestItemStorageBag != null)
            {
                QuestItemStorageBag = Instantiate(QuestItemStorageBag);
                QuestItemStorageBag.Initialize();
            }
            if (OtherItemStorageBag != null)
            {
                OtherItemStorageBag = Instantiate(OtherItemStorageBag);
                OtherItemStorageBag.Initialize();
            }
        }
    }
}
