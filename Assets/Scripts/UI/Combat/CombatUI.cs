using System;
using TXDCL.Character;
using UnityEngine;

public class CombatUI : Singleton<CombatUI>
{
    public CanvasGroup canvasGroup;
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
    
    private void OnCharacterTurnEndEvent(CharacterBase character)
    {
        CharacterStatsPanel.Instance.CharaterStats.gameObject.SetActive(false);
        FaShuPanelUI.gameObject.SetActive(false);
    }

    public void FadeCombatPanel(float alpha)
    {
        canvasGroup.alpha = alpha;
    }
}
