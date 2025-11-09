using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DaoCangSlotUI : MonoBehaviour,IBeginDragHandler,IDragHandler,IEndDragHandler,IPointerClickHandler
{
    public Image Icon;
    public TextMeshProUGUI AmountText;
    public WuxingDaoCang wuxingDaoCang;
    public bool isSelectingSlot;//是否为已选择的道藏槽
    public bool isRequireDaoCang;//是否为必要消耗的道藏，如火焰类法术几乎必定需要消耗一定数量的火道藏
    public int sameDaoCangCost;
    
    public void SetUpDaoCangSlotUI(WuxingDaoCang DaoCang, bool IsSelectingSlot, bool IsRequireDaoCang)
    {
        wuxingDaoCang = new WuxingDaoCang { Wuxing = DaoCang.Wuxing, DaoCang = DaoCang.DaoCang };
        isSelectingSlot = IsSelectingSlot;
        isRequireDaoCang = IsRequireDaoCang;
        Icon.sprite = DaoCangPanelUI.Instance.GetWuXingIcon(DaoCang.Wuxing.currentWuXing);
        AmountText.text = wuxingDaoCang.DaoCang.ToString();
    }
    
    public void SetUpDaoCangSlotUI(WuxingDaoCang DaoCang, int Amount, int SameDaoCangCost, bool IsSelectingSlot, bool IsRequireDaoCang)
    {
        wuxingDaoCang = new WuxingDaoCang { Wuxing = DaoCang.Wuxing, DaoCang = Amount };
        isSelectingSlot = IsSelectingSlot;
        isRequireDaoCang = IsRequireDaoCang;
        sameDaoCangCost = SameDaoCangCost;
        Icon.sprite = DaoCangPanelUI.Instance.GetWuXingIcon(DaoCang.Wuxing.currentWuXing);
        AmountText.text = (wuxingDaoCang.DaoCang + sameDaoCangCost).ToString();
    }

    public void UpdateDaoCangSlotUI(int DaoCang, int SameDaoCangCost)
    {
        wuxingDaoCang.DaoCang = DaoCang;
        sameDaoCangCost = SameDaoCangCost;
        AmountText.text = (wuxingDaoCang.DaoCang + sameDaoCangCost).ToString();
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (wuxingDaoCang.DaoCang <= 0 && !isSelectingSlot) return;
        DaoCangPanelUI.Instance.SwapDaoCangIcon.sprite = Icon.sprite;
        DaoCangPanelUI.Instance.SwapDaoCangIcon.gameObject.SetActive(true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        DaoCangPanelUI.Instance.SwapDaoCangIcon.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        DaoCangPanelUI.Instance.SwapDaoCangIcon.gameObject.SetActive(false);
        if (eventData.pointerCurrentRaycast.gameObject == null) return;
        if (eventData.pointerCurrentRaycast.gameObject.GetComponent<DaoCangSlotUI>() == null) return;
        var targetSlot = eventData.pointerCurrentRaycast.gameObject.GetComponent<DaoCangSlotUI>();
        if (!targetSlot.isSelectingSlot) return;
        DaoCangPanelUI.Instance.SwapSelectingDaoCangSlot(this, targetSlot);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(isSelectingSlot) return;
        if (eventData.clickCount % 2 == 0)
        {
            DaoCangPanelUI.Instance.ChangeFaShuSameCostSlots(this);
        }
    }
}
