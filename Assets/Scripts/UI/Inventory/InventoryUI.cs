using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using TXDCL.Character;
using UnityEngine;
using UnityEngine.UI;

namespace TXDCL.Inventory
{
    public class InventoryUI : Singleton<InventoryUI>
    {
        public ItemSlotUI itemSlotUIPrefab;//背包格子UI预制体
        public CharacterBase currentCharacter;//当前浏览的角色
        public InventoryBag inventoryBag;//当前浏览的角色的背包
        public Image draggedItemIcon;
        [Header("InventoryBag")] 
        public TextMeshProUGUI LingShiAmount;//灵石数量TextUI
        public List<ItemSlotUI> storageBags;//储物袋UI
        public RectTransform itemSlotContainer;//背包容器
        public TMP_Dropdown storageBagDropdown;//当前显示储物袋类型
        public ItemSlotUI currentStorageBagUI;//当前显示储物袋图片
        [Header("FaBaoBag")]
        public Image characterIcon;//当前角色图片
        public TextMeshProUGUI characterName;//当前角色名字
        public List<ItemSlotUI> WearingFaBaoList;//装备栏
        public List<ItemSlotUI> CarryOnItemsList;//携带栏
        public Toggle wearingFaBaoToggle;//装备栏启用转换键
        public Toggle carryOnItemsToggle;//携带栏启用转换键

        protected override void Awake()
        {
            base.Awake();
            wearingFaBaoToggle.isOn = true;
            carryOnItemsToggle.isOn = false;
        }

        private void OnEnable()
        {
            EventHandler.UpdateInventoryUIEvent += OnUpdateInventoryUIEvent;
        }

        private void OnDisable()
        {
            EventHandler.UpdateInventoryUIEvent -= OnUpdateInventoryUIEvent;
        }
        private void OnUpdateInventoryUIEvent(CharacterBase character)
        {
            currentCharacter = character;
            inventoryBag = currentCharacter.InventoryBag;
            SetUpStorageBags();
            SetUpFaBaoBag();
            SetUpItemBag();
        }
        private void SetUpSlotIndexes()
        {
            var currentSlotIndex = 0;
            currentStorageBagUI.SlotIndex = currentSlotIndex;
            currentSlotIndex++;
            foreach (var s in storageBags)
            {
                //Debug.Log(currentSlotIndex);
                s.SlotIndex = currentSlotIndex;
                currentSlotIndex++;
            }
            
            foreach (var s in WearingFaBaoList)
            {
                //Debug.Log(currentSlotIndex);
                s.SlotIndex = currentSlotIndex;
                currentSlotIndex++;
            }

            foreach (var s in CarryOnItemsList)
            {
                //Debug.Log(currentSlotIndex);
                s.SlotIndex = currentSlotIndex;
                currentSlotIndex++;
            }
            for (var i = 0; i < itemSlotContainer.childCount; i++)
            {
                //Debug.Log(currentSlotIndex);
                itemSlotContainer.GetChild(i).GetComponent<ItemSlotUI>().SlotIndex = currentSlotIndex;
                currentSlotIndex++;
            }
        }
        private void SetUpStorageBags()
        {
            //TODO:根据实际情况决定是否就10个储物袋
            for (var i = 0; i < storageBags.Count; i++)
            {
                storageBags[i].availableItemType = ItemSlotAvailableType.储物袋;
                storageBags[i].SetupItemSlot(inventoryBag.storageBags[i]);
            }
        }

        public void SetUpItemBag()
        {
            StartCoroutine(SetupItemBag());
        }

