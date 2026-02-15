using System;
using TMPro;
using TXDCL.Combat;
using TXDCL.XiuLian.FuShu;
using UnityEngine;
using UnityEngine.UI;

public class FaShuSlotUI : MonoBehaviour
{
    private FaShuData faShuData;
    public Image FaShuIcon;
    public Button FaShuButton;
    public bool CanCastFaShu;
    public Image FaShuCoolDownIcon;
    public TextMeshProUGUI FaShuSlotIndex;
    public TextMeshProUGUI FaShuCoolDownText;
    public Image forbiddenShadow;
    private void Awake()
    {
        FaShuButton.onClick.AddListener(CastFaShu);
    }

    private void CastFaShu()
    {
        DaoCangPanelUI.Instance.SelectFaShu(faShuData);
    }

    public void SetUpFaShuSlotUI(FaShuData FaShuData, bool canCast)
    {
        faShuData = FaShuData;
        FaShuIcon.sprite = faShuData.FaShuIcon;
        FaShuCoolDownIcon.gameObject.SetActive(false);
        FaShuCoolDownText.gameObject.SetActive(false);
        forbiddenShadow.gameObject.SetActive(false);
        if (CombatUI.Instance.forbidFaShus) forbiddenShadow.gameObject.SetActive(true);
        UpdateFaShuSlotUI(canCast);
        UpdateFaShuCoolDownUI();
    }

    public void SetUpEmptyFaShuSlotUI()
    {
        faShuData = null;
        FaShuIcon.gameObject.SetActive(false);
        FaShuCoolDownIcon.gameObject.SetActive(false);
        FaShuCoolDownText.gameObject.SetActive(false);
        FaShuButton.interactable = false;
    }

    private void UpdateFaShuSlotUI( bool canCast)
    {
        CanCastFaShu = canCast;
        FaShuButton.interactable = CanCastFaShu;
        FaShuIcon.color = CanCastFaShu ? Color.white : new Color(1f, 1f, 1f, 0.2f);
    }

    private void UpdateFaShuCoolDownUI()
    {
        if (faShuData.CurrentCoolDownTime == 0)
        {
            FaShuCoolDownIcon.gameObject.SetActive(false);
            FaShuCoolDownText.gameObject.SetActive(false);
            return;
        }
        if (faShuData.MaxCoolDownTime != 0) FaShuCoolDownIcon.fillAmount = (float)faShuData.CurrentCoolDownTime / faShuData.MaxCoolDownTime;
        FaShuCoolDownIcon.gameObject.SetActive(true);
        FaShuCoolDownText.text = faShuData.CurrentCoolDownTime.ToString();
        FaShuCoolDownText.gameObject.SetActive(true);
    }
}
