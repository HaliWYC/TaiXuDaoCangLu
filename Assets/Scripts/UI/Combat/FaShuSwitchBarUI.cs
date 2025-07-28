using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FaShuSwitchBarUI : MonoBehaviour
{
    public Button NormalFaShuButton;
    public Button MeleeFaShuButton;
    public Button ShenShiFaShuButton;
    public Image ResourcesIcon;
    public TextMeshProUGUI ResourcesText;
    public TextMeshProUGUI AmountText;

    private void Awake()
    {
        NormalFaShuButton.onClick.AddListener(OnNormalFaShuButtonClick);
        MeleeFaShuButton.onClick.AddListener(OnMeleeFaShuButtonClick);
        ShenShiFaShuButton.onClick.AddListener(OnShenShiFaButtonClick);
    }
    
    private void OnNormalFaShuButtonClick()
    {
        
    }
    private void OnMeleeFaShuButtonClick()
    {
        
    }

    private void OnShenShiFaButtonClick()
    {
        
    }

    private void UpdateFaShuBarUI()
    {
        
    }
}
