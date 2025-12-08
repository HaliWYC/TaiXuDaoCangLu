using System;
using System.Collections.Generic;
using System.Linq;
using TXDCL.Character;
using UnityEngine;
using UnityEngine.UI;

namespace TXDCL.Combat
{
    public class CombatUI : Singleton<CombatUI>
    {
        public FaShuPanelUI FaShuPanelUI;
        public GameObject CombatTurnProgressUIBar;
        public GameObject CombatOrderHolder;
        public GameObject CombatOrderSlotUIPrefab;
        public RectTransform InitialTurnProgressRectTransform;
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

            CharacterStatsPanel.Instance.CharaterStats.gameObject.SetActive(true);
            FaShuPanelUI.gameObject.SetActive(true);
            StartCoroutine(CharacterStatsPanel.Instance.UpdateCharacterStats(character));
            FaShuPanelUI.SetUpFaShuSlots(character);
            DaoCangPanelUI.Instance.InitializeDaoCangPanel(character);
        }

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

        public void IgnoreCombatPanel(bool ignore)
        {
            FaShuPanelUI.gameObject.SetActive(!ignore);
        }
    }
}
