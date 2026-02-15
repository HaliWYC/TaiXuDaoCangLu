using System;
using System.Collections.Generic;
using TXDCL.Character;
using TXDCL.Combat;
using TXDCL.XiuLian.FuShu;
using UnityEngine;
using UnityEngine.UI;

public class CombatPanelUI : MonoBehaviour
{
    private List<FaShuData> currentFaShuList;
    private FaShuData currentSelectingFaShu;
    public GameObject FaShuSlotPrefab;
    public RectTransform FaShuSlotHolder;
    public List<FaShuSlotUI> FaShuSlots;
    public Button InventoryBagButton;
    private bool isSelectedFaShu;
    private void Awake()
    {
        InventoryBagButton.onClick.AddListener(OpenInventoryBag);
    }

    private void OpenInventoryBag()
    {
        MenuUI.Instance.menuPanel.SetActive(true);
        MenuUI.Instance.FunctionsContainer.SetActive(false);
        MenuUI.Instance.ResetTogglesStatus();
        MenuUI.Instance. FunctionToggles[1].isOn = true;
        EventHandler.CallUpdateInventoryUIEvent(GameManager.Instance.Player);
        MenuUI.Instance.FunctionsPanel.SetActive(true);
    }
    
    public void SetUpFaShuSlots(CharacterBase character)
    {
        currentFaShuList = character.currentFaShuList;
        FaShuSlots.Clear();
        for (var i = 0; i < FaShuSlotHolder.childCount; i++)
        {
            Destroy(FaShuSlotHolder.GetChild(i).gameObject);
        }

        for (var i = 0; i < 10; i++)
        {
            var FaShuSlot = Instantiate(FaShuSlotPrefab, FaShuSlotHolder).GetComponent<FaShuSlotUI>();
            FaShuSlot.FaShuSlotIndex.text = i == 9 ? "0" : (i + 1).ToString();
            if (i < character.currentFaShuList.Count)
            {
                FaShuSlot.SetUpFaShuSlotUI(character.currentFaShuList[i],
                    !CombatUI.Instance.forbidFaShus &&
                    FaShuManager.Instance.CheckReleaseFaShuConditions(character.CharacterData,
                        character.currentFaShuList[i], true));
                FaShuSlot.FaShuIcon.gameObject.SetActive(true);
                FaShuSlots.Add(FaShuSlot);
            }
            else
            {
                FaShuSlot.SetUpEmptyFaShuSlotUI();
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
            if (!DaoCangPanelUI.Instance.EndTurnButton.isActiveAndEnabled)
            {
                DaoCangPanelUI.Instance.ConfirmButtonOnClick();
            }
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
