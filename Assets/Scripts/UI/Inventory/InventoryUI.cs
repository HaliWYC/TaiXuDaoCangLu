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
        public ItemSlotUI itemSlotUIPrefab;
        public CharacterBase currentCharacter;
        public InventoryBag inventoryBag;
        public Image draggedItemIcon;
        [Header("InventoryBag")] 
        public TextMeshProUGUI LingShiAmount;
        public List<ItemSlotUI> storageBags;
        public RectTransform itemSlotContainer;
        public TMP_Dropdown storageBagDropdown;
        public ItemSlotUI currentStorageBagUI;
        [Header("FaBaoBag")]
        public Image characterIcon;
        public TextMeshProUGUI characterName;
        public List<ItemSlotUI> WearingFaBaoList;
        public List<ItemSlotUI> CarryOnItemsList;
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
            inventoryBag.InitializeData();
            //SetUpSlotIndexes();
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
                storageBags[i].availableItemType = ItemType.储物袋;
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
                        var item = Instantiate(itemSlotUIPrefab, itemSlotContainer).GetComponent<ItemSlotUI>();
                        item.availableItemType = ItemType.法宝;
                        item.SetupItemSlot(inventoryBag.basicFaBaoList[i]);
                    }
                    var FaBaoCount = inventoryBag.FaBaoStorageBag!=null? inventoryBag.FaBaoStorageBag.maxCapacity : 0;
                    for (var i = 0; i < FaBaoCount; i++)
                    {
                        var item = Instantiate(itemSlotUIPrefab, itemSlotContainer).GetComponent<ItemSlotUI>();
                        item.availableItemType = ItemType.法宝;
                        item.SetupItemSlot(inventoryBag.FaBaoStorageBag.items[i]);
                    }
                    currentStorageBagUI.SetupItemSlot(inventoryBag.FaBaoStorageBag == null ? null : inventoryBag.FaBaoStorageBag);
                    break;
                case 1:
                    for (var i = 0; i < inventoryBag.basicConsumablesCapacity; i++)
                    {
                        var item = Instantiate(itemSlotUIPrefab, itemSlotContainer).GetComponent<ItemSlotUI>();
                        item.availableItemType = ItemType.消耗品;
                        item.SetupItemSlot(inventoryBag.basicConsumablesList[i]);
                    }
                    var ConsumablesCount = inventoryBag.ConsumablesStorageBag!=null? inventoryBag.ConsumablesStorageBag.maxCapacity : 0;
                    for (var i = 0; i < ConsumablesCount; i++)
                    {
                        var item = Instantiate(itemSlotUIPrefab, itemSlotContainer).GetComponent<ItemSlotUI>();
                        item.availableItemType = ItemType.消耗品;
                        item.SetupItemSlot(inventoryBag.ConsumablesStorageBag.items[i]);
                    }
                    currentStorageBagUI.SetupItemSlot(inventoryBag.ConsumablesStorageBag == null ? null : inventoryBag.ConsumablesStorageBag);
                    break;
                case 2:
                    for (var i = 0; i < inventoryBag.basicQuestItemCapacity; i++)
                    {
                        var item = Instantiate(itemSlotUIPrefab, itemSlotContainer).GetComponent<ItemSlotUI>();
                        item.availableItemType = ItemType.任务物品;
                        item.SetupItemSlot(inventoryBag.basicQuestItemList[i]);
                    }
                    var QuestItemCount = inventoryBag.QuestItemStorageBag!=null? inventoryBag.QuestItemStorageBag.maxCapacity : 0;
                    for (var i = 0; i < QuestItemCount; i++)
                    {
                        var item = Instantiate(itemSlotUIPrefab, itemSlotContainer).GetComponent<ItemSlotUI>();
                        item.availableItemType = ItemType.任务物品;
                        item.SetupItemSlot(inventoryBag.QuestItemStorageBag.items[i]);
                    }
                    currentStorageBagUI.SetupItemSlot(inventoryBag.QuestItemStorageBag == null ? null : inventoryBag.QuestItemStorageBag);
                    break;
                case 3:
                    for (var i = 0; i < inventoryBag.basicOtherItemCapacity; i++)
                    {
                        var item = Instantiate(itemSlotUIPrefab, itemSlotContainer).GetComponent<ItemSlotUI>();
                        item.availableItemType = ItemType.其他物品;
                        item.SetupItemSlot(inventoryBag.basicOtherItemList[i]);
                    }
                    var OtherItemCount = inventoryBag.OtherItemStorageBag!=null? inventoryBag.OtherItemStorageBag.maxCapacity : 0;
                    for (var i = 0; i < OtherItemCount; i++)
                    {
                        var item = Instantiate(itemSlotUIPrefab, itemSlotContainer).GetComponent<ItemSlotUI>();
                        item.availableItemType = ItemType.其他物品;
                        item.SetupItemSlot(inventoryBag.OtherItemStorageBag.items[i]);
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
                WearingFaBaoList[i].SetupItemSlot(inventoryBag.wearingFaBaoList[i]);
            }
        }
        public void SetUpCarryOnItemsList()
        {
            if(inventoryBag.carryOnItems.Count ==0 ) return;
            for (var i = 0; i < inventoryBag.carryOnItems.Count; i++)
            {
                CarryOnItemsList[i].SetupItemSlot(inventoryBag.carryOnItems[i]);
            }
        }
    }
}
