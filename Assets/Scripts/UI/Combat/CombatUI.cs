using System;
using TXDCL.Character;
using UnityEngine;

public class CombatUI : Singleton<CombatUI>
{
    public CanvasGroup canvasGroup;
    public CharacterStats CharacterStatsPanel;
    public FaShuPanelUI FaShuPanel;
    private void OnEnable()
    {
        EventHandler.CharacterTurnBeginEvent += OnCharacterTurnBeginEvent;
    }
    private void OnDisable()
    {
        EventHandler.CharacterTurnBeginEvent -= OnCharacterTurnBeginEvent;
    }
    
    private void OnCharacterTurnBeginEvent(CharacterBase character)
    {
        if (character != GameManager.Instance.Player)
        {
            CharacterStatsPanel.gameObject.SetActive(false);
            FaShuPanel.gameObject.SetActive(false);
            return;
        }
        CharacterStatsPanel.gameObject.SetActive(true);
        FaShuPanel.gameObject.SetActive(true);
        CharacterStatsPanel.UpdateCharacterStats(character.CharacterData);
        FaShuPanel.SetUpFaShuSlots(character.currentFaShuList);
    }

    public void FadeCombatPanel(float alpha, bool blockcast)
    {
        canvasGroup.alpha = alpha;
        canvasGroup.blocksRaycasts = blockcast;
    }
}
