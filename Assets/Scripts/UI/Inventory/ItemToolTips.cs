using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TXDCL.Inventory
{
    public class ItemToolTips : Singleton<ItemToolTips>,IPointerExitHandler
    {
        public RectTransform itemToolTip;
        public bool isFrozen;
        private ItemSlotUI itemSlotUI;
        private bool InDetails;
        
        [Header("Components")]
        public Image itemRarityIcon;
        public Image itemIcon;
        public TextMeshProUGUI itemNameText;
        public TextMeshProUGUI itemTypeText;
        public TextMeshProUGUI itemRarityText;
        public TextMeshProUGUI itemDescriptionText;
        public GameObject moreInformationPromptText;
        
        [Header("FaBaoDetails")]
        public GameObject faBaoDetailsPart;
        public Image currentDurabilityProgressIcon;
        public Image currentMaxDurabilityProgressIcon;
        //public Image maxDurabilityProgressIcon;
        public TextMeshProUGUI currentDurabilityText;
        public TextMeshProUGUI currentMaxDurabilityText;
        public TextMeshProUGUI maxDurabilityText;
        public TextMeshProUGUI minDepreciationDurabilityText;
        public RectTransform propertiesHolder;
        public GameObject propertiesTextPrefab;

        private void Update()
        {
            if (!itemToolTip.gameObject.activeInHierarchy) return;
            if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
            {
                SetUpTooltip(itemSlotUI, !InDetails);
            }
        }

        public void SetUpTooltip(ItemSlotUI itemSlot, bool inDetails)
        {
            if(itemSlot.itemDetails == null) return;
            itemSlotUI = itemSlot;
            InDetails = inDetails;
            var itemDetails = itemSlotUI.itemDetails;
            moreInformationPromptText.SetActive(!InDetails);
            //根据是否详细并依据物品类型的不同进一步显示特有的物品信息，如法宝特有的耐久度
            if (InDetails)
            {
                SetToolTipPosition(new Vector3 { x = itemSlotUI.transform.position.x, y = Screen.height / 2f, z = 0 }, itemSlotUI.sizeOfSlot);
                switch (itemDetails.itemType)
                {
                    case ItemType.法宝:
                        SetUpFaBaoDetails();
                        break;
                    case ItemType.消耗品:
                        break;
                    case ItemType.任务物品:
                        break;
                    case ItemType.其他物品:
                        break;
                    case ItemType.储物袋:
                        break;
                }
            }
            else
            {
                SetToolTipPosition(itemSlotUI.transform.position, itemSlotUI.sizeOfSlot);
                faBaoDetailsPart.SetActive(false);
            }
            itemRarityIcon.color = InventoryManager.Instance.GetItemColorByRarity(itemDetails.Rarity);
            itemIcon.sprite = itemDetails.Icon;
            itemNameText.text = itemDetails.Name;
            itemTypeText.text = itemDetails.itemType.ToString();
            itemRarityText.text = itemDetails.Rarity.ToString() + itemDetails.ItemRarity;
            itemDescriptionText.text = itemDetails.Description;
            itemToolTip.gameObject.SetActive(true);
            LayoutRebuilder.ForceRebuildLayoutImmediate(itemToolTip);
            StartCoroutine(FreezeTooltip());
        }

        private void SetToolTipPosition(Vector3 slotPos, float size)
        {
            var offset = 500 + (size - 300) / 2;
            transform.position = slotPos + new Vector3(slotPos.x + offset <= Screen.width ? offset : -offset, -125f, 0);
        }
        private IEnumerator FreezeTooltip()
        {
            yield return new WaitForSeconds(3f);
            yield return isFrozen = true;
        }

        /// <summary>
        /// 重置信息便签，用于接触便签锁定
        /// </summary>
        public void ResetTooltip()
        {
            StopAllCoroutines();
            isFrozen = false;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!isFrozen) return;
            ResetTooltip();
            itemToolTip.gameObject.SetActive(false);
        }
        private void SetUpFaBaoDetails()
        {
            faBaoDetailsPart.SetActive(true);
            var faBaoDetails = itemSlotUI.itemDetails as FaBaoDetails;
            if (faBaoDetails == null) return;
            //耐久
            if (faBaoDetails.maximumDurability != 0)
            {
                currentMaxDurabilityProgressIcon.fillAmount = faBaoDetails.currentMaxDurability / faBaoDetails.maximumDurability;
                currentDurabilityProgressIcon.fillAmount = faBaoDetails.currentDurability / faBaoDetails.maximumDurability;
            }
            currentDurabilityText.text = "当前耐久：" + faBaoDetails.currentDurability;
            currentMaxDurabilityText.text = "当前最大耐久：" +faBaoDetails.currentMaxDurability;
            maxDurabilityText.text = "最大耐久：" +faBaoDetails.maximumDurability;
            minDepreciationDurabilityText.text = "最低修理损耗：" +faBaoDetails.minDurabilityDepreciation;
            //属性
            for (var i = 0; i < propertiesHolder.childCount; i++)
            {
                Destroy(propertiesHolder.GetChild(i).gameObject);
            }
            foreach (var t in faBaoDetails.properties.Where(t => t.value != 0))
            {
                var PropertiesText = Instantiate(propertiesTextPrefab, propertiesHolder).GetComponent<TextMeshProUGUI>();
                PropertiesText.text = (t.value > 0 ? "+" : "-") + t.value + " " + t.propertyType;
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(itemToolTip);
        }
    }
}
