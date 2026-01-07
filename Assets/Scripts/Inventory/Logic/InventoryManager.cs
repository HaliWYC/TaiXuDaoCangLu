using System;
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
            //判断是否为当前格子是否为空或者是否与目标格子相同
            if(currentSlot == null || currentSlot == targetSlot || targetSlot == null) return;
            if (currentSlot.availableItemType == targetSlot.availableItemType)
            {
                SetItemAtIndexInBag(currentSlot,targetSlot);
                SetItemAtIndexInBag(targetSlot,currentSlot);
            }
            else 
            {
                if (currentSlot.availableItemType == ItemSlotAvailableType.万能)
                {
                    if (ItemSlotTypeMatchItemType(targetSlot.availableItemType) == currentSlot.itemDetails.itemType)
                    {
                        SetItemAtIndexInBag(currentSlot,targetSlot);
                        SetItemAtIndexInBag(targetSlot,currentSlot);
                    }
                }
                else if (targetSlot.availableItemType == ItemSlotAvailableType.万能)
                {
                    if (targetSlot.itemDetails == null)
                    {
                        if (currentSlot.itemDetails.itemType != ItemType.储物袋)
                        {
                            SetItemAtIndexInBag(currentSlot,targetSlot);
                            SetItemAtIndexInBag(targetSlot,currentSlot);
                        }
                    }
                    else if (ItemSlotTypeMatchItemType(currentSlot.availableItemType) == targetSlot.itemDetails.itemType)
                    {
                        SetItemAtIndexInBag(currentSlot,targetSlot);
                        SetItemAtIndexInBag(targetSlot,currentSlot);
                    }
                }
            }
            EventHandler.CallUpdateInventoryUIEvent(InventoryUI.Instance.currentCharacter);
        }

        private void SetItemAtIndexInBag(ItemSlotUI currentSlot, ItemSlotUI targetSlot)
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
                    InventoryUI.Instance.currentCharacter.UpdateData();
                    return;
                }
                currentIndex++;
            }
            for (var i = 0; i < inventoryBag.carryOnItems.Count; i++)
            {
                if (currentIndex == targetSlot.SlotIndex)
                {
                    inventoryBag.carryOnItems[i] = new InventoryItem { itemDetails = currentSlot.itemDetails, itemAmount = currentSlot.itemAmount};
                    InventoryUI.Instance.currentCharacter.UpdateData();
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
                                inventoryBag.ConsumablesStorageBag.items[i] = new InventoryItem { itemDetails = currentSlot.itemDetails, itemAmount = currentSlot.itemAmount};
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
        public ItemType ItemSlotTypeMatchItemType(ItemSlotAvailableType bagType)
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
    }
    [Serializable]
    public struct InventoryItem
    { 
        public ItemDetails itemDetails;
        public int itemAmount;
    }
}
