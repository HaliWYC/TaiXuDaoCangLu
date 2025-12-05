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
        private readonly Dictionary<CharacterBase, int> CharacterTurnProgressDict = new();
        public bool isCharacterTurnActive;
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
                value += (int)(character.CharacterData.Reaction * Settings.TurnProgress * turnProgressModifier *
                               UnityEngine.Time.fixedDeltaTime);
                CharacterTurnProgressDict[character] = value;
                //更新UI
                if (CharacterTurnProgressDict[character] < Settings.TurnThreshold) continue;
                //Debug.Log(character.CharacterData.characterName);
                CharacterTurnProgressDict[character] -= Settings.TurnThreshold;
                isCharacterTurnActive = true;
                CursorManager.Instance.isSelecting = false;
                EventHandler.CallCharacterTurnBeginEvent(character);
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
