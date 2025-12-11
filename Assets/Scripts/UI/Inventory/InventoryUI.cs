using System;
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
        private InventoryBag inventoryBag;
        [Header("InventoryBag")] 
        public List<ItemSlotUI> storageBags;
        public RectTransform itemSlotContainer;
        public TMP_Dropdown storageBagDropdown;
        
        [Header("FaBaoBag")]
        public Image characterIcon;
        public TextMeshProUGUI characterName;
        public List<ItemSlotUI> FaBaoList;
        public List<ItemSlotUI> CarryOnItemList;
        
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
            var inventoryBag = character.InventoryBag;
            SetUpStorageBags();
            SetUpItemBag();
        }

        private void SetUpStorageBags()
        {
            for (var i = 0; i < storageBags.Count; i++)
            {
                storageBags[i].availableItemType = ItemType.储物袋;
                storageBags[i].SetupItemSlot(inventoryBag.storageBags[i]);
            }
        }

        private void SetUpItemBag()
        {
            for (var i = 0; i < itemSlotContainer.childCount; i++)
            {
                Destroy(itemSlotContainer.GetChild(i).gameObject);
            }
            switch (storageBagDropdown.value)
            {
                case 0:
                    for (var i = 0; i < inventoryBag.basicFaBaoCapacity; i++)
                    {
                        var item = Instantiate(itemSlotUIPrefab, itemSlotContainer).GetComponent<ItemSlotUI>();
                        item.availableItemType = ItemType.法宝;
                        item.SetupItemSlot(inventoryBag.basicFaBaoList[i]);
                    }
                    var FaBaoCount = inventoryBag.FaBaoStorageBag!=null? inventoryBag.FaBaoStorageBag.items.Count : 0;
                    for (var i = 0; i < FaBaoCount; i++)
                    {
                        var item = Instantiate(itemSlotUIPrefab, itemSlotContainer).GetComponent<ItemSlotUI>();
                        item.availableItemType = ItemType.法宝;
                        item.SetupItemSlot(inventoryBag.FaBaoStorageBag.items[i]);
                    }
                    break;
                case 1:
                    for (var i = 0; i < inventoryBag.basicConsumablesCapacity; i++)
                    {
                        var item = Instantiate(itemSlotUIPrefab, itemSlotContainer).GetComponent<ItemSlotUI>();
                        item.availableItemType = ItemType.消耗品;
                        item.SetupItemSlot(inventoryBag.basicConsumablesList[i]);
                    }
                    var ConsumablesCount = inventoryBag.ConsumablesStorageBag!=null? inventoryBag.ConsumablesStorageBag.items.Count : 0;
                    for (var i = 0; i < ConsumablesCount; i++)
                    {
                        var item = Instantiate(itemSlotUIPrefab, itemSlotContainer).GetComponent<ItemSlotUI>();
                        item.availableItemType = ItemType.消耗品;
                        item.SetupItemSlot(inventoryBag.ConsumablesStorageBag.items[i]);
                    }
                    break;
                case 2:
                    for (var i = 0; i < inventoryBag.basicQuestItemCapacity; i++)
                    {
                        var item = Instantiate(itemSlotUIPrefab, itemSlotContainer).GetComponent<ItemSlotUI>();
                        item.availableItemType = ItemType.任务物品;
                        item.SetupItemSlot(inventoryBag.basicQuestItemList[i]);
                    }
                    var QuestItemCount = inventoryBag.QuestItemStorageBag!=null? inventoryBag.QuestItemStorageBag.items.Count : 0;
                    for (var i = 0; i < QuestItemCount; i++)
                    {
                        var item = Instantiate(itemSlotUIPrefab, itemSlotContainer).GetComponent<ItemSlotUI>();
                        item.availableItemType = ItemType.任务物品;
                        item.SetupItemSlot(inventoryBag.QuestItemStorageBag.items[i]);
                    }
                    break;
                case 3:
                    for (var i = 0; i < inventoryBag.basicOtherItemCapacity; i++)
                    {
                        var item = Instantiate(itemSlotUIPrefab, itemSlotContainer).GetComponent<ItemSlotUI>();
                        item.availableItemType = ItemType.其他物品;
                        item.SetupItemSlot(inventoryBag.basicOtherItemList[i]);
                    }
                    var OtherItemCount = inventoryBag.OtherItemStorageBag!=null? inventoryBag.OtherItemStorageBag.items.Count : 0;
                    for (var i = 0; i < OtherItemCount; i++)
                    {
                        var item = Instantiate(itemSlotUIPrefab, itemSlotContainer).GetComponent<ItemSlotUI>();
                        item.availableItemType = ItemType.其他物品;
                        item.SetupItemSlot(inventoryBag.OtherItemStorageBag.items[i]);
                    }
                    break;
            }
        }
    }
}