        private IEnumerator SetupItemBag()
        {
            LingShiAmount.text = inventoryBag.LingShiAmount.ToString();
            for (var i = 0; i < itemSlotContainer.childCount; i++)
            {
                Destroy(itemSlotContainer.GetChild(i).gameObject);
            }
            yield return null;
            switch (storageBagDropdown.value)
            {
                case 0:
                    for (var i = 0; i < inventoryBag.basicFaBaoCapacity; i++)
                    {
                        var slot = Instantiate(itemSlotUIPrefab, itemSlotContainer).GetComponent<ItemSlotUI>();
                        slot.availableItemType = ItemSlotAvailableType.法宝;
                        slot.SetupItemSlot(inventoryBag.basicFaBaoList[i]);
                    }

                    var FaBaoCount = inventoryBag.FaBaoStorageBag != null ? inventoryBag.FaBaoStorageBag.items.Count : 0;
                    for (var i = 0; i < FaBaoCount; i++)
                    {
                        var slot = Instantiate(itemSlotUIPrefab, itemSlotContainer).GetComponent<ItemSlotUI>();
                        slot.availableItemType = ItemSlotAvailableType.法宝;
                        slot.SetupItemSlot(inventoryBag.FaBaoStorageBag.items[i]);
                    }
                    currentStorageBagUI.SetupItemSlot(inventoryBag.FaBaoStorageBag == null ? null : inventoryBag.FaBaoStorageBag);
                    break;
                case 1:
                    for (var i = 0; i < inventoryBag.basicConsumablesCapacity; i++)
                    {
                        var slot = Instantiate(itemSlotUIPrefab, itemSlotContainer).GetComponent<ItemSlotUI>();
                        slot.availableItemType = ItemSlotAvailableType.消耗品;
                        slot.SetupItemSlot(inventoryBag.basicConsumablesList[i]);
                    }

                    var ConsumablesCount = inventoryBag.ConsumablesStorageBag != null ? inventoryBag.ConsumablesStorageBag.items.Count : 0;
                    for (var i = 0; i < ConsumablesCount; i++)
                    {
                        var slot = Instantiate(itemSlotUIPrefab, itemSlotContainer).GetComponent<ItemSlotUI>();
                        slot.availableItemType = ItemSlotAvailableType.消耗品;
                        slot.SetupItemSlot(inventoryBag.ConsumablesStorageBag.items[i]);
                    }
                    currentStorageBagUI.SetupItemSlot(inventoryBag.ConsumablesStorageBag == null ? null : inventoryBag.ConsumablesStorageBag);
                    break;
                case 2:
                    for (var i = 0; i < inventoryBag.basicQuestItemCapacity; i++)
                    {
                        var slot = Instantiate(itemSlotUIPrefab, itemSlotContainer).GetComponent<ItemSlotUI>();
                        slot.availableItemType = ItemSlotAvailableType.任务物品;
                        slot.SetupItemSlot(inventoryBag.basicQuestItemList[i]);
                    }

                    var QuestItemCount = inventoryBag.QuestItemStorageBag != null ? inventoryBag.QuestItemStorageBag.items.Count : 0;
                    for (var i = 0; i < QuestItemCount; i++)
                    {
                        var slot = Instantiate(itemSlotUIPrefab, itemSlotContainer).GetComponent<ItemSlotUI>();
                        slot.availableItemType = ItemSlotAvailableType.任务物品;
                        slot.SetupItemSlot(inventoryBag.QuestItemStorageBag.items[i]);
                    }
                    currentStorageBagUI.SetupItemSlot(inventoryBag.QuestItemStorageBag == null ? null : inventoryBag.QuestItemStorageBag);
                    break;
                case 3:
                    for (var i = 0; i < inventoryBag.basicOtherItemCapacity; i++)
                    {
                        var slot = Instantiate(itemSlotUIPrefab, itemSlotContainer).GetComponent<ItemSlotUI>();
                        slot.availableItemType = ItemSlotAvailableType.其他物品;
                        slot.SetupItemSlot(inventoryBag.basicOtherItemList[i]);
                    }

                    var OtherItemCount = inventoryBag.OtherItemStorageBag != null ? inventoryBag.OtherItemStorageBag.items.Count : 0;
                    for (var i = 0; i < OtherItemCount; i++)
                    {
                        var slot = Instantiate(itemSlotUIPrefab, itemSlotContainer).GetComponent<ItemSlotUI>();
                        slot.availableItemType = ItemSlotAvailableType.其他物品;
                        slot.SetupItemSlot(inventoryBag.OtherItemStorageBag.items[i]);
                    }
                    currentStorageBagUI.SetupItemSlot(inventoryBag.OtherItemStorageBag == null ? null : inventoryBag.OtherItemStorageBag);
                    break;
            }
            yield return null;
            SetUpSlotIndexes();
        }

        private void SetUpFaBaoBag()
        {
            characterIcon.sprite = currentCharacter.CharacterData.characterSprite;
            characterName.text = currentCharacter.CharacterData.characterName;
            SetUpWearingFaBaoList();
            SetUpCarryOnItemsList();
        }

        public void SetUpWearingFaBaoList()
        {
            if(inventoryBag.wearingFaBaoList.Count ==0) return;
            for (var i = 0; i < inventoryBag.wearingFaBaoList.Count; i++)
            {
                WearingFaBaoList[i].availableItemType = ItemSlotAvailableType.法宝;
                WearingFaBaoList[i].SetupItemSlot(inventoryBag.wearingFaBaoList[i]);
            }
        }
        public void SetUpCarryOnItemsList()
        {
            if(inventoryBag.carryOnItems.Count ==0 ) return;
            for (var i = 0; i < inventoryBag.carryOnItems.Count; i++)
            {
                CarryOnItemsList[i].availableItemType = ItemSlotAvailableType.万能;
                CarryOnItemsList[i].SetupItemSlot(inventoryBag.carryOnItems[i]);
            }
        }
    }
}
