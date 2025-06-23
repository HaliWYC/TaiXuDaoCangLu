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
        public Dictionary<CharacterBase, int> CharacterTurnProgressDict = new();
        // public CharacterBase player;
        public bool isCharacterTurnActive = false;
        public List<CharacterBase> CharactersInCombat = new();

        private float turnProgressModifier = 0;

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
            foreach (var character in CharactersInCombat.Where(character => !CharacterTurnProgressDict.ContainsKey(character)))
            {
                CharacterTurnProgressDict.Add(character, 0);
            }
            CombatGridPath.Instance.GetAndSetCharactersInGrid();
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
                if (CharacterTurnProgressDict[character] < Settings.TurnThreshold) continue;
                CharacterTurnProgressDict[character] -= Settings.TurnThreshold;
                isCharacterTurnActive = true;
                EventHandler.CallCharacterTurnBeginEvent(character);
            }
        }
        [ContextMenu("Combat Begin")]
        private void CombatBegin()
        {
            CharacterTurnProgressDict.Clear();
            EventHandler.CallCombatBeginEvent();
            EventHandler.CallNewCharactersEnterCombatEvent(null);
        }

        private float GetTurnProgressModifier()
        {
            return 500f / Mathf.Max(CharactersInCombat.Select(c => c.CharacterData.Reaction).ToArray());
        }
    }
}
