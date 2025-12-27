using System.Collections.Generic;
using TXDCL.Character;
using TXDCL.Combat;
using UnityEngine;
using UnityEngine.UI;

public class MenuUI : Singleton<MenuUI>
{
    [Header("Components")]
    public GameObject menuPanel;
    public GameObject FunctionsContainer;
    public GameObject FunctionsPanel;
    public Button CharacterPanelButton;
    public Button InventoryPanelButton;
    public Button GongFa_FaShuPanelButton;
    public Button TalentPanelButton;
    public Button QuestPanelButton;
    public Button MapPanelButton;
    
    public List<Toggle> FunctionToggles;
    protected override void Awake()
    {
        base.Awake();
        CharacterPanelButton.onClick.AddListener(OnCharacterPanelButtonClicked);
        InventoryPanelButton.onClick.AddListener(OnInventoryPanelButtonClicked);
        GongFa_FaShuPanelButton.onClick.AddListener(OnGongFa_FaShuPanelButtonClicked);
        TalentPanelButton.onClick.AddListener(OnTalentPanelButtonClicked);
        QuestPanelButton.onClick.AddListener(OnQuestPanelButtonClicked);
        MapPanelButton.onClick.AddListener(OnMapPanelButtonClicked);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (CombatManager.Instance.isCombating)
            {
                if (CombatManager.Instance.currentCharacter == GameManager.Instance.Player)
                {
                    menuPanel.SetActive(true);
                    FunctionsContainer.SetActive(true);
                    FunctionsPanel.SetActive(false);
                }
            }
            else
            {
                menuPanel.SetActive(true);
                FunctionsContainer.SetActive(true);
                FunctionsPanel.SetActive(false);
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            menuPanel.SetActive(false);
        }
    }

    private void OnCharacterPanelButtonClicked()
    {
        FunctionsContainer.SetActive(false);
        ResetTogglesStatus();
        FunctionToggles[0].isOn = true;
        FunctionsPanel.SetActive(true);
    }
    private void OnInventoryPanelButtonClicked()
    {
        FunctionsContainer.SetActive(false);
        ResetTogglesStatus();
        FunctionToggles[1].isOn = true;
        EventHandler.CallUpdateInventoryUIEvent(GameManager.Instance.Player);
        FunctionsPanel.SetActive(true);
    }
    private void OnGongFa_FaShuPanelButtonClicked()
    {
        FunctionsContainer.SetActive(false);
        ResetTogglesStatus();
        FunctionToggles[2].isOn = true;
        FunctionsPanel.SetActive(true);
    }
    private void OnTalentPanelButtonClicked()
    {
        FunctionsContainer.SetActive(false);
        ResetTogglesStatus();
        FunctionToggles[3].isOn = true;
        FunctionsPanel.SetActive(true);
    }
    private void OnQuestPanelButtonClicked()
    {
        FunctionsContainer.SetActive(false);
        ResetTogglesStatus();
        FunctionToggles[4].isOn = true;
        FunctionsPanel.SetActive(true);
    }
    private void OnMapPanelButtonClicked()
    {
        FunctionsContainer.SetActive(false);
        ResetTogglesStatus();
        FunctionToggles[5].isOn = true;
        FunctionsPanel.SetActive(true);
    }

    private void ResetTogglesStatus()
    {
        foreach (var toggle in FunctionToggles)
        {
            toggle.isOn = false;
        }
    }
}
