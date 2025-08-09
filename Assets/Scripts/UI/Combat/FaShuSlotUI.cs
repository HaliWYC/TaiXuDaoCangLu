using System;
using TMPro;
using TXDCL.XiuLian.FuShu;
using UnityEngine;
using UnityEngine.UI;

public class FaShuSlotUI : MonoBehaviour
{
    public Image FaShuIcon;
    public Button FaShuButton;
    private FaShuData faShuData;
    public bool CanCastFaShu;

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
        UpdateFaShuSlotUI(canCast);
    }

    private void UpdateFaShuSlotUI( bool canCast)
    {
        CanCastFaShu = canCast;
        FaShuButton.interactable = CanCastFaShu;
        FaShuIcon.color = canCast ? Color.white : new Color(1f, 1f, 1f, 0.2f);
    }
}
