using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TXDCL.Inventory
{
    public class ItemSlotUI : MonoBehaviour,IBeginDragHandler,IDragHandler,IEndDragHandler
    {
        public ItemDetails itemDetails;
        public int SlotIndex;
        //public int itemID;
        public int itemAmount;
        public ItemSlotAvailableType availableItemType;//当前格子可放置的物品类型
        public Image itemImage;//物品图片
        public TextMeshProUGUI itemName;//物品名称
        //public Image itemStatsIcon;//若已装备/已携带则增加蒙版
        //public TextMeshProUGUI itemStats;//是否已装备/携带该物品
        public Text itemAmountText;//物品数量
        public bool isCarriedOnItemSlot;//是否为随身携带物品专属格子，战斗中一回合可使用两个随身携带物品，而仅可使用一个背包物品（任务道具除外）
        
        public void SetupItemSlot(InventoryItem item)
        {
            itemImage.gameObject.SetActive(false);
            itemName.gameObject.SetActive(false);
            //itemStatsIcon.gameObject.SetActive(false);
            //itemStats.gameObject.SetActive(false);
            itemAmountText.gameObject.SetActive(false);
            if (item.itemDetails == null || item.itemAmount == 0)
            {
                SetUpEmptySlotUI();
                return;
            }
            //if (!StorageTypeMatchItemType(availableItemType, itemDetails.itemType)) return;
            itemDetails = item.itemDetails.itemType switch
            {
                ItemType.法宝 => item.itemDetails as FaBaoDetails,
                ItemType.消耗品 => item.itemDetails as ConsumablesDetails,
                ItemType.任务物品 => item.itemDetails as QuestItemDetails,
                ItemType.其他物品 => item.itemDetails as OtherItemDetails,
                ItemType.储物袋 => item.itemDetails as StorageBagDetails,
                _ => item.itemDetails
            };
            //itemID = item.itemDetails.ID;
            itemAmount = item.itemAmount;
            SetUpSlotText();
        }

        public void SetupItemSlot(ItemDetails ItemDetails, int ItemAmount)
        {
            itemImage.gameObject.SetActive(false);
            itemName.gameObject.SetActive(false);
            //itemStatsIcon.gameObject.SetActive(false);
            //itemStats.gameObject.SetActive(false);
            itemAmountText.gameObject.SetActive(false);
            if (ItemDetails == null || ItemAmount == 0)
            {
                SetUpEmptySlotUI();
                return;
            }
            //if(availableItemType != ItemDetails.itemType) return;
            itemDetails = ItemDetails.itemType switch
            {
                ItemType.法宝 => ItemDetails as FaBaoDetails,
                ItemType.消耗品 => ItemDetails as ConsumablesDetails,
                ItemType.任务物品 => ItemDetails as QuestItemDetails,
                ItemType.其他物品 => ItemDetails as OtherItemDetails,
                ItemType.储物袋 => ItemDetails as StorageBagDetails,
                _ => ItemDetails
            };
            //itemID = ItemDetails.ID;
            itemAmount = ItemAmount;
            SetUpSlotText();
        }
        
        public void SetupItemSlot(ItemDetails ItemDetails)
        {
            itemImage.gameObject.SetActive(false);
            itemName.gameObject.SetActive(false);
            //itemStatsIcon.gameObject.SetActive(false);
            //itemStats.gameObject.SetActive(false);
            itemAmountText.gameObject.SetActive(false);
            if (ItemDetails == null)
            {
                SetUpEmptySlotUI();
                return;
            }
            //itemID = ItemDetails.ID;
            //if (!StorageTypeMatchItemType(availableItemType, itemDetails.itemType)) return;
            itemDetails = ItemDetails.itemType switch
            {
                ItemType.法宝 => ItemDetails as FaBaoDetails,
                ItemType.消耗品 => ItemDetails as ConsumablesDetails,
                ItemType.任务物品 => ItemDetails as QuestItemDetails,
                ItemType.其他物品 => ItemDetails as OtherItemDetails,
                ItemType.储物袋 => ItemDetails as StorageBagDetails,
                _ => ItemDetails
            };
            SetUpSlotText();
        }

        private void SetUpEmptySlotUI()
        {
            itemDetails = null;
            itemAmount = 0;
            itemImage.sprite = null;
            itemName.text = string.Empty;
            itemImage.gameObject.SetActive(false);
            itemName.gameObject.SetActive(false);
            //itemStatsIcon.gameObject.SetActive(false);
            //itemStats.gameObject.SetActive(false);
            itemAmountText.gameObject.SetActive(false);
        }

        private void SetUpSlotText()
        {
            if (itemDetails == null) return;
            itemImage.sprite = itemDetails.Icon;
            itemImage.gameObject.SetActive(true);
            itemName.text = itemDetails.Name;
            itemName.gameObject.SetActive(true);
            itemName.color = InventoryManager.Instance.GetItemColorByRarity(itemDetails.Rarity);
            if (itemDetails.itemType == ItemType.法宝 || !itemDetails.canStack || itemDetails.stackSize == 1)
                itemAmountText.text = string.Empty;
            else
                itemAmountText.text = itemAmount.ToString();
            itemAmountText.gameObject.SetActive(true);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (itemDetails == null || !itemImage.isActiveAndEnabled) return;
            InventoryUI.Instance.draggedItemIcon.sprite = itemImage.sprite;
            InventoryUI.Instance.draggedItemIcon.gameObject.SetActive(true);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (itemDetails == null || !itemImage.isActiveAndEnabled) return;
            InventoryUI.Instance.draggedItemIcon.transform.position = eventData.position;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (itemDetails == null || !itemImage.isActiveAndEnabled) return;
            InventoryUI.Instance.draggedItemIcon.gameObject.SetActive(false);
            if(eventData.pointerCurrentRaycast.gameObject == null) return;
            InventoryManager.Instance.SwapItemSlots(this, eventData.pointerCurrentRaycast.gameObject.GetComponent<ItemSlotUI>());
        }
    }
}
