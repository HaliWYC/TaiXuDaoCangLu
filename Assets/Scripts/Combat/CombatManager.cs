using System;
using System.Collections.Generic;
using System.Linq;
using TXDCL.Character;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TXDCL.Combat
{
    public class CombatManager : Singleton<CombatManager>
    {
        public bool isCombating;
        public readonly Dictionary<CharacterBase, float> CharacterTurnProgressDict = new();
        public bool isCharacterTurnActive;
        public CharacterBase currentCharacter;
        public List<CharacterBase> CharactersInCombat = new();
        public List<CharacterBase> PlayerSides;
        public List<CharacterBase> EnemySides;
        private float turnProgressModifier;

        private void OnEnable()
        {
            EventHandler.NewCharactersEnterCombatEvent += OnNewCharacterEnterEvent;
        }

        private void OnDisable()
        {
            EventHandler.NewCharactersEnterCombatEvent -= OnNewCharacterEnterEvent;
        }

        private void OnNewCharacterEnterEvent(List<CharacterBase> characters)
        {
            isCombating = true;
            foreach (var character in CharactersInCombat.Where(character => !CharacterTurnProgressDict.ContainsKey(character)))
            {
                CharacterTurnProgressDict.Add(character, 0);
            }
            CombatUI.Instance.InitializedCharactersTurnProgress();
            CombatGridManager.Instance.GetAndSetCharactersInGrid();
            turnProgressModifier = GetTurnProgressModifier();
        }

        private void FixedUpdate()
        {
            if (!isCharacterTurnActive)
            {
                UpdateCharacterTurnProgress();
            }
        }
        private void UpdateCharacterTurnProgress()
        {
            if (CharacterTurnProgressDict.Count <= 0) return;
            foreach (var character in CharactersInCombat)
            {
                var value = CharacterTurnProgressDict[character];
                value += character.CharacterData.Reaction * Settings.TurnProgressBooster * turnProgressModifier *
                               UnityEngine.Time.fixedDeltaTime;
                CharacterTurnProgressDict[character] = value;
                //更新UI
                CombatUI.Instance.UpdateCharactersTurnProgressUI(character, CharacterTurnProgressDict[character]);
            }
            foreach (var character in CharacterTurnProgressDict.Where(character => character.Value >= Settings.TurnThreshold).Where(character => character.Value == CharacterTurnProgressDict.Values.Max()))
            {
                CharacterTurnProgressDict[character.Key] -= Settings.TurnThreshold;
                CombatUI.Instance.UpdateCharactersTurnProgressUI(character.Key, CharacterTurnProgressDict[character.Key]);
                isCharacterTurnActive = true;
                CursorManager.Instance.isSelecting = false;
                currentCharacter = character.Key;
                EventHandler.CallCharacterTurnBeginEvent(character.Key);
                return;
            }
        }
        public void RegisterPlayerSide(CharacterBase character)
        {
            PlayerSides.Add(character);
            if (!CharactersInCombat.Contains(character))
            {
                CharactersInCombat.Add(character);
            }
        }
        public void RegisterEnemySide(CharacterBase character)
        {
            EnemySides.Add(character);
            if (!CharactersInCombat.Contains(character))
            {
                CharactersInCombat.Add(character);
            }
        }
        [ContextMenu("Combat Begin")]
        private void CombatBegin()
        {
            CharacterTurnProgressDict.Clear();
            PlayerSides.Clear();
            EnemySides.Clear();
            CharactersInCombat.Clear();
            EventHandler.CallBeforeCombatBeginEvent();
            EventHandler.CallNewCharactersEnterCombatEvent(null);
            EventHandler.CallAfterCombatBeginEvent();
        }
        private float GetTurnProgressModifier()
        {
            return 500f / Mathf.Max(CharactersInCombat.Select(c => c.CharacterData.Reaction).ToArray());
        }
        
    }
}
