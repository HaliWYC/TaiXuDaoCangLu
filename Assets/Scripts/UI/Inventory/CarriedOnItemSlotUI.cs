using System;
using System.Collections.Generic;
using TMPro;
using TXDCL.Combat;
using TXDCL.XiuLian.FuShu;
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
        public TextMeshProUGUI IndextText;
        public TextMeshProUGUI ItemAmountText;
        public Image forbiddenShadow;
        [Header("FaBao")] 
        public Image faBaoCoolDownIcon;
        public TextMeshProUGUI faBaoCoolDownText;
        private enum CarriedOnItemSlotIndex{Q,W,E,A,S,D}
        private List<FaShuData> CurrentItemFaShuList = new();
        private List<FaShuData> PotentialItemFaShuList = new();

        public void SetupItemSlotUI(ItemDetails ItemDetails, int index)
        {
            if (ItemDetails == null) 
                SetupEmptySlotUI();
            else
            {
                itemDetails = ItemDetails;
                itemIcon.sprite = itemDetails.Icon;
                itemIcon.gameObject.SetActive(true);
                itemRarityIcon.color = InventoryManager.Instance.GetItemColorByRarity(itemDetails.Rarity);
                itemRarityIcon.gameObject.SetActive(true);
                IndextText.text = ((CarriedOnItemSlotIndex)index).ToString();
                if (itemDetails.itemType == ItemType.法宝)
                {
                    PotentialItemFaShuList = (itemDetails as FaBaoDetails).FaShuDatas;
                }
                IndextText.gameObject.SetActive(true);
                forbiddenShadow.gameObject.SetActive(false);
                CurrentItemFaShuList.Clear();
            }
        }

        public void UpdateItemCoolDownUI()
        {
            switch (itemDetails.itemType)
            {
                case ItemType.法宝:
                    var item = itemDetails as FaBaoDetails;
                    if (item.currentCoolDown == 0)
                    {
                        faBaoCoolDownIcon.gameObject.SetActive(false);
                        faBaoCoolDownText.gameObject.SetActive(false);
                        return;
                    }
                    if (item.maxCoolDown != 0) faBaoCoolDownIcon.fillAmount = (float)item.currentCoolDown / item.maxCoolDown;
                    faBaoCoolDownIcon.gameObject.SetActive(true);
                    faBaoCoolDownText.text = item.currentCoolDown.ToString();
                    faBaoCoolDownText.gameObject.SetActive(true);
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

        public void ResetItemData()
        {
            if (itemDetails == null) return;
            switch (itemDetails.itemType)
            {
                case ItemType.法宝:
                    var Item = itemDetails as FaBaoDetails;
                    Item.currentCoolDown = 0;
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
        private void SetupEmptySlotUI()
        {
            itemIcon.sprite = null;
            itemIcon.gameObject.SetActive(false);
            itemRarityIcon.sprite = null;
            itemRarityIcon.gameObject.SetActive(false);
            IndextText.text = "";
            IndextText.gameObject.SetActive(false);
            ItemAmountText.text = "";
            ItemAmountText.gameObject.SetActive(false);
            faBaoCoolDownIcon.gameObject.SetActive(false);
            faBaoCoolDownText.gameObject.SetActive(false);
            forbiddenShadow.gameObject.SetActive(false);
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            if (itemDetails == null || CombatUI.Instance.forbidCarriedOnItems) return;
            if (eventData.clickCount % 2 == 0)
            {
                CombatUI.Instance.currentCarriedOnItemSlotUI = this;
                CombatUI.Instance.UseCarriedOnItem(itemDetails);
            }
        }
    }
}
