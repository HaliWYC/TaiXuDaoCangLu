using TXDCL.Combat;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TXDCL.Inventory
{
    public class ItemSlotMethodsTip : Singleton<ItemSlotMethodsTip>, IPointerExitHandler
    {
        public GameObject slotMethodsUIObject;//背包格子选项UI
        public ItemSlotUI currentSelectedSlot;//当前选定的格子，用于判断是哪个格子调用更多方法选项
        public Button MoreInformation;//详情
        public Button UseInCombat;//战斗中使用
        public Button Equip;//装备
        public Button UnEquip;//卸下
        public Button MassUse;//批量使用
        public Button Split;//拆分
        
        protected override void Awake()
        {
            base.Awake();
            MoreInformation.onClick.AddListener(OnMoreInformationButtonClicked);
            UseInCombat.onClick.AddListener(OnUseInCombatButtonClicked);
            Equip.onClick.AddListener(OnEquipButtonClicked);
            UnEquip.onClick.AddListener(OnUnEquipButtonClicked);
            MassUse.onClick.AddListener(OnMassUseButtonClicked);
            Split.onClick.AddListener(OnSplitButtonClicked);
        }
        
        public void Setup()
        {
            MoreInformation.gameObject.SetActive(true);
            UseInCombat.gameObject.SetActive(false);
            Equip.gameObject.SetActive(false);
            UnEquip.gameObject.SetActive(false);
            MassUse.gameObject.SetActive(false);
            Split.gameObject.SetActive(false);
            if (CombatManager.Instance.isCombating)
            {
                if (!currentSelectedSlot.isCarriedOnItemSlot)
                {
                    UseInCombat.gameObject.SetActive(true);
                    return;
                }
            }
            else if(currentSelectedSlot.isWearingFaBaoSlot || currentSelectedSlot.isCarriedOnItemSlot)
            {
                UnEquip.gameObject.SetActive(true);
            }
            else if (currentSelectedSlot.itemDetails.itemType is ItemType.法宝 or ItemType.消耗品)
            {
                Equip.gameObject.SetActive(true);
                if (currentSelectedSlot.itemDetails.itemType == ItemType.消耗品)
                {
                    MassUse.gameObject.SetActive(true);
                }
            }
            if(currentSelectedSlot.itemAmount > 1 )
            {
                Split.gameObject.SetActive(true);
            }
        }
        
        private void OnMoreInformationButtonClicked()
        {
            slotMethodsUIObject.SetActive(false);
            ItemToolTips.Instance.ResetTooltip();
            ItemToolTips.Instance.SetUpTooltip(currentSelectedSlot, true);
        }
        private void OnUseInCombatButtonClicked()
        {
            //显示法术使用待定阶段，若使用则使用后直接跳过当前回合
            
        }
        private void OnEquipButtonClicked()
        {
            if (currentSelectedSlot.itemDetails.itemType is ItemType.法宝 or ItemType.消耗品)
            {
                //检测目前启用的是装备栏或携带栏
                if (InventoryUI.Instance.wearingFaBaoToggle.isOn)
                {
                    //若当前启用的为装备栏则检测目前是否为法宝，非法宝无法添加入装备栏
                    if (currentSelectedSlot.itemDetails.itemType != ItemType.法宝) return;
                    InventoryManager.Instance.EquipItem(InventoryUI.Instance.inventoryBag, currentSelectedSlot.itemDetails, currentSelectedSlot.itemAmount, true, out var isFull);
                    if (!isFull)
                    {
                        InventoryManager.Instance.RemoveItem(InventoryUI.Instance.inventoryBag, currentSelectedSlot.itemDetails, currentSelectedSlot.itemAmount, out var Success);
                    }
                }
                else if (InventoryUI.Instance.carryOnItemsToggle.isOn)
                {
                    InventoryManager.Instance.EquipItem(InventoryUI.Instance.inventoryBag, currentSelectedSlot.itemDetails, currentSelectedSlot.itemAmount, false, out var isFull);
                    if (!isFull) InventoryManager.Instance.RemoveItem(InventoryUI.Instance.inventoryBag, currentSelectedSlot.itemDetails, currentSelectedSlot.itemAmount, out var Success);
                }
                slotMethodsUIObject.SetActive(false);
                EventHandler.CallUpdateInventoryUIEvent(InventoryUI.Instance.currentCharacter);
                InventoryUI.Instance.currentCharacter.UpdateCharacterData();
            }
        }
        private void OnUnEquipButtonClicked()
        {
            InventoryManager.Instance.AddItem(InventoryUI.Instance.inventoryBag, new InventoryItem { itemDetails = currentSelectedSlot.itemDetails, itemAmount = currentSelectedSlot.itemAmount }, out var Success);
            //若添加物品未成功则不执行移除命令
            if (Success) InventoryManager.Instance.UnEquipItem(InventoryUI.Instance.inventoryBag, currentSelectedSlot.itemDetails);
            slotMethodsUIObject.SetActive(false);
            EventHandler.CallUpdateInventoryUIEvent(InventoryUI.Instance.currentCharacter);
            InventoryUI.Instance.currentCharacter.UpdateCharacterData();
        }
        private void OnMassUseButtonClicked()
        {
            
        }
        private void OnSplitButtonClicked()
        {
            
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            slotMethodsUIObject.SetActive(false);
        }
    }
}
