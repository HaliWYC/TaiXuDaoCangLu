using System;
using System.Collections.Generic;
using TXDCL.Character;
using TXDCL.Combat;
using TXDCL.XiuLian.FuShu;
using UnityEngine;

public class FaShuPanelUI : Singleton<FaShuPanelUI>
{
    private List<FaShuData> currentFaShuList;
    private FaShuData currentSelectingFaShu;
    public GameObject FaBaoPanel;
    public List<FaShuSlotUI> FaShuSlots;
    public GameObject DaoCangPanel;
    private bool isSelectedFaShu;
    public void SetUpFaShuSlots(CharacterBase character)
    {
        currentFaShuList = character.currentFaShuList;
        for (var i = 0; i < FaShuSlots.Count; i++)
        {
            if (i < character.currentFaShuList.Count)
            {
                FaShuSlots[i].SetUpFaShuSlotUI(character.currentFaShuList[i], FaShuManager.Instance.CheckReleaseFaShuConditions(character.CharacterData, character.currentFaShuList[i], true));
                FaShuSlots[i].FaShuIcon.gameObject.SetActive(true);
            }
            else
            {
                FaShuSlots[i].FaShuIcon.gameObject.SetActive(false);
            }
        }
    }

    public void SelectFaShuSlot(int index)
    {
        if(currentFaShuList == null) return;
        //依次为当前法术是否超出法术列表数量，当前法术是否可释放，玩家是否能在此时施法
        if (currentFaShuList.Count <= index || !FaShuSlots[index].CanCastFaShu ||
            !CombatGridManager.Instance.canCurrentCharacterCastFaShu) return;
        //如果选择的并非已选中的法术或者为处于释放法术期间，则重新选择新的，否则取消选择
        if (currentSelectingFaShu == currentFaShuList[index])
        {
            DaoCangPanelUI.Instance.SelectFaShu(currentSelectingFaShu);
        }
        else if (currentSelectingFaShu != currentFaShuList[index] || !CursorManager.Instance.isCastingFaShu)
        {
            currentSelectingFaShu = currentFaShuList[index];
            DaoCangPanelUI.Instance.SelectFaShu(currentSelectingFaShu);
        }
        else
        {
            DaoCangPanelUI.Instance.CancelButtonOnClick();
        }
        CursorManager.Instance.isConfirm = false;
    }
}
