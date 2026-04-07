using System;
using System.Linq;
using TXDCL.Combat;
using UnityEngine;

namespace TXDCL.Inventory
{
    public class InventoryManager : Singleton<InventoryManager>
    {
        public ItemDetailList itemDetailList;
        
        public Color SpecialRarityColor;
        public Color TianRarityColor;
        public Color DiRariryColor;
        public Color RenRarityColor;
        public Color FanRarityColor;
        public Color fragmentaryRarityColor;
        
        protected override void Awake()
        {
            base.Awake();
            InitializedItemDetailList();
        }

        /// <summary>
        /// 初始化物品信息列表
        /// </summary>
        private void InitializedItemDetailList()
        {
            for (var i = 1; i < itemDetailList.ItemList.Count + 1; i++)
            {
                itemDetailList.ItemList[i - 1].ID = i;
            }
        }
        /// <summary>
        /// 根据物品的品质返回相对应的颜色
        /// </summary>
        /// <param name="rarity"></param>
        /// <returns></returns>
        public Color GetItemColorByRarity(Rarity rarity)
        {
            return rarity switch
            {
                Rarity.残缺 => fragmentaryRarityColor,
                Rarity.凡级 => FanRarityColor,
                Rarity.人级 => RenRarityColor,
                Rarity.地级 => DiRariryColor,
                Rarity.天级 => TianRarityColor,
                Rarity.特殊 => SpecialRarityColor,
                _ => Color.white
            };
        }
        /// <summary>
        /// 获得物品的克隆体
        /// </summary>
        /// <param name="itemID"></param>
        /// <returns></returns>
        public ItemDetails GetItemDetail(int itemID)
        {
            return itemID != 0 ? Instantiate(itemDetailList.ItemList[itemID - 1]) : null;
        }
        /// <summary>
        /// 交换两个格子中的物品
        /// </summary>
        /// <param name="currentSlot"></param>
        /// <param name="targetSlot"></param>
        public void SwapItemSlots(ItemSlotUI currentSlot, ItemSlotUI targetSlot)
        {
            //判断是否处于战斗状态，战斗状态下无法更改装备和携带栏的物品
            if (CombatManager.Instance.isCombating && (currentSlot.isCarriedOnItemSlot || targetSlot.isCarriedOnItemSlot || currentSlot.isWearingFaBaoSlot || targetSlot.isWearingFaBaoSlot)) return;
            //判断是否为当前格子是否为空或者是否与目标格子相同
            if(currentSlot == null || currentSlot == targetSlot || targetSlot == null) return;
            if (currentSlot.availableItemType == targetSlot.availableItemType)
            {
                SwapItemsInBag(currentSlot,targetSlot);
                SwapItemsInBag(targetSlot,currentSlot);
            }
            else 
            {
                if (currentSlot.availableItemType == ItemSlotAvailableType.万能)
                {
                    if (ItemSlotTypeMatchItemType(targetSlot.availableItemType) == currentSlot.itemDetails.itemType)
                    {
                        SwapItemsInBag(currentSlot,targetSlot);
                        SwapItemsInBag(targetSlot,currentSlot);
                    }
                }
                else if (targetSlot.availableItemType == ItemSlotAvailableType.万能)
                {
                    if (targetSlot.itemDetails == null)
                    {
                        if (currentSlot.itemDetails.itemType != ItemType.储物袋)
                        {
                            SwapItemsInBag(currentSlot,targetSlot);
                            SwapItemsInBag(targetSlot,currentSlot);
                        }
                    }
                    else if (ItemSlotTypeMatchItemType(currentSlot.availableItemType) == targetSlot.itemDetails.itemType)
                    {
                        SwapItemsInBag(currentSlot,targetSlot);
                        SwapItemsInBag(targetSlot,currentSlot);
                    }
                }
            }
            EventHandler.CallUpdateInventoryUIEvent(InventoryUI.Instance.currentCharacter);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentSlot"></param>
        /// <param name="targetSlot"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private void SwapItemsInBag(ItemSlotUI currentSlot, ItemSlotUI targetSlot)
        {
            var inventoryBag = InventoryUI.Instance.inventoryBag;
            var currentIndex = 0;
            if (currentIndex == targetSlot.SlotIndex)
            {
                switch (InventoryUI.Instance.storageBagDropdown.value)
                {
                    case 0:
                        if (currentSlot.itemDetails != null && (currentSlot.itemDetails as StorageBagDetails).storageBagType != StorageBagType.法宝) return;
                        inventoryBag.FaBaoStorageBag = currentSlot.itemDetails != null ? currentSlot.itemDetails as StorageBagDetails : null;
                        break;
                    case 1:
                        if (currentSlot.itemDetails != null && (currentSlot.itemDetails as StorageBagDetails).storageBagType != StorageBagType.消耗品) return;
                        inventoryBag.ConsumablesStorageBag = currentSlot.itemDetails != null ? currentSlot.itemDetails as StorageBagDetails : null;
                        break;
                    case 2:
                        if (currentSlot.itemDetails != null && (currentSlot.itemDetails as StorageBagDetails).storageBagType != StorageBagType.任务物品) return;
                        inventoryBag.QuestItemStorageBag = currentSlot.itemDetails != null ? currentSlot.itemDetails as StorageBagDetails : null;
                        break;
                    case 3:
                        if (currentSlot.itemDetails != null && (currentSlot.itemDetails as StorageBagDetails).storageBagType != StorageBagType.其他物品) return;
                        inventoryBag.OtherItemStorageBag = currentSlot.itemDetails != null ? currentSlot.itemDetails as StorageBagDetails : null;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                return;
            }
            currentIndex++;
            for (var i = 0; i < inventoryBag.storageBags.Count; i++)
            {
                if (currentIndex == targetSlot.SlotIndex)
                {
                    if (currentSlot == InventoryUI.Instance.currentStorageBagUI)
                    {
                        switch (InventoryUI.Instance.storageBagDropdown.value)
                        {
                            case 0:
                                if (targetSlot.itemDetails != null && (targetSlot.itemDetails as StorageBagDetails).storageBagType != StorageBagType.法宝) return;
                                break;
                            case 1:
                                if (targetSlot.itemDetails != null && (targetSlot.itemDetails as StorageBagDetails).storageBagType != StorageBagType.消耗品) return;
                                break;
                            case 2:
                                if (targetSlot.itemDetails != null && (targetSlot.itemDetails as StorageBagDetails).storageBagType != StorageBagType.任务物品) return;
                                break;
                            case 3:
                                if (targetSlot.itemDetails != null && (targetSlot.itemDetails as StorageBagDetails).storageBagType != StorageBagType.其他物品) return;
                                break;
                        }
                    }
                    inventoryBag.storageBags[i] = currentSlot.itemDetails != null ? currentSlot.itemDetails as StorageBagDetails : null;
                    return;
                }
                currentIndex++;
            }
            for (var i = 0; i < inventoryBag.wearingFaBaoList.Count; i++)
            {
                if (currentIndex == targetSlot.SlotIndex)
                {
                    inventoryBag.wearingFaBaoList[i] = new InventoryItem { itemDetails = currentSlot.itemDetails, itemAmount = currentSlot.itemAmount};
                    InventoryUI.Instance.currentCharacter.UpdateCharacterData();
                    return;
                }
                currentIndex++;
            }
            for (var i = 0; i < inventoryBag.carryOnItems.Count; i++)
            {
                if (currentIndex == targetSlot.SlotIndex)
                {
                    inventoryBag.carryOnItems[i] = new InventoryItem { itemDetails = currentSlot.itemDetails, itemAmount = currentSlot.itemAmount};
                    InventoryUI.Instance.currentCharacter.UpdateCharacterData();
                    return;
                }
                currentIndex++;
            }
            switch (targetSlot.availableItemType)
            {
                case ItemSlotAvailableType.法宝:
                    for (var i = 0; i < inventoryBag.basicFaBaoList.Count; i++)
                    {
                        if (currentIndex == targetSlot.SlotIndex)
                        {
                            inventoryBag.basicFaBaoList[i] = new InventoryItem{ itemDetails = currentSlot.itemDetails, itemAmount = currentSlot.itemAmount};
                            return;
                        }
                        currentIndex++;
                    }
                    if (inventoryBag.FaBaoStorageBag != null)
                    {
                        for (var i = 0; i < inventoryBag.FaBaoStorageBag.items.Count; i++)
                        {
                            if (currentIndex == targetSlot.SlotIndex)
                            {
                                inventoryBag.FaBaoStorageBag.items[i] = new InventoryItem { itemDetails = currentSlot.itemDetails, itemAmount = currentSlot.itemAmount};
                                return;
                            }
                            currentIndex++;
                        }
                    }
                    break;
                case ItemSlotAvailableType.消耗品:
                    for (var i = 0; i < inventoryBag.basicConsumablesList.Count; i++)
                    {
                        if (currentIndex == targetSlot.SlotIndex)
                        { 
                            inventoryBag.basicConsumablesList[i] = new InventoryItem { itemDetails = currentSlot.itemDetails, itemAmount = currentSlot.itemAmount};
                            return;
                        }
                        currentIndex++;
                    }
            
                    if (inventoryBag.ConsumablesStorageBag != null)
                    {
                        for (var i = 0; i < inventoryBag.ConsumablesStorageBag.items.Count; i++)
                        {
                            if (currentIndex == targetSlot.SlotIndex)
                            {
                                inventoryBag.ConsumablesStorageBag.items[i] = new InventoryItem { itemDetails = currentSlot.itemDetails, itemAmount = currentSlot.itemAmount };
                                return;
                            }
                            currentIndex++;
                        }
                    }
                    break;
                case ItemSlotAvailableType.任务物品:
                    for (var i = 0; i < inventoryBag.basicQuestItemList.Count; i++)
                    {
                        if (currentIndex == targetSlot.SlotIndex)
                        { 
                            inventoryBag.basicQuestItemList[i] = new InventoryItem { itemDetails = currentSlot.itemDetails, itemAmount = currentSlot.itemAmount};
                            return;
                        }
                        currentIndex++;
                    }
            
                    if (inventoryBag.QuestItemStorageBag != null)
                    {
                        for (var i = 0; i < inventoryBag.QuestItemStorageBag.items.Count; i++)
                        {
                            if (currentIndex == targetSlot.SlotIndex)
                            {
                                inventoryBag.QuestItemStorageBag.items[i] = new InventoryItem { itemDetails = currentSlot.itemDetails, itemAmount = currentSlot.itemAmount};
                                return;
                            }
                            currentIndex++;
                        }
                    }
                    break;
                case ItemSlotAvailableType.其他物品:
                    for (var i = 0; i < inventoryBag.basicOtherItemList.Count; i++)
                    {
                        if (currentIndex == targetSlot.SlotIndex)
                        { 
                            inventoryBag.basicOtherItemList[i] = new InventoryItem { itemDetails = currentSlot.itemDetails, itemAmount = currentSlot.itemAmount};
                            return;
                        }
                        currentIndex++;
                    }
            
                    if (inventoryBag.OtherItemStorageBag != null)
                    {
                        for (var i = 0; i < inventoryBag.OtherItemStorageBag.items.Count; i++)
                        {
                            if (currentIndex == targetSlot.SlotIndex)
                            {
                                inventoryBag.OtherItemStorageBag.items[i] = new InventoryItem { itemDetails = currentSlot.itemDetails, itemAmount = currentSlot.itemAmount};
                                return;
                            }
                            currentIndex++;
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// 添加物品
        /// </summary>
        /// <param name="inventoryBag"></param>
        /// <param name="item"></param>
        public void AddItem(InventoryBag inventoryBag, InventoryItem item, out bool Success)
        {
            Success = false;
            if (inventoryBag == null || item.itemDetails == null || item.itemAmount == 0) return;
            //判断是否可堆叠，若可堆叠则优先堆叠
            if (item.itemDetails.canStack)
            {
                //若可堆叠则分别判断所有符合的目标是否达到可堆叠最大值
                AddExistedItem(inventoryBag, item.itemDetails, item.itemAmount, out var success);
                Success = success;
            }
            if (item.itemDetails.canStack && Success) return;
            //判断背包是否满了
            if (CheckBagCapacity(inventoryBag, item.itemDetails))
            {
                Debug.Log("Bag is full");
                Success = false;
            }
            else
            {
                //若没有满则添加一个新的格子
                AddNewItem(inventoryBag, item.itemDetails, item.itemAmount);
                Success = true;
            }
        }
        
        /// <summary>
        /// 添加一个在背包中存在的且为达到堆叠上限的物品
        /// </summary>
        /// <param name="inventoryBag"></param>
        /// <param name="itemDetails"></param>
        /// <param name="ItemAmount"></param>
        /// <param name="Success"></param>
        private void AddExistedItem(InventoryBag inventoryBag, ItemDetails itemDetails, int ItemAmount , out bool Success)
        {
            Success = false;
            if (inventoryBag == null || itemDetails == null || ItemAmount == 0) return;
            //先根据物品类型检索第一个符合的物品，若物品数量为超过可堆叠上限则叠加，否则继续寻找下一个符合目标
            switch (itemDetails.itemType)
            {
                case ItemType.法宝:
                    for (var i = 0; i < inventoryBag.basicFaBaoList.Count; i++)
                    {
                        if (inventoryBag.basicFaBaoList[i].itemDetails != itemDetails) continue;
                        if (inventoryBag.basicFaBaoList[i].itemAmount + ItemAmount > itemDetails.stackSize) continue;
                        inventoryBag.basicFaBaoList[i] = new InventoryItem
                        {
                            itemDetails = itemDetails,
                            itemAmount = inventoryBag.basicFaBaoList[i].itemAmount + ItemAmount
                        };
                        Success = true;
                        return;
                    }
                    if (inventoryBag.FaBaoStorageBag != null)
                    {
                        for (var i = 0; i < inventoryBag.FaBaoStorageBag.items.Count; i++)
                        {
                            if (inventoryBag.FaBaoStorageBag.items[i].itemDetails != itemDetails) continue;
                            if (inventoryBag.FaBaoStorageBag.items[i].itemAmount + ItemAmount > itemDetails.stackSize) continue;
                            inventoryBag.FaBaoStorageBag.items[i] = new InventoryItem
                            {
                                itemDetails = itemDetails,
                                itemAmount = inventoryBag.FaBaoStorageBag.items[i].itemAmount + ItemAmount
                            };
                            Success = true;
                            return;
                        }
                    }
                    break;
                case ItemType.消耗品:
                    for (var i = 0; i < inventoryBag.basicConsumablesList.Count; i++)
                    {
                        if (inventoryBag.basicConsumablesList[i].itemDetails != itemDetails) continue;
                        if (inventoryBag.basicConsumablesList[i].itemAmount + ItemAmount > itemDetails.stackSize) continue;
                        inventoryBag.basicConsumablesList[i] = new InventoryItem
                        {
                            itemDetails = itemDetails,
                            itemAmount = inventoryBag.basicConsumablesList[i].itemAmount + ItemAmount
                        };
                        Success = true;
                        return;
                    }
                    if (inventoryBag.ConsumablesStorageBag != null)
                    {
                        for (var i = 0; i < inventoryBag.ConsumablesStorageBag.items.Count; i++)
                        {
                            if (inventoryBag.ConsumablesStorageBag.items[i].itemDetails != itemDetails) continue;
                            if (inventoryBag.ConsumablesStorageBag.items[i].itemAmount + ItemAmount > itemDetails.stackSize) continue;
                            inventoryBag.ConsumablesStorageBag.items[i] = new InventoryItem
                            {
                                itemDetails = itemDetails,
                                itemAmount = inventoryBag.ConsumablesStorageBag.items[i].itemAmount + ItemAmount
                            };
                            Success = true;
                            return;
                        }
                    }
                    break;
                case ItemType.任务物品:
                    for (var i = 0; i < inventoryBag.basicQuestItemList.Count; i++)
                    {
                        if (inventoryBag.basicQuestItemList[i].itemDetails != itemDetails) continue;
                        if (inventoryBag.basicQuestItemList[i].itemAmount + ItemAmount > itemDetails.stackSize) continue;
                        inventoryBag.basicQuestItemList[i] = new InventoryItem
                        {
                            itemDetails = itemDetails,
                            itemAmount = inventoryBag.basicQuestItemList[i].itemAmount + ItemAmount
                        };
                        Success = true;
                        return;
                    }
                    if (inventoryBag.QuestItemStorageBag != null)
                    {
                        for (var i = 0; i < inventoryBag.QuestItemStorageBag.items.Count; i++)
                        {
                            if (inventoryBag.QuestItemStorageBag.items[i].itemDetails != itemDetails) continue;
                            if (inventoryBag.QuestItemStorageBag.items[i].itemAmount + ItemAmount > itemDetails.stackSize) continue;
                            inventoryBag.QuestItemStorageBag.items[i] = new InventoryItem
                            {
                                itemDetails = itemDetails,
                                itemAmount = inventoryBag.QuestItemStorageBag.items[i].itemAmount + ItemAmount
                            };
                            Success = true;
                            return;
                        }
                    }
                    break;
                case ItemType.其他物品:
                    for (var i = 0; i < inventoryBag.basicOtherItemList.Count; i++)
                    {
                        if (inventoryBag.basicOtherItemList[i].itemDetails != itemDetails) continue;
                        if (inventoryBag.basicOtherItemList[i].itemAmount + ItemAmount > itemDetails.stackSize) continue;
                        inventoryBag.basicOtherItemList[i] = new InventoryItem
                        {
                            itemDetails = itemDetails,
                            itemAmount = inventoryBag.basicOtherItemList[i].itemAmount + ItemAmount
                        };
                        Success = true;
                        return;
                    }
                    if (inventoryBag.OtherItemStorageBag != null)
                    {
                        for (var i = 0; i < inventoryBag.OtherItemStorageBag.items.Count; i++)
                        {
                            if (inventoryBag.OtherItemStorageBag.items[i].itemDetails != itemDetails) continue;
                            if (inventoryBag.OtherItemStorageBag.items[i].itemAmount + ItemAmount > itemDetails.stackSize) continue;
                            inventoryBag.OtherItemStorageBag.items[i] = new InventoryItem
                            {
                                itemDetails = itemDetails,
                                itemAmount = inventoryBag.OtherItemStorageBag.items[i].itemAmount + ItemAmount
                            };
                            Success = true;
                            return;
                        }
                    }
                    break;
            }
            //其次检索已有的万能储物袋是否可以添加
            foreach (var t in inventoryBag.storageBags.Where(t => t != null && (t.storageBagType == StorageBagType.万能 || StorageTypeMatchItemType(t.storageBagType) == itemDetails.itemType)))
            {
                for (var j = 0; j < t.items.Count; j++)
                {
                    if (t.items[j].itemDetails != itemDetails) continue;
                    if (t.items[j].itemAmount + ItemAmount > itemDetails.stackSize) continue;
                    t.items[j] = new InventoryItem
                    {
                        itemDetails = itemDetails,
                        itemAmount = t.items[j].itemAmount + ItemAmount
                    };
                    Success = true;
                    return;
                }
            }
        }

        private void AddNewItem(InventoryBag inventoryBag, ItemDetails itemDetails, int ItemAmount)
        {
            //优先根据物品类型分配新的格子
            switch (itemDetails.itemType)
            {
                case ItemType.法宝:
                    for (var i = 0; i < inventoryBag.basicFaBaoList.Count; i++)
                    {
                        if (inventoryBag.basicFaBaoList[i].itemDetails != null && inventoryBag.basicFaBaoList[i].itemAmount != 0) continue;
                        inventoryBag.basicFaBaoList[i] = new InventoryItem { itemDetails = itemDetails, itemAmount = ItemAmount };
                        return;
                    }
                    if (inventoryBag.FaBaoStorageBag != null)
                    {
                        for (var i = 0; i < inventoryBag.FaBaoStorageBag.items.Count; i++)
                        {
                            if (inventoryBag.FaBaoStorageBag.items[i].itemDetails != null && inventoryBag.FaBaoStorageBag.items[i].itemAmount != 0) continue;
                            inventoryBag.FaBaoStorageBag.items[i] = new InventoryItem { itemDetails = itemDetails, itemAmount = ItemAmount };
                            return;
                        }
                    }
                    break;
                case ItemType.消耗品:
                    for (var i = 0; i < inventoryBag.basicConsumablesList.Count; i++)
                    {
                        if (inventoryBag.basicConsumablesList[i].itemDetails != null && inventoryBag.basicConsumablesList[i].itemAmount != 0) continue;
                        inventoryBag.basicConsumablesList[i] = new InventoryItem { itemDetails = itemDetails, itemAmount = ItemAmount };
                        return;
                    }
                    if (inventoryBag.ConsumablesStorageBag != null)
                    {
                        for (var i = 0; i < inventoryBag.ConsumablesStorageBag.items.Count; i++)
                        {
                            if (inventoryBag.ConsumablesStorageBag.items[i].itemDetails != null && inventoryBag.ConsumablesStorageBag.items[i].itemAmount != 0) continue;
                            inventoryBag.ConsumablesStorageBag.items[i] = new InventoryItem { itemDetails = itemDetails, itemAmount = ItemAmount };
                            return;
                        }
                    }
                    break;
                case ItemType.任务物品:
                    for (var i = 0; i < inventoryBag.basicQuestItemList.Count; i++)
                    {
                        if (inventoryBag.basicQuestItemList[i].itemDetails != null && inventoryBag.basicQuestItemList[i].itemAmount != 0) continue;
                        inventoryBag.basicQuestItemList[i] = new InventoryItem { itemDetails = itemDetails, itemAmount = ItemAmount };
                        return;
                    }
                    if (inventoryBag.QuestItemStorageBag != null)
                    {
                        for (var i = 0; i < inventoryBag.QuestItemStorageBag.items.Count; i++)
                        {
                            if (inventoryBag.QuestItemStorageBag.items[i].itemDetails != null && inventoryBag.QuestItemStorageBag.items[i].itemAmount != 0) continue;
                            inventoryBag.QuestItemStorageBag.items[i] = new InventoryItem { itemDetails = itemDetails, itemAmount = ItemAmount };
                            return;
                        }
                    }
                    break;
                case ItemType.其他物品:
                    for (var i = 0; i < inventoryBag.basicOtherItemList.Count; i++)
                    {
                        if (inventoryBag.basicOtherItemList[i].itemDetails != null && inventoryBag.basicOtherItemList[i].itemAmount != 0) continue;
                        inventoryBag.basicOtherItemList[i] = new InventoryItem { itemDetails = itemDetails, itemAmount = ItemAmount };
                        return;
                    }
                    if (inventoryBag.OtherItemStorageBag != null)
                    {
                        for (var i = 0; i < inventoryBag.OtherItemStorageBag.items.Count; i++)
                        {
                            if (inventoryBag.OtherItemStorageBag.items[i].itemDetails != null && inventoryBag.OtherItemStorageBag.items[i].itemAmount != 0) continue;
                            inventoryBag.OtherItemStorageBag.items[i] = new InventoryItem { itemDetails = itemDetails, itemAmount = ItemAmount };
                            return;
                        }
                    }
                    break;
                case ItemType.储物袋:
                    for (var i = 0; i < inventoryBag.storageBags.Count; i++)
                    {
                        if (inventoryBag.storageBags[i] != null) continue;
                        inventoryBag.storageBags[i] = itemDetails as StorageBagDetails;
                        return;
                    }
                    break;
            }
            //在已有的万能储物袋中分配新的格子
            foreach (var t in inventoryBag.storageBags.Where(t => t != null && (t.storageBagType == StorageBagType.万能 || StorageTypeMatchItemType(t.storageBagType) == itemDetails.itemType)))
            {
                for (var j = 0; j < t.items.Count; j++)
                {
                    if (t.items[j].itemDetails == null || t.items[j].itemAmount == 0)
                    {
                        t.items[j] = new InventoryItem { itemDetails = itemDetails, itemAmount = ItemAmount };
                    }
                }
            }
        }

        private bool CheckBagCapacity(InventoryBag inventoryBag, ItemDetails itemDetails)
        {
            //先检查当前物品类型的基础格子和类型专属储物袋格子
            switch (itemDetails.itemType)
            {
                case ItemType.法宝:
                    for (var i = 0; i < inventoryBag.basicFaBaoList.Count; i++)
                    {
                        if (inventoryBag.basicFaBaoList[i].itemDetails == null || inventoryBag.basicFaBaoList[i].itemAmount == 0)
                        {
                            return false;
                        }
                    }
                    if (inventoryBag.FaBaoStorageBag != null)
                    {
                        for (var i = 0; i < inventoryBag.FaBaoStorageBag.items.Count; i++)
                        {
                            if (inventoryBag.FaBaoStorageBag.items[i].itemDetails == null || inventoryBag.FaBaoStorageBag.items[i].itemAmount == 0)
                            {
                                return false;
                            }
                        }
                    }
                    break;
                case ItemType.消耗品:
                    for (var i = 0; i < inventoryBag.basicConsumablesList.Count; i++)
                    {
                        if (inventoryBag.basicConsumablesList[i].itemDetails == null || inventoryBag.basicConsumablesList[i].itemAmount == 0)
                        {
                            return false;
                        }
                    }
                    if (inventoryBag.ConsumablesStorageBag != null)
                    {
                        for (var i = 0; i < inventoryBag.ConsumablesStorageBag.items.Count; i++)
                        {
                            if (inventoryBag.ConsumablesStorageBag.items[i].itemDetails == null || inventoryBag.ConsumablesStorageBag.items[i].itemAmount == 0)
                            {
                                return false;
                            }
                        }
                    }
                    break;
                case ItemType.任务物品:
                    for (var i = 0; i < inventoryBag.basicQuestItemList.Count; i++)
                    {
                        if (inventoryBag.basicQuestItemList[i].itemDetails == null || inventoryBag.basicQuestItemList[i].itemAmount == 0)
                        {
                            return false;
                        }
                    }
                    if (inventoryBag.QuestItemStorageBag != null)
                    {
                        for (var i = 0; i < inventoryBag.QuestItemStorageBag.items.Count; i++)
                        {
                            if (inventoryBag.QuestItemStorageBag.items[i].itemDetails == null || inventoryBag.QuestItemStorageBag.items[i].itemAmount == 0)
                            {
                                return false;
                            }
                        }
                    }
                    break;
                case ItemType.其他物品:
                    for (var i = 0; i < inventoryBag.basicOtherItemList.Count; i++)
                    {
                        if (inventoryBag.basicOtherItemList[i].itemDetails == null || inventoryBag.basicOtherItemList[i].itemAmount == 0)
                        {
                            return false;
                        }
                    }
                    if (inventoryBag.OtherItemStorageBag != null)
                    {
                        for (var i = 0; i < inventoryBag.OtherItemStorageBag.items.Count; i++)
                        {
                            if (inventoryBag.OtherItemStorageBag.items[i].itemDetails == null || inventoryBag.OtherItemStorageBag.items[i].itemAmount == 0)
                            {
                                return false;
                            }
                        }
                    }
                    break;
                case ItemType.储物袋:
                    if (inventoryBag.storageBags.Any(t => t == null))
                    {
                        return false;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            //再判断万能储物袋是否满了
            foreach (var t in inventoryBag.storageBags.Where(t => t != null && (t.storageBagType == StorageBagType.万能 || StorageTypeMatchItemType(t.storageBagType) == itemDetails.itemType)))
            {
                for (var j = 0; j < t.items.Count; j++)
                {
                    if (t.items[j].itemDetails == null || t.items[j].itemAmount == 0)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 寻找装备栏和携带栏中的空位，装备指定物品
        /// </summary>
        /// <param name="inventoryBag"></param>
        /// <param name="itemDetails">指定物品</param>
        /// <param name="itemAmount">数量</param>
        /// <param name="isFaBaoBag">是否在装备栏中，否的话为在携带栏中</param>
        public void EquipItem(InventoryBag inventoryBag, ItemDetails itemDetails, int itemAmount, bool isFaBaoBag, out bool isFull)
        {
            isFull = true;
            if (isFaBaoBag)
            {
                for (var i = 0; i < inventoryBag.wearingFaBaoList.Count; i++)
                {
                    if (inventoryBag.wearingFaBaoList[i].itemDetails != null) continue;
                    inventoryBag.wearingFaBaoList[i] = new InventoryItem { itemDetails = itemDetails, itemAmount = itemAmount };
                    isFull = false;
                    return;
                }
            }
            else
            {
                for (var i = 0; i < inventoryBag.carryOnItems.Count; i++)
                {
                    if (inventoryBag.carryOnItems[i].itemDetails != null) continue;
                    inventoryBag.carryOnItems[i] = new InventoryItem { itemDetails = itemDetails, itemAmount = itemAmount };
                    isFull = false;
                    return;
                }
            }
        }

        /// <summary>
        /// 寻找装备栏和携带栏中的指定物品，卸下该物品
        /// </summary>
        /// <param name="inventoryBag"></param>
        /// <param name="itemDetails">指定物品</param>
        public void UnEquipItem(InventoryBag inventoryBag, ItemDetails itemDetails)
        {
            for (var i = 0; i < inventoryBag.wearingFaBaoList.Count; i++)
            {
                if (inventoryBag.wearingFaBaoList[i].itemDetails != itemDetails) continue;
                inventoryBag.wearingFaBaoList[i] = new InventoryItem { itemDetails = null, itemAmount = 0 };
                return;
            }
            for (var i = 0; i < inventoryBag.carryOnItems.Count; i++)
            {
                if (inventoryBag.carryOnItems[i].itemDetails != itemDetails) continue;
                inventoryBag.carryOnItems[i] = new InventoryItem { itemDetails = null, itemAmount = 0 };
                return;
            }
        }
        /// <summary>
        /// 在背包中查找对应物品后移除指定数量
        /// </summary>
        /// <param name="inventoryBag">背包</param>
        /// <param name="itemDetails">物品</param>
        /// <param name="ItemAmount"></param>
        /// <param name="Success"></param>
        public void RemoveItem(InventoryBag inventoryBag, ItemDetails itemDetails, int ItemAmount, out bool Success)
        {
            Success = false;
            if (inventoryBag == null || itemDetails == null || ItemAmount == 0) return;
            //先根据物品类型检索该类型储物袋物品
            switch (itemDetails.itemType)
            {
                case ItemType.法宝:
                    for (var i = 0; i < inventoryBag.basicFaBaoList.Count; i++)
                    {
                        if (inventoryBag.basicFaBaoList[i].itemDetails != itemDetails) continue;
                        if (inventoryBag.basicFaBaoList[i].itemAmount - ItemAmount < 0) continue;
                        inventoryBag.basicFaBaoList[i] = new InventoryItem
                        {
                            itemDetails = itemDetails,
                            itemAmount = inventoryBag.basicFaBaoList[i].itemAmount - ItemAmount
                        };
                        Success = true;
                        return;
                    }
                    if (inventoryBag.FaBaoStorageBag != null)
                    {
                        for (var i = 0; i < inventoryBag.FaBaoStorageBag.items.Count; i++)
                        {
                            if (inventoryBag.FaBaoStorageBag.items[i].itemDetails != itemDetails) continue;
                            if (inventoryBag.FaBaoStorageBag.items[i].itemAmount - ItemAmount < 0) continue;
                            inventoryBag.FaBaoStorageBag.items[i] = new InventoryItem
                            {
                                itemDetails = itemDetails,
                                itemAmount = inventoryBag.FaBaoStorageBag.items[i].itemAmount - ItemAmount
                            };
                            Success = true;
                            return;
                        }
                    }
                    break;
                case ItemType.消耗品:
                    for (var i = 0; i < inventoryBag.basicConsumablesList.Count; i++)
                    {
                        if (inventoryBag.basicConsumablesList[i].itemDetails != itemDetails) continue;
                        if (inventoryBag.basicConsumablesList[i].itemAmount - ItemAmount < 0) continue;
                        inventoryBag.basicConsumablesList[i] = new InventoryItem
                        {
                            itemDetails = itemDetails,
                            itemAmount = inventoryBag.basicConsumablesList[i].itemAmount - ItemAmount
                        };
                        Success = true;
                        return;
                    }
                    if (inventoryBag.ConsumablesStorageBag != null)
                    {
                        for (var i = 0; i < inventoryBag.ConsumablesStorageBag.items.Count; i++)
                        {
                            if (inventoryBag.ConsumablesStorageBag.items[i].itemDetails != itemDetails) continue;
                            if (inventoryBag.ConsumablesStorageBag.items[i].itemAmount - ItemAmount < 0) continue;
                            inventoryBag.ConsumablesStorageBag.items[i] = new InventoryItem
                            {
                                itemDetails = itemDetails,
                                itemAmount = inventoryBag.ConsumablesStorageBag.items[i].itemAmount - ItemAmount
                            };
                            Success = true;
                            return;
                        }
                    }
                    break;
                case ItemType.任务物品:
                    for (var i = 0; i < inventoryBag.basicQuestItemList.Count; i++)
                    {
                        if (inventoryBag.basicQuestItemList[i].itemDetails != itemDetails) continue;
                        if (inventoryBag.basicQuestItemList[i].itemAmount - ItemAmount < 0) continue;
                        inventoryBag.basicQuestItemList[i] = new InventoryItem
                        {
                            itemDetails = itemDetails,
                            itemAmount = inventoryBag.basicQuestItemList[i].itemAmount - ItemAmount
                        };
                        Success = true;
                        return;
                    }
                    if (inventoryBag.QuestItemStorageBag != null)
                    {
                        for (var i = 0; i < inventoryBag.QuestItemStorageBag.items.Count; i++)
                        {
                            if (inventoryBag.QuestItemStorageBag.items[i].itemDetails != itemDetails) continue;
                            if (inventoryBag.QuestItemStorageBag.items[i].itemAmount - ItemAmount < 0) continue;
                            inventoryBag.QuestItemStorageBag.items[i] = new InventoryItem
                            {
                                itemDetails = itemDetails,
                                itemAmount = inventoryBag.QuestItemStorageBag.items[i].itemAmount - ItemAmount
                            };
                            Success = true;
                            return;
                        }
                    }
                    break;
                case ItemType.其他物品:
                    for (var i = 0; i < inventoryBag.basicOtherItemList.Count; i++)
                    {
                        if (inventoryBag.basicOtherItemList[i].itemDetails != itemDetails) continue;
                        if (inventoryBag.basicOtherItemList[i].itemAmount - ItemAmount < 0) continue;
                        inventoryBag.basicOtherItemList[i] = new InventoryItem
                        {
                            itemDetails = itemDetails,
                            itemAmount = inventoryBag.basicOtherItemList[i].itemAmount - ItemAmount
                        };
                        Success = true;
                        return;
                    }
                    if (inventoryBag.OtherItemStorageBag != null)
                    {
                        for (var i = 0; i < inventoryBag.OtherItemStorageBag.items.Count; i++)
                        {
                            if (inventoryBag.OtherItemStorageBag.items[i].itemDetails != itemDetails) continue;
                            if (inventoryBag.OtherItemStorageBag.items[i].itemAmount - ItemAmount < 0) continue;
                            inventoryBag.OtherItemStorageBag.items[i] = new InventoryItem
                            {
                                itemDetails = itemDetails,
                                itemAmount = inventoryBag.OtherItemStorageBag.items[i].itemAmount - ItemAmount
                            };
                            Success = true;
                            return;
                        }
                    }
                    break;
            }
            //其次检索已有的万能储物袋是否存在该物品
            foreach (var t in inventoryBag.storageBags.Where(t => t != null && (t.storageBagType == StorageBagType.万能 || StorageTypeMatchItemType(t.storageBagType) == itemDetails.itemType)))
            {
                for (var j = 0; j < t.items.Count; j++)
                {
                    if (t.items[j].itemDetails != itemDetails) continue;
                    if (t.items[j].itemAmount - ItemAmount < 0) continue;
                    t.items[j] = new InventoryItem
                    {
                        itemDetails = itemDetails,
                        itemAmount = t.items[j].itemAmount - ItemAmount
                    };
                    Success = true;
                    return;
                }
            }
            CheckInvalidItems(inventoryBag);
        }

        /// <summary>
        /// 检测当前背包中是否存在非标准的数据，如ItemDetails为Null或ItemAmount=0
        /// </summary>
        /// <param name="inventoryBag"></param>
        private void CheckInvalidItems(InventoryBag inventoryBag)
        {
            if (inventoryBag == null) return;
            //循环背包中的所有数据
            for (var i = 0; i < inventoryBag.basicFaBaoList.Count; i++)
            {
                if (inventoryBag.basicFaBaoList[i].itemDetails != null && inventoryBag.basicFaBaoList[i].itemAmount != 0) continue;
                inventoryBag.basicFaBaoList[i] = new InventoryItem
                {
                    itemDetails = null,
                    itemAmount = 0
                };
            }
            if (inventoryBag.FaBaoStorageBag != null)
            {
                for (var i = 0; i < inventoryBag.FaBaoStorageBag.items.Count; i++)
                {
                    if (inventoryBag.FaBaoStorageBag.items[i].itemDetails != null && inventoryBag.FaBaoStorageBag.items[i].itemAmount != 0) continue;
                    inventoryBag.FaBaoStorageBag.items[i] = new InventoryItem
                    {
                        itemDetails = null,
                        itemAmount = 0
                    };
                }
            }
            
            for (var i = 0; i < inventoryBag.basicConsumablesList.Count; i++)
            {
                if (inventoryBag.basicConsumablesList[i].itemDetails != null && inventoryBag.basicConsumablesList[i].itemAmount != 0) continue;
                inventoryBag.basicConsumablesList[i] = new InventoryItem
                {
                    itemDetails = null,
                    itemAmount = 0
                };
            }
            if (inventoryBag.ConsumablesStorageBag != null)
            {
                for (var i = 0; i < inventoryBag.ConsumablesStorageBag.items.Count; i++)
                {
                    if (inventoryBag.ConsumablesStorageBag.items[i].itemDetails != null && inventoryBag.ConsumablesStorageBag.items[i].itemAmount != 0) continue;
                    inventoryBag.ConsumablesStorageBag.items[i] = new InventoryItem
                    {
                        itemDetails = null,
                        itemAmount = 0
                    };
                }
            }
            
            for (var i = 0; i < inventoryBag.basicQuestItemList.Count; i++)
            {
                if (inventoryBag.basicQuestItemList[i].itemDetails != null && inventoryBag.basicQuestItemList[i].itemAmount != 0) continue;
                inventoryBag.basicQuestItemList[i] = new InventoryItem
                {
                    itemDetails = null,
                    itemAmount = 0
                };
            }
            if (inventoryBag.QuestItemStorageBag != null)
            {
                for (var i = 0; i < inventoryBag.QuestItemStorageBag.items.Count; i++)
                {
                    if (inventoryBag.QuestItemStorageBag.items[i].itemDetails != null && inventoryBag.QuestItemStorageBag.items[i].itemAmount != 0) continue;
                    inventoryBag.QuestItemStorageBag.items[i] = new InventoryItem
                    {
                        itemDetails = null,
                        itemAmount = 0
                    };
                }
            }
            
            for (var i = 0; i < inventoryBag.basicOtherItemList.Count; i++)
            {
                if (inventoryBag.basicOtherItemList[i].itemDetails != null && inventoryBag.basicOtherItemList[i].itemAmount != 0) continue;
                inventoryBag.basicOtherItemList[i] = new InventoryItem
                {
                    itemDetails = null,
                    itemAmount = 0
                };
            }
            if (inventoryBag.OtherItemStorageBag != null)
            {
                for (var i = 0; i < inventoryBag.OtherItemStorageBag.items.Count; i++)
                {
                    if (inventoryBag.OtherItemStorageBag.items[i].itemDetails != null && inventoryBag.OtherItemStorageBag.items[i].itemAmount != 0) continue;
                    inventoryBag.OtherItemStorageBag.items[i] = new InventoryItem
                    {
                        itemDetails = null,
                        itemAmount = 0
                    };
                }
            }
            //其次检索已有的储物袋
            foreach (var t in inventoryBag.storageBags.Where(t => t != null))
            {
                for (var j = 0; j < t.items.Count; j++)
                {
                    if (t.items[j].itemDetails != null && t.items[j].itemAmount != 0) continue;
                    t.items[j] = new InventoryItem
                    {
                        itemDetails = null,
                        itemAmount = 0
                    };
                }
            }
        }
        
        /// <summary>
        /// 根据当前的格子可容纳类型，转化为物品类型，用于判断是否可以拖动
        /// </summary>
        /// <param name="bagType"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private ItemType ItemSlotTypeMatchItemType(ItemSlotAvailableType bagType)
        {
            return bagType switch
            {
                ItemSlotAvailableType.法宝 => ItemType.法宝,
                ItemSlotAvailableType.消耗品 => ItemType.消耗品,
                ItemSlotAvailableType.任务物品 => ItemType.任务物品,
                ItemSlotAvailableType.其他物品 => ItemType.其他物品,
                ItemSlotAvailableType.储物袋 => ItemType.储物袋,
                _ => throw new ArgumentOutOfRangeException(nameof(bagType), bagType, null)
            };
        }
        
        /// <summary>
        /// 根据当前的格子可容纳类型，转化为物品类型，用于判断是否可以拖动
        /// </summary>
        /// <param name="bagType"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private ItemType StorageTypeMatchItemType(StorageBagType bagType)
        {
            return bagType switch
            {
                StorageBagType.法宝 => ItemType.法宝,
                StorageBagType.消耗品 => ItemType.消耗品,
                StorageBagType.任务物品 => ItemType.任务物品,
                StorageBagType.其他物品 => ItemType.其他物品,
                _ => throw new ArgumentOutOfRangeException(nameof(bagType), bagType, null)
            };
        }
        
    }
    [Serializable]
    public struct InventoryItem
    { 
        public ItemDetails itemDetails;
        public int itemAmount;
    }
}
