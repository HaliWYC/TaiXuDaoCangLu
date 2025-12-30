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
            if(currentSlot == null || currentSlot == targetSlot) return;
            if (currentSlot.availableItemType == targetSlot.availableItemType)
            {
                var firstItem = currentSlot.itemDetails;
                var firstAmount = currentSlot.itemAmount;
                var secondItem = targetSlot.itemDetails;
                var secondAmount = targetSlot.itemAmount;
                SetItemAtIndexInBag(targetSlot.SlotIndex, firstItem, firstAmount, currentSlot.availableItemType);
                SetItemAtIndexInBag(currentSlot.SlotIndex, secondItem, secondAmount, targetSlot.availableItemType);
                EventHandler.CallUpdateInventoryUIEvent(InventoryUI.Instance.currentCharacter);
            }
        }

        private void SetItemAtIndexInBag(int index, ItemDetails itemDetails, int itemAmount, ItemType itemType)
        {
            var currentIndex = 0;
            if (index == currentIndex)
            {
                switch (InventoryUI.Instance.storageBagDropdown.value)
                {
                    case 0:
                        InventoryUI.Instance.inventoryBag.FaBaoStorageBag = itemDetails != null ? itemDetails as StorageBagDetails : null;
                        break;
                    case 1:
                        InventoryUI.Instance.inventoryBag.ConsumablesStorageBag = itemDetails != null ? itemDetails as StorageBagDetails : null;
                        break;
                    case 2:
                        InventoryUI.Instance.inventoryBag.QuestItemStorageBag = itemDetails != null ? itemDetails as StorageBagDetails : null;
                        break;
                    case 3:
                        InventoryUI.Instance.inventoryBag.OtherItemStorageBag = itemDetails != null ? itemDetails as StorageBagDetails : null;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                return;
            }
            currentIndex++;
            for (var i = 0; i < InventoryUI.Instance.inventoryBag.storageBags.Count; i++)
            {
                if (index == currentIndex)
                {
                    InventoryUI.Instance.inventoryBag.storageBags[i] = itemDetails != null ? itemDetails as StorageBagDetails : null;
                    return;
                }
                currentIndex++;
            }
            //TODO:实现装备和携带效果
            for (var i = 0; i < InventoryUI.Instance.inventoryBag.wearingFaBaoList.Count; i++)
            {
                if (index == currentIndex)
                {
                    InventoryUI.Instance.inventoryBag.wearingFaBaoList[i] = new InventoryItem { itemDetails = itemDetails, amount = itemAmount };
                    return;
                }
                currentIndex++;
            }
            for (var i = 0; i < InventoryUI.Instance.inventoryBag.carryOnItems.Count; i++)
            {
                if (index == currentIndex)
                {
                    InventoryUI.Instance.inventoryBag.carryOnItems[i] = new InventoryItem { itemDetails = itemDetails, amount = itemAmount };
                    return;
                }
                currentIndex++;
            }
            switch (itemType)
            {
                case ItemType.法宝:
                    for (var i = 0; i < InventoryUI.Instance.inventoryBag.basicFaBaoList.Count; i++)
                    {
                        if (index == currentIndex)
                        {
                            InventoryUI.Instance.inventoryBag.basicFaBaoList[i] = new InventoryItem { itemDetails = itemDetails, amount = itemAmount };
                            return;
                        }
                        currentIndex++;
                    }

                    if (InventoryUI.Instance.inventoryBag.FaBaoStorageBag != null)
                    {
                        for (var i = 0; i < InventoryUI.Instance.inventoryBag.FaBaoStorageBag.items.Count; i++)
                        {
                            if (index == currentIndex)
                            {
                                InventoryUI.Instance.inventoryBag.FaBaoStorageBag.items[i] = new InventoryItem { itemDetails = itemDetails, amount = itemAmount };
                                return;
                            }
                            currentIndex++;
                        }
                    }
                    break;
                case ItemType.消耗品:
                    for (var i = 0; i < InventoryUI.Instance.inventoryBag.basicConsumablesList.Count; i++)
                    {
                        if (index == currentIndex)
                        { 
                            InventoryUI.Instance.inventoryBag.basicConsumablesList[i] = new InventoryItem { itemDetails = itemDetails, amount = itemAmount };
                            return;
                        }
                        currentIndex++;
                    }

                    if (InventoryUI.Instance.inventoryBag.ConsumablesStorageBag != null)
                    {
                        for (var i = 0; i < InventoryUI.Instance.inventoryBag.ConsumablesStorageBag.items.Count; i++)
                        {
                            if (index == currentIndex)
                            {
                                InventoryUI.Instance.inventoryBag.ConsumablesStorageBag.items[i] = new InventoryItem { itemDetails = itemDetails, amount = itemAmount };
                                return;
                            }
                            currentIndex++;
                        }
                    }
                    break;
                case ItemType.任务物品:
                    for (var i = 0; i < InventoryUI.Instance.inventoryBag.basicQuestItemList.Count; i++)
                    {
                        if (index == currentIndex)
                        { 
                            InventoryUI.Instance.inventoryBag.basicQuestItemList[i] = new InventoryItem { itemDetails = itemDetails, amount = itemAmount };
                            return;
                        }
                        currentIndex++;
                    }

                    if (InventoryUI.Instance.inventoryBag.QuestItemStorageBag != null)
                    {
                        for (var i = 0; i < InventoryUI.Instance.inventoryBag.QuestItemStorageBag.items.Count; i++)
                        {
                            if (index == currentIndex)
                            {
                                InventoryUI.Instance.inventoryBag.QuestItemStorageBag.items[i] = new InventoryItem { itemDetails = itemDetails, amount = itemAmount };
                                return;
                            }
                            currentIndex++;
                        }
                    }
                    break;
                case ItemType.其他物品:
                    for (var i = 0; i < InventoryUI.Instance.inventoryBag.basicOtherItemList.Count; i++)
                    {
                        if (index == currentIndex)
                        { 
                            InventoryUI.Instance.inventoryBag.basicOtherItemList[i] = new InventoryItem { itemDetails = itemDetails, amount = itemAmount };
                            return;
                        }
                        currentIndex++;
                    }

                    if (InventoryUI.Instance.inventoryBag.OtherItemStorageBag != null)
                    {
                        for (var i = 0; i < InventoryUI.Instance.inventoryBag.OtherItemStorageBag.items.Count; i++)
                        {
                            if (index == currentIndex)
                            {
                                InventoryUI.Instance.inventoryBag.OtherItemStorageBag.items[i] = new InventoryItem { itemDetails = itemDetails, amount = itemAmount };
                                return;
                            }
                            currentIndex++;
                        }
                    }
                    break;
                case ItemType.储物袋:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
        }
    }
    [Serializable]
    public struct InventoryItem
    { 
        public ItemDetails itemDetails;
        public int amount;
    }
}
