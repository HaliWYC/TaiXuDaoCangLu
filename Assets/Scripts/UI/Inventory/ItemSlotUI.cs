using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TXDCL.Inventory
{
    public class ItemSlotUI : MonoBehaviour
    {
        private ItemDetails itemDetails;
        public ItemType availableItemType;//当前格子可放置的物品类型
        public Image itemImage;//物品图片
        public TextMeshProUGUI itemName;//物品名称
        public Text itemAmount;//物品数量
        
        public bool isFaBao;//是否为法宝，装备法宝将获得法宝提供的基础属性
        public bool isCarriedOn;//是否为随身携带物品，战斗中一回合可使用两个随身携带物品，而仅可使用一个背包物品（任务道具除外）
        
        public void SetupItemSlot(Item item)
        {
            gameObject.SetActive(false);
            if(availableItemType != item.itemDetails.itemType) return;
            itemDetails = item.itemDetails.itemType switch
            {
                ItemType.法宝 => item.itemDetails as FaBaoDetails,
                ItemType.消耗品 => item.itemDetails as ConsumablesDetails,
                ItemType.任务物品 => item.itemDetails as QuestItemDetails,
                ItemType.其他物品 => item.itemDetails as OtherItemDetails,
                ItemType.储物袋 => item.itemDetails as StorageBagDetails,
                _ => item.itemDetails
            };
            if (itemDetails == null) return;
            itemImage.sprite = itemDetails.Icon;
            itemName.text = itemDetails.Name;
            gameObject.SetActive(true);
        }
        public void SetupItemSlot(ItemDetails ItemDetails)
        {
            gameObject.SetActive(false);
            if(availableItemType != ItemDetails.itemType) return;
            itemDetails = ItemDetails.itemType switch
            {
                ItemType.法宝 => ItemDetails as FaBaoDetails,
                ItemType.消耗品 => ItemDetails as ConsumablesDetails,
                ItemType.任务物品 => ItemDetails as QuestItemDetails,
                ItemType.其他物品 => ItemDetails as OtherItemDetails,
                ItemType.储物袋 => ItemDetails as StorageBagDetails,
                _ => ItemDetails
            };
            if (itemDetails == null) return;
            itemImage.sprite = itemDetails.Icon;
            itemName.text = itemDetails.Name;
            gameObject.SetActive(true);
        }
    }
}
