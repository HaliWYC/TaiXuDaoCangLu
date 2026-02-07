using System;
using System.Collections.Generic;
using System.Linq;
using TXDCL.Character;
using TXDCL.Inventory;
using UnityEngine;
using UnityEngine.UI;

namespace TXDCL.Combat
{
    public class CombatUI : Singleton<CombatUI>
    {
        public FaShuPanelUI FaShuPanelUI;
        public GameObject CombatTurnProgressUIBar;
        public RectTransform CombatOrderHolder;
        public RectTransform CarriedOnItemsHolder;
        public GameObject CombatOrderSlotUIPrefab;
        public GameObject CarriedOnItemsSlotUIPrefab;
        public List<CarriedOnItemSlotUI> carriedOnItemSlotUIList = new ();
        //public RectTransform InitialTurnProgressRectTransform;
        private Dictionary<CharacterBase, GameObject> activeCharacters = new();
        
        private void OnEnable()
        {
            EventHandler.CharacterTurnBeginEvent += OnCharacterTurnBeginEvent;
            EventHandler.CharacterTurnEndEvent += OnCharacterTurnEndEvent;
        }

        private void OnDisable()
        {
            EventHandler.CharacterTurnBeginEvent -= OnCharacterTurnBeginEvent;
            EventHandler.CharacterTurnEndEvent -= OnCharacterTurnEndEvent;
        }
        
        public void InitializedCharactersTurnProgress()
        {
            CombatTurnProgressUIBar.gameObject.SetActive(true);
            for (var i = 0; i < CombatOrderHolder.childCount; i++)
            {
                Destroy(CombatOrderHolder.GetChild(i).gameObject);
            }
            foreach (var character in CombatManager.Instance.CharactersInCombat)
            {
                if (activeCharacters.ContainsKey(character)) continue;
                var order = Instantiate(CombatOrderSlotUIPrefab, CombatOrderHolder.transform).GetComponent<CombatOrderSlotUI>();
                order.transform.localPosition = new Vector3(-400, 10, 0);
                order.SetCharacterIcon(character);
                activeCharacters.Add(character, order.gameObject);
            }
        }

        private void OnCharacterTurnBeginEvent(CharacterBase character)
        {
            if (character != GameManager.Instance.Player)
            {
                CharacterStatsPanel.Instance.CharaterStats.gameObject.SetActive(false);
                FaShuPanelUI.gameObject.SetActive(false);
                return;
            }
            SetupCharacterCarriedOnItems(character);
            CharacterStatsPanel.Instance.CharaterStats.gameObject.SetActive(true);
            FaShuPanelUI.gameObject.SetActive(true);
            StartCoroutine(CharacterStatsPanel.Instance.UpdateCharacterStats(character));
            FaShuPanelUI.SetUpFaShuSlots(character);
            DaoCangPanelUI.Instance.InitializeDaoCangPanel(character);
        }
        
        /// <summary>
        /// 初始化当前角色的携带物品并同步UI
        /// </summary>
        /// <param name="character"></param>
        private void SetupCharacterCarriedOnItems(CharacterBase character)
        {
            carriedOnItemSlotUIList.Clear();
            for (var i = 0; i < CarriedOnItemsHolder.childCount; i++)
            {
                Destroy(CarriedOnItemsHolder.GetChild(i).gameObject);
            }
            for (var i = 0; i < character.InventoryBag.carryOnItems.Count; i++)
            {
                var itemSlot = Instantiate(CarriedOnItemsSlotUIPrefab, CarriedOnItemsHolder.transform).GetComponent<CarriedOnItemSlotUI>();
                carriedOnItemSlotUIList.Add(itemSlot);
                itemSlot.SetupItemSlotUI(character.InventoryBag.carryOnItems[i].itemDetails);
            }
        }
        /// <summary>
        /// 更新当前战斗中角色的回合进度条
        /// </summary>
        /// <param name="character"></param>
        /// <param name="value"></param>
        public void UpdateCharactersTurnProgressUI(CharacterBase character, float value)
        {
            if(!activeCharacters.TryGetValue(character, out var activeCharacter)) return;
            activeCharacter.transform.localPosition = new Vector3(value < 0 ? Mathf.Max(-1000, -400 + value * 0.4f) : Mathf.Min(1000, -400 + value * 0.4f), 10, 0);
        }
        
        private void OnCharacterTurnEndEvent(CharacterBase character)
        {
            CharacterStatsPanel.Instance.CharaterStats.gameObject.SetActive(false);
            FaShuPanelUI.gameObject.SetActive(false);
        }
        /// <summary>
        /// 是否显示法术（携带物品、道藏）面板
        /// </summary>
        /// <param name="ignore"></param>
        public void IgnoreCombatPanel(bool ignore)
        {
            FaShuPanelUI.gameObject.SetActive(!ignore);
        }

        public void SelectCarriedOnItem(int index)
        {
            if (carriedOnItemSlotUIList.Count <= index || carriedOnItemSlotUIList[index].itemDetails == null) return;
            switch (carriedOnItemSlotUIList[index].itemDetails.itemType)
            {
                case ItemType.法宝:
                    var FaBao = carriedOnItemSlotUIList[index].itemDetails as FaBaoDetails;
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
