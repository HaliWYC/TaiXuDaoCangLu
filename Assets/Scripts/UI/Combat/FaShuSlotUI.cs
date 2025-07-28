using TMPro;
using TXDCL.XiuLian.FuShu;
using UnityEngine;
using UnityEngine.UI;

public class FaShuSlotUI : MonoBehaviour
{
    public Image FaShuIcon;

    public void SetUpFaShuSlotUI(FaShuData faShuData)
    {
        FaShuIcon.sprite = faShuData.FaShuIcon;
    }
}
