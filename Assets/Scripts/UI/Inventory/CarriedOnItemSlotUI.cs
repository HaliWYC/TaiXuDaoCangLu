using System;
using TXDCL.Combat;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TXDCL.Inventory
{
    public class CarriedOnItemSlotUI : MonoBehaviour,IPointerClickHandler
    {
        public ItemDetails itemDetails;
        public Image itemIcon;
        public Image itemRarityIcon;

        public void SetupItemSlotUI(ItemDetails ItemDetails)
        {
            if (ItemDetails == null) 
                SetupEmptySlotUI();
            else
            {
                itemDetails = ItemDetails;
                itemIcon.sprite = itemDetails.Icon;
                itemRarityIcon.color = InventoryManager.Instance.GetItemColorByRarity(itemDetails.Rarity);
            }
        }

        private void SetupEmptySlotUI()
        {
            itemIcon.sprite = null;
            itemIcon.gameObject.SetActive(false);
            itemRarityIcon.sprite = null;
            itemRarityIcon.gameObject.SetActive(false);
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            if (itemDetails == null) return;
            if (eventData.clickCount % 2 == 0)
            {
                switch (itemDetails.itemType)
                {
                    case ItemType.法宝:
                        var FaBao = itemDetails as FaBaoDetails;
                        if (FaBao.FaShuDatas.Count > 0) CombatGridManager.Instance.DisplayFaShuReleasePath(FaBao.FaShuDatas[0]);
                        break;
                    case ItemType.消耗品:
                        break;
                    case ItemType.任务物品:
                        break;
                    case ItemType.其他物品:
                        break;
                    case ItemType.储物袋:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}
