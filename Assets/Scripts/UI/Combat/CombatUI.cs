using System;
using TXDCL.Character;
using UnityEngine;

public class CombatUI : Singleton<CombatUI>
{
    public CanvasGroup canvasGroup;
    public CharacterStats CharacterStatsPanel;
    public FaShuPanelUI FaShuPanelUI;
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
    
    private void OnCharacterTurnBeginEvent(CharacterBase character)
    {
        if (character != GameManager.Instance.Player)
        {
            CharacterStatsPanel.gameObject.SetActive(false);
            FaShuPanelUI.gameObject.SetActive(false);
            return;
        }
        CharacterStatsPanel.gameObject.SetActive(true);
        FaShuPanelUI.gameObject.SetActive(true);
        CharacterStatsPanel.UpdateCharacterStats(character.CharacterData);
        FaShuPanelUI.SetUpFaShuSlots(character);
        DaoCangPanelUI.Instance.InitializeDaoCangPanel(character);
    }
    
    private void OnCharacterTurnEndEvent()
    {
        CharacterStatsPanel.gameObject.SetActive(false);
        FaShuPanelUI.gameObject.SetActive(false);
    }

    public void FadeCombatPanel(float alpha)
    {
        canvasGroup.alpha = alpha;
    }
}
