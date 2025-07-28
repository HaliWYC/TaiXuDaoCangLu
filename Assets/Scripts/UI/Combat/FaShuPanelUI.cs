using System.Collections.Generic;
using TXDCL.XiuLian.FuShu;
using UnityEngine;

public class FaShuPanelUI : MonoBehaviour
{
    public GameObject FaBaoPanel;
    public List<FaShuSlotUI> FaShuSlots;
    public GameObject DaoCangPanel;

    public void SetUpFaShuSlots(List<FaShuData> faShuSlots)
    {
        for (var i = 0; i < FaShuSlots.Count; i++)
        {
            if (i < faShuSlots.Count)
            {
                FaShuSlots[i].SetUpFaShuSlotUI(faShuSlots[i]);
                FaShuSlots[i].FaShuIcon.gameObject.SetActive(true);
            }
            else
            {
                FaShuSlots[i].FaShuIcon.gameObject.SetActive(false);
            }
        }
    }
    
}
