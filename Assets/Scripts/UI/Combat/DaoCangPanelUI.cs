using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using TXDCL.Character;
using TXDCL.Combat;
using TXDCL.XiuLian.FuShu;
using UnityEngine;
using UnityEngine.UI;

public class DaoCangPanelUI : Singleton<DaoCangPanelUI>
{
    [SerializeField] private Sprite MetalDaoCangIcon;
    [SerializeField] private Sprite WoodDaoCangIcon;
    [SerializeField] private Sprite WaterDaoCangIcon;
    [SerializeField] private Sprite FireDaoCangIcon;
    [SerializeField] private Sprite EarthDaoCangIcon;
    [SerializeField] private List<DaoCangSlotUI> PotentialDaoCangSlots;
    [SerializeField] private List<DaoCangSlotUI> SelectedDaoCangSlots;
    [SerializeField] private TextMeshProUGUI Promotion_CounterText; 
    public Button EndTurnButton;
    [SerializeField] private Button ConfirmButton;
    [SerializeField] private Button CancelButton;
    private int ActiveSelectedDaoCangSlot;
    private CharacterBase currentCharacter;
    private FaShuData currentSelectingFaShu;
    public Image SwapDaoCangIcon;
    private int Promotion_CounterSign = 0;
    protected override void Awake()
    {
        base.Awake();
        ConfirmButton.onClick.AddListener(ConfirmButtonOnClick);
        CancelButton.onClick.AddListener(CancelButtonOnClick);
        EndTurnButton.onClick.AddListener(OnEndTurnButtonClick);
    }
    
    private void OnEndTurnButtonClick()
    {
        EventHandler.CallCharacterTurnEndEvent(currentCharacter);
        CombatManager.Instance.isCharacterTurnActive = false;
    }

    public void ConfirmButtonOnClick()
    {
        CombatGridManager.Instance.DisplayFaShuReleasePath(currentSelectingFaShu);
        GameManager.Instance.SetGameCameraLenInGridSize(currentSelectingFaShu.ReleaseRange);
    }

    public void CancelButtonOnClick()
    {
        currentSelectingFaShu = null;
        GameManager.Instance.ResetGameCameraLenInGridSize();
        CombatGridManager.Instance.ClearPotentialTiles();
        CombatGridManager.Instance.DisplayCharactersMovementPath();
        ResetDaoCangPanelUI();
    }
    public void ResetDaoCangPanelUI()
    {
        foreach (var slot in SelectedDaoCangSlots)
        {
            slot.gameObject.SetActive(false);
        }
        Promotion_CounterText.gameObject.SetActive(false);
        ResetCurrentDaoCang(currentCharacter.CharacterData, PotentialDaoCangSlots);
        EndTurnButton.gameObject.SetActive(true);
        ConfirmButton.gameObject.SetActive(false);
        CancelButton.gameObject.SetActive(false);
        CursorManager.Instance.isCastingFaShu = false;
    }

    /// <summary>
    /// 玩家方行动时调用初始化道藏面版功能
    /// </summary>
    /// <param name="characterData"></param>
    public void InitializeDaoCangPanel(CharacterBase character)
    {
        currentCharacter = character;
        PotentialDaoCangSlots[0].SetUpDaoCangSlotUI(new WuxingDaoCang
        {
            Wuxing = new Wuxing { currentWuXing = WuXing.锐金 },
            DaoCang = currentCharacter.CharacterData.currentMetalDaocang
        }, false, false);
        PotentialDaoCangSlots[1].SetUpDaoCangSlotUI(new WuxingDaoCang
        {
            Wuxing = new Wuxing { currentWuXing = WuXing.灵木 },
            DaoCang = currentCharacter.CharacterData.currentWoodDaocang
        }, false, false);
        PotentialDaoCangSlots[2].SetUpDaoCangSlotUI(new WuxingDaoCang
        {
            Wuxing = new Wuxing { currentWuXing = WuXing.弱水 },
            DaoCang = currentCharacter.CharacterData.currentWaterDaocang
        }, false, false);
        PotentialDaoCangSlots[3].SetUpDaoCangSlotUI(new WuxingDaoCang
        {
            Wuxing = new Wuxing { currentWuXing = WuXing.离火 },
            DaoCang = currentCharacter.CharacterData.currentFireDaocang
        }, false, false);
        PotentialDaoCangSlots[4].SetUpDaoCangSlotUI(new WuxingDaoCang
        {
            Wuxing = new Wuxing { currentWuXing = WuXing.厚土 },
            DaoCang = currentCharacter.CharacterData.currentEarthDaocang
        }, false, false);
        ConfirmButton.gameObject.SetActive(false);
        CancelButton.gameObject.SetActive(false);
        Promotion_CounterText.gameObject.SetActive(false);
        foreach (var slot in SelectedDaoCangSlots)
        {
            slot.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 选择法术
    /// </summary>
    /// <param name="FaShuData"></param>
    public void SelectFaShu(FaShuData FaShuData)
    {
        currentSelectingFaShu = FaShuData;
        //根据选择的法术所需道藏类型数量以及是否需求相同道藏决定启用对应数量的格子
        ActiveSelectedDaoCangSlot = Mathf.Min(5, currentSelectingFaShu.DaoCangCosts.Count);
        //清空选择部分的所有道藏格子
        foreach (var slot in SelectedDaoCangSlots)
        {
            slot.wuxingDaoCang.DaoCang = 0;
            slot.gameObject.SetActive(false);
        }
        //获得当前玩家身上的剩余道藏信息
        var DaoCangList = new List<WuxingDaoCang>
        {
            PotentialDaoCangSlots[0].wuxingDaoCang,
            PotentialDaoCangSlots[1].wuxingDaoCang,
            PotentialDaoCangSlots[2].wuxingDaoCang,
            PotentialDaoCangSlots[3].wuxingDaoCang,
            PotentialDaoCangSlots[4].wuxingDaoCang
        };
        DaoCangList[0].DaoCang = currentCharacter.CharacterData.currentMetalDaocang;
        DaoCangList[1].DaoCang = currentCharacter.CharacterData.currentWoodDaocang;
        DaoCangList[2].DaoCang = currentCharacter.CharacterData.currentWaterDaocang;
        DaoCangList[3].DaoCang = currentCharacter.CharacterData.currentFireDaocang;
        DaoCangList[4].DaoCang = currentCharacter.CharacterData.currentEarthDaocang;
        //限定一个最大循环数，防止进入死循环
        var LoopMaxCount = 0;
        if (currentSelectingFaShu.SameCost > 0)
        {
            ActiveSelectedDaoCangSlot += 1;
            switch (ActiveSelectedDaoCangSlot)
            {
               case 1:
                   while (LoopMaxCount < 9999)
                   {
                       LoopMaxCount++;
                       var randNum = UnityEngine.Random.Range(0, 5);
                       if (DaoCangList[randNum].DaoCang < currentSelectingFaShu.SameCost) continue;
                       DaoCangList[randNum].DaoCang-= currentSelectingFaShu.SameCost;
                       SelectedDaoCangSlots[2].SetUpDaoCangSlotUI(DaoCangList[randNum], 0, currentSelectingFaShu.SameCost, true, false);
                       SelectedDaoCangSlots[2].gameObject.SetActive(true);
                       break;
                   }
                   break;
               case 2:
                   SelectedDaoCangSlots[1].SetUpDaoCangSlotUI(currentSelectingFaShu.DaoCangCosts[0], true, true);
                   SelectedDaoCangSlots[1].gameObject.SetActive(true);
                   DaoCangList[(int)currentSelectingFaShu.DaoCangCosts[0].Wuxing.currentWuXing].DaoCang -=
                       currentSelectingFaShu.DaoCangCosts[0].DaoCang;
                   
                   while (LoopMaxCount <1000)
                   {
                       LoopMaxCount++;
                       var randNum = UnityEngine.Random.Range(0, 5);
                       if (DaoCangList[randNum].DaoCang < currentSelectingFaShu.SameCost) continue;
                       DaoCangList[randNum].DaoCang-= currentSelectingFaShu.SameCost;
                       SelectedDaoCangSlots[3].SetUpDaoCangSlotUI(DaoCangList[randNum], 0, currentSelectingFaShu.SameCost, true, false);
                       SelectedDaoCangSlots[3].gameObject.SetActive(true);
                       break;
                   }
                   break;
               case 3:
                   SelectedDaoCangSlots[1].SetUpDaoCangSlotUI(currentSelectingFaShu.DaoCangCosts[0], true, true);
                   SelectedDaoCangSlots[1].gameObject.SetActive(true);
                   DaoCangList[(int)currentSelectingFaShu.DaoCangCosts[0].Wuxing.currentWuXing].DaoCang -=
                       currentSelectingFaShu.DaoCangCosts[0].DaoCang;
                   
                   SelectedDaoCangSlots[2].SetUpDaoCangSlotUI(currentSelectingFaShu.DaoCangCosts[1], true, true);
                   SelectedDaoCangSlots[2].gameObject.SetActive(true);
                   DaoCangList[(int)currentSelectingFaShu.DaoCangCosts[1].Wuxing.currentWuXing].DaoCang -=
                       currentSelectingFaShu.DaoCangCosts[1].DaoCang;
                   
                   while (LoopMaxCount <1000)
                   {
                       LoopMaxCount++;
                       var randNum = UnityEngine.Random.Range(0, 5);
                       if (DaoCangList[randNum].DaoCang < currentSelectingFaShu.SameCost) continue;
                       DaoCangList[randNum].DaoCang-= currentSelectingFaShu.SameCost;
                       SelectedDaoCangSlots[3].SetUpDaoCangSlotUI(DaoCangList[randNum], 0, currentSelectingFaShu.SameCost, true, false);
                       SelectedDaoCangSlots[3].gameObject.SetActive(true);
                       break;
                   }
                   break;
               case 4:
                   SelectedDaoCangSlots[0].SetUpDaoCangSlotUI(currentSelectingFaShu.DaoCangCosts[0], true, true);
                   SelectedDaoCangSlots[0].gameObject.SetActive(true);
                   DaoCangList[(int)currentSelectingFaShu.DaoCangCosts[0].Wuxing.currentWuXing].DaoCang -=
                       currentSelectingFaShu.DaoCangCosts[0].DaoCang;
                   
                   SelectedDaoCangSlots[1].SetUpDaoCangSlotUI(currentSelectingFaShu.DaoCangCosts[1], true, true);
                   SelectedDaoCangSlots[1].gameObject.SetActive(true);
                   DaoCangList[(int)currentSelectingFaShu.DaoCangCosts[1].Wuxing.currentWuXing].DaoCang -=
                       currentSelectingFaShu.DaoCangCosts[1].DaoCang;
                   
                   SelectedDaoCangSlots[3].SetUpDaoCangSlotUI(currentSelectingFaShu.DaoCangCosts[2], true, true);
                   SelectedDaoCangSlots[3].gameObject.SetActive(true);
                   DaoCangList[(int)currentSelectingFaShu.DaoCangCosts[2].Wuxing.currentWuXing].DaoCang -=
                       currentSelectingFaShu.DaoCangCosts[2].DaoCang;
                   
                   while (LoopMaxCount <1000)
                   {
                       LoopMaxCount++;
                       var randNum = UnityEngine.Random.Range(0, 5);
                       if (DaoCangList[randNum].DaoCang < currentSelectingFaShu.SameCost) continue;
                       DaoCangList[randNum].DaoCang-= currentSelectingFaShu.SameCost;
                       SelectedDaoCangSlots[4].SetUpDaoCangSlotUI(DaoCangList[randNum], 0, currentSelectingFaShu.SameCost, true, false);
                       SelectedDaoCangSlots[4].gameObject.SetActive(true);
                       break;
                   }
                   break;
               case 5:
                   SelectedDaoCangSlots[0].SetUpDaoCangSlotUI(currentSelectingFaShu.DaoCangCosts[0], true, true);
                   SelectedDaoCangSlots[0].gameObject.SetActive(true);
                   DaoCangList[(int)currentSelectingFaShu.DaoCangCosts[0].Wuxing.currentWuXing].DaoCang -=
                       currentSelectingFaShu.DaoCangCosts[0].DaoCang;
                   
                   SelectedDaoCangSlots[1].SetUpDaoCangSlotUI(currentSelectingFaShu.DaoCangCosts[1], true, true);
                   SelectedDaoCangSlots[1].gameObject.SetActive(true);
                   DaoCangList[(int)currentSelectingFaShu.DaoCangCosts[1].Wuxing.currentWuXing].DaoCang -=
                       currentSelectingFaShu.DaoCangCosts[1].DaoCang;
                   
                   SelectedDaoCangSlots[2].SetUpDaoCangSlotUI(currentSelectingFaShu.DaoCangCosts[2], true, true);
                   SelectedDaoCangSlots[2].gameObject.SetActive(true);
                   DaoCangList[(int)currentSelectingFaShu.DaoCangCosts[2].Wuxing.currentWuXing].DaoCang -=
                       currentSelectingFaShu.DaoCangCosts[2].DaoCang;
                   
                   SelectedDaoCangSlots[3].SetUpDaoCangSlotUI(currentSelectingFaShu.DaoCangCosts[3], true, true);
                   SelectedDaoCangSlots[3].gameObject.SetActive(true);
                   DaoCangList[(int)currentSelectingFaShu.DaoCangCosts[3].Wuxing.currentWuXing].DaoCang -=
                       currentSelectingFaShu.DaoCangCosts[3].DaoCang;
                   
                   while (LoopMaxCount < 1000)
                   {
                       LoopMaxCount++;
                       var randNum = UnityEngine.Random.Range(0, 5);
                       if (DaoCangList[randNum].DaoCang < currentSelectingFaShu.SameCost) continue;
                       DaoCangList[randNum].DaoCang-= currentSelectingFaShu.SameCost;
                       SelectedDaoCangSlots[4].SetUpDaoCangSlotUI(DaoCangList[randNum], 0, currentSelectingFaShu.SameCost, true, false);
                       SelectedDaoCangSlots[4].gameObject.SetActive(true);
                       break;
                   }
                   break;
               case 6:
                   SelectedDaoCangSlots[0].SetUpDaoCangSlotUI(currentSelectingFaShu.DaoCangCosts[0], true, true);
                   SelectedDaoCangSlots[0].gameObject.SetActive(true);
                   DaoCangList[(int)currentSelectingFaShu.DaoCangCosts[0].Wuxing.currentWuXing].DaoCang -=
                       currentSelectingFaShu.DaoCangCosts[0].DaoCang;
                   SelectedDaoCangSlots[1].SetUpDaoCangSlotUI(currentSelectingFaShu.DaoCangCosts[1], true, true);
                   SelectedDaoCangSlots[1].gameObject.SetActive(true);
                   DaoCangList[(int)currentSelectingFaShu.DaoCangCosts[1].Wuxing.currentWuXing].DaoCang -=
                       currentSelectingFaShu.DaoCangCosts[1].DaoCang;
                   
                   SelectedDaoCangSlots[2].SetUpDaoCangSlotUI(currentSelectingFaShu.DaoCangCosts[2], true, true);
                   SelectedDaoCangSlots[2].gameObject.SetActive(true);
                   DaoCangList[(int)currentSelectingFaShu.DaoCangCosts[2].Wuxing.currentWuXing].DaoCang -=
                       currentSelectingFaShu.DaoCangCosts[2].DaoCang;
                   
                   SelectedDaoCangSlots[3].SetUpDaoCangSlotUI(currentSelectingFaShu.DaoCangCosts[3], true, true);
                   SelectedDaoCangSlots[3].gameObject.SetActive(true);
                   DaoCangList[(int)currentSelectingFaShu.DaoCangCosts[3].Wuxing.currentWuXing].DaoCang -=
                       currentSelectingFaShu.DaoCangCosts[3].DaoCang;
                   
                   SelectedDaoCangSlots[4].SetUpDaoCangSlotUI(currentSelectingFaShu.DaoCangCosts[4], true, true);
                   SelectedDaoCangSlots[4].gameObject.SetActive(true);
                   DaoCangList[(int)currentSelectingFaShu.DaoCangCosts[4].Wuxing.currentWuXing].DaoCang -=
                       currentSelectingFaShu.DaoCangCosts[4].DaoCang;
                   
                   while (LoopMaxCount <1000)
                   {
                       LoopMaxCount++;
                       var randNum = UnityEngine.Random.Range(0, 5);
                       if (DaoCangList[randNum].DaoCang < currentSelectingFaShu.SameCost) continue;
                       foreach (var DaoCangSlot in SelectedDaoCangSlots.Where(DaoCangSlot => DaoCangList[randNum].Wuxing.currentWuXing == DaoCangSlot.wuxingDaoCang.Wuxing.currentWuXing))
                       {
                           DaoCangList[randNum].DaoCang-= currentSelectingFaShu.SameCost;
                           DaoCangSlot.UpdateDaoCangSlotUI(SelectedDaoCangSlots[randNum].wuxingDaoCang.DaoCang, currentSelectingFaShu.SameCost);
                           break;
                       }
                       break;
                   }
                   break;
            }
        }
        else
        {
            switch (ActiveSelectedDaoCangSlot)
            {
                case 0:
                    break;
                 case 1:
                    SelectedDaoCangSlots[2].SetUpDaoCangSlotUI(currentSelectingFaShu.DaoCangCosts[0], true, true);
                    SelectedDaoCangSlots[2].gameObject.SetActive(true);
                    DaoCangList[(int)currentSelectingFaShu.DaoCangCosts[0].Wuxing.currentWuXing].DaoCang -=
                        currentSelectingFaShu.DaoCangCosts[0].DaoCang;
                    break;
                case 2:
                    SelectedDaoCangSlots[1].SetUpDaoCangSlotUI(currentSelectingFaShu.DaoCangCosts[0], true, true);
                    SelectedDaoCangSlots[1].gameObject.SetActive(true);
                    DaoCangList[(int)currentSelectingFaShu.DaoCangCosts[0].Wuxing.currentWuXing].DaoCang -=
                        currentSelectingFaShu.DaoCangCosts[0].DaoCang;
                    
                    SelectedDaoCangSlots[3].SetUpDaoCangSlotUI(currentSelectingFaShu.DaoCangCosts[1], true, true);
                    SelectedDaoCangSlots[3].gameObject.SetActive(true);
                    DaoCangList[(int)currentSelectingFaShu.DaoCangCosts[1].Wuxing.currentWuXing].DaoCang -=
                        currentSelectingFaShu.DaoCangCosts[1].DaoCang;
                    break;
                case 3:
                    SelectedDaoCangSlots[1].gameObject.SetActive(true);
                    SelectedDaoCangSlots[1].SetUpDaoCangSlotUI(currentSelectingFaShu.DaoCangCosts[0], true, true);
                    DaoCangList[(int)currentSelectingFaShu.DaoCangCosts[0].Wuxing.currentWuXing].DaoCang -=
                        currentSelectingFaShu.DaoCangCosts[0].DaoCang;
                    
                    SelectedDaoCangSlots[2].SetUpDaoCangSlotUI(currentSelectingFaShu.DaoCangCosts[1], true, true);
                    SelectedDaoCangSlots[2].gameObject.SetActive(true);
                    DaoCangList[(int)currentSelectingFaShu.DaoCangCosts[1].Wuxing.currentWuXing].DaoCang -=
                        currentSelectingFaShu.DaoCangCosts[1].DaoCang;
                    
                    SelectedDaoCangSlots[3].SetUpDaoCangSlotUI(currentSelectingFaShu.DaoCangCosts[2], true, true);
                    SelectedDaoCangSlots[3].gameObject.SetActive(true);
                    DaoCangList[(int)currentSelectingFaShu.DaoCangCosts[2].Wuxing.currentWuXing].DaoCang -=
                        currentSelectingFaShu.DaoCangCosts[2].DaoCang;
                    break;
                case 4:
                    SelectedDaoCangSlots[0].SetUpDaoCangSlotUI(currentSelectingFaShu.DaoCangCosts[0], true, true);
                    SelectedDaoCangSlots[0].gameObject.SetActive(true);
                    DaoCangList[(int)currentSelectingFaShu.DaoCangCosts[0].Wuxing.currentWuXing].DaoCang -=
                        currentSelectingFaShu.DaoCangCosts[0].DaoCang;
                    
                    SelectedDaoCangSlots[1].SetUpDaoCangSlotUI(currentSelectingFaShu.DaoCangCosts[1], true, true);
                    SelectedDaoCangSlots[1].gameObject.SetActive(true);
                    DaoCangList[(int)currentSelectingFaShu.DaoCangCosts[1].Wuxing.currentWuXing].DaoCang -=
                        currentSelectingFaShu.DaoCangCosts[1].DaoCang;
                    
                    SelectedDaoCangSlots[3].SetUpDaoCangSlotUI(currentSelectingFaShu.DaoCangCosts[2], true, true);
                    SelectedDaoCangSlots[3].gameObject.SetActive(true);
                    DaoCangList[(int)currentSelectingFaShu.DaoCangCosts[2].Wuxing.currentWuXing].DaoCang -=
                        currentSelectingFaShu.DaoCangCosts[2].DaoCang;
                    
                    SelectedDaoCangSlots[4].SetUpDaoCangSlotUI(currentSelectingFaShu.DaoCangCosts[3], true, true);
                    SelectedDaoCangSlots[4].gameObject.SetActive(true);
                    DaoCangList[(int)currentSelectingFaShu.DaoCangCosts[3].Wuxing.currentWuXing].DaoCang -=
                        currentSelectingFaShu.DaoCangCosts[3].DaoCang;
                    break;
                case 5:
                    SelectedDaoCangSlots[0].SetUpDaoCangSlotUI(currentSelectingFaShu.DaoCangCosts[0], true, true);
                    SelectedDaoCangSlots[0].gameObject.SetActive(true);
                    DaoCangList[(int)currentSelectingFaShu.DaoCangCosts[0].Wuxing.currentWuXing].DaoCang -= currentSelectingFaShu.DaoCangCosts[0].DaoCang;
                    
                    SelectedDaoCangSlots[1].SetUpDaoCangSlotUI(currentSelectingFaShu.DaoCangCosts[1], true, true);
                    SelectedDaoCangSlots[1].gameObject.SetActive(true);
                    DaoCangList[(int)currentSelectingFaShu.DaoCangCosts[1].Wuxing.currentWuXing].DaoCang -=
                        currentSelectingFaShu.DaoCangCosts[1].DaoCang;
                    
                    SelectedDaoCangSlots[2].SetUpDaoCangSlotUI(currentSelectingFaShu.DaoCangCosts[2], true, true);
                    SelectedDaoCangSlots[2].gameObject.SetActive(true);
                    DaoCangList[(int)currentSelectingFaShu.DaoCangCosts[2].Wuxing.currentWuXing].DaoCang -=
                        currentSelectingFaShu.DaoCangCosts[2].DaoCang;
                    
                    SelectedDaoCangSlots[3].SetUpDaoCangSlotUI(currentSelectingFaShu.DaoCangCosts[3], true, true);
                    SelectedDaoCangSlots[3].gameObject.SetActive(true);
                    DaoCangList[(int)currentSelectingFaShu.DaoCangCosts[3].Wuxing.currentWuXing].DaoCang -=
                        currentSelectingFaShu.DaoCangCosts[3].DaoCang;
                    
                    SelectedDaoCangSlots[4].SetUpDaoCangSlotUI(currentSelectingFaShu.DaoCangCosts[4], true, true);
                    SelectedDaoCangSlots[4].gameObject.SetActive(true);
                    DaoCangList[(int)currentSelectingFaShu.DaoCangCosts[4].Wuxing.currentWuXing].DaoCang -=
                        currentSelectingFaShu.DaoCangCosts[4].DaoCang;
                    break;
            }
        }

        SetPromotionOrCounterText();
        PotentialDaoCangSlots[0].SetUpDaoCangSlotUI(DaoCangList[0], false, false);
        PotentialDaoCangSlots[1].SetUpDaoCangSlotUI(DaoCangList[1], false, false);
        PotentialDaoCangSlots[2].SetUpDaoCangSlotUI(DaoCangList[2], false, false);
        PotentialDaoCangSlots[3].SetUpDaoCangSlotUI(DaoCangList[3], false, false);
        PotentialDaoCangSlots[4].SetUpDaoCangSlotUI(DaoCangList[4], false, false);
        EndTurnButton.gameObject.SetActive(false);
        ConfirmButton.gameObject.SetActive(true);
        CancelButton.gameObject.SetActive(true);
    }

    private void SetPromotionOrCounterText()
    {
        Promotion_CounterSign = CheckIsPromotionOrCounter();
        switch (Promotion_CounterSign)
        {
            case 0:
                Promotion_CounterText.gameObject.SetActive(false);
                break;
            case 1:
                Promotion_CounterText.text = "相生";
                Promotion_CounterText.color = Color.green;
                Promotion_CounterText.gameObject.SetActive(true);
                break;
            case -1:
                Promotion_CounterText.text = "相克";
                Promotion_CounterText.color = Color.red;
                Promotion_CounterText.gameObject.SetActive(true);
                break;
        }
    }
    
    /// <summary>
    /// 检测所选择的五行道藏是否相生相克
    /// </summary>
    /// <param name="WuxingDaoCangs"></param>
    /// <returns>-1为相克，0为无效，1为相生</returns>
    private int CheckIsPromotionOrCounter()
    {
        var WuxingDaoCangs = (from DaoCang in SelectedDaoCangSlots where DaoCang.isActiveAndEnabled select DaoCang.wuxingDaoCang).ToList();
        switch (WuxingDaoCangs.Count)
        {
            case 0:
            case 1:
                return 0;
            case 2:
                if (WuxingDaoCangs[0].Wuxing.promoteWuXing == WuxingDaoCangs[1].Wuxing.currentWuXing ||
                    WuxingDaoCangs[1].Wuxing.promoteWuXing == WuxingDaoCangs[0].Wuxing.currentWuXing)
                {
                    return 1;
                }
                if (WuxingDaoCangs[0].Wuxing.counterWuXing == WuxingDaoCangs[1].Wuxing.currentWuXing ||
                    WuxingDaoCangs[1].Wuxing.counterWuXing == WuxingDaoCangs[0].Wuxing.currentWuXing)
                {
                    {
                        return -1;
                    }
                }
                return 0;
            case 3:
            case 4:
            case 5:
                var value = 0;
                for (var i = 0; i < WuxingDaoCangs.Count-1; i++)
                {
                    if (WuxingDaoCangs[i].Wuxing.promoteWuXing == WuxingDaoCangs[i + 1].Wuxing.currentWuXing)
                    {
                        value += 1;
                    }
                    if (WuxingDaoCangs[i].Wuxing.counterWuXing == WuxingDaoCangs[i + 1].Wuxing.currentWuXing)
                    {
                        value += -1;
                    }
                }
                if (value == WuxingDaoCangs.Count - 1) return 1;
                if (value == -WuxingDaoCangs.Count + 1) return -1;
                return 0;
        }
        return 0;
    }

    private void ResetCurrentDaoCang(CharacterData characterData, List<DaoCangSlotUI> WuxingDaoCangs)
    {
        WuxingDaoCangs[0].UpdateDaoCangSlotUI(characterData.currentMetalDaocang, 0);
        WuxingDaoCangs[1].UpdateDaoCangSlotUI(characterData.currentWoodDaocang, 0);
        WuxingDaoCangs[2].UpdateDaoCangSlotUI(characterData.currentWaterDaocang, 0);
        WuxingDaoCangs[3].UpdateDaoCangSlotUI(characterData.currentFireDaocang, 0);
        WuxingDaoCangs[4].UpdateDaoCangSlotUI(characterData.currentEarthDaocang, 0);
    }

    public void UpdateDaoCangCost()
    {
        foreach (var DaoCang in SelectedDaoCangSlots)
        {
            switch (DaoCang.wuxingDaoCang.Wuxing.currentWuXing)
            {
                case WuXing.锐金:
                    currentCharacter.CharacterData.currentMetalDaocang -= DaoCang.wuxingDaoCang.DaoCang + DaoCang.sameDaoCangCost;
                    break;
                case WuXing.灵木:
                    currentCharacter.CharacterData.currentWoodDaocang -= DaoCang.wuxingDaoCang.DaoCang + DaoCang.sameDaoCangCost;
                    break;
                case WuXing.弱水:
                    currentCharacter.CharacterData.currentWaterDaocang -= DaoCang.wuxingDaoCang.DaoCang + DaoCang.sameDaoCangCost;
                    break;
                case WuXing.离火:
                    currentCharacter.CharacterData.currentFireDaocang -= DaoCang.wuxingDaoCang.DaoCang + DaoCang.sameDaoCangCost;
                    break;
                case WuXing.厚土:
                    currentCharacter.CharacterData.currentEarthDaocang -= DaoCang.wuxingDaoCang.DaoCang + DaoCang.sameDaoCangCost;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public void SwapSelectingDaoCangSlot(DaoCangSlotUI originalSlot, DaoCangSlotUI targetSlot)
    {
        if (originalSlot.isSelectingSlot)
        {
            var tempSlot = Instantiate(targetSlot);
            targetSlot.SetUpDaoCangSlotUI(originalSlot.wuxingDaoCang, originalSlot.wuxingDaoCang.DaoCang,
                originalSlot.sameDaoCangCost, originalSlot.isSelectingSlot, originalSlot.isRequireDaoCang);
            originalSlot.SetUpDaoCangSlotUI(tempSlot.wuxingDaoCang, tempSlot.wuxingDaoCang.DaoCang,
                tempSlot.sameDaoCangCost, tempSlot.isSelectingSlot, tempSlot.isRequireDaoCang);
        }
        else
        {
            if(targetSlot.isRequireDaoCang) return;
            if (originalSlot.wuxingDaoCang.DaoCang >= targetSlot.wuxingDaoCang.DaoCang)
            {
                var PotentialSlot = PotentialDaoCangSlots[(int)targetSlot.wuxingDaoCang.Wuxing.currentWuXing];
                PotentialSlot.UpdateDaoCangSlotUI(PotentialSlot.wuxingDaoCang.DaoCang + targetSlot.sameDaoCangCost, 0);
                originalSlot.UpdateDaoCangSlotUI(originalSlot.wuxingDaoCang.DaoCang - targetSlot.sameDaoCangCost, 0);
                targetSlot.SetUpDaoCangSlotUI(new WuxingDaoCang { Wuxing = originalSlot.wuxingDaoCang.Wuxing },
                    targetSlot.wuxingDaoCang.DaoCang, targetSlot.sameDaoCangCost, targetSlot.isSelectingSlot,
                    targetSlot.isRequireDaoCang);
            }
        }
        SetPromotionOrCounterText();
    }

    public void ChangeFaShuSameCostSlots(DaoCangSlotUI ChangeDaoCangSlot)
    {
        if(currentSelectingFaShu ==null) return;
        if (currentSelectingFaShu.SameCost <= 0 ||
            ChangeDaoCangSlot.wuxingDaoCang.DaoCang < currentSelectingFaShu.SameCost) return;
        if (ActiveSelectedDaoCangSlot >= 6)
        {
            foreach (var DaoCangSlot in SelectedDaoCangSlots)
            {
                if (DaoCangSlot.sameDaoCangCost == 0 || !DaoCangSlot.isActiveAndEnabled) continue;
                var PotentialSlot = PotentialDaoCangSlots[(int)DaoCangSlot.wuxingDaoCang.Wuxing.currentWuXing];
                PotentialSlot.UpdateDaoCangSlotUI(PotentialSlot.wuxingDaoCang.DaoCang + DaoCangSlot.sameDaoCangCost,
                    0);
                ChangeDaoCangSlot.UpdateDaoCangSlotUI(
                    ChangeDaoCangSlot.wuxingDaoCang.DaoCang - DaoCangSlot.sameDaoCangCost, 0);
                DaoCangSlot.SetUpDaoCangSlotUI(new WuxingDaoCang { Wuxing = ChangeDaoCangSlot.wuxingDaoCang.Wuxing },
                    DaoCangSlot.wuxingDaoCang.DaoCang, DaoCangSlot.sameDaoCangCost, DaoCangSlot.isSelectingSlot,
                    DaoCangSlot.isRequireDaoCang);
                break;
            }
        }
        else
        {
            foreach (var DaoCangSlot in SelectedDaoCangSlots)
            {
                if (DaoCangSlot.isRequireDaoCang || !DaoCangSlot.isActiveAndEnabled) continue;
                var PotentialSlot = PotentialDaoCangSlots[(int)DaoCangSlot.wuxingDaoCang.Wuxing.currentWuXing];
                PotentialSlot.UpdateDaoCangSlotUI(PotentialSlot.wuxingDaoCang.DaoCang + currentSelectingFaShu.SameCost,
                    0);
                ChangeDaoCangSlot.UpdateDaoCangSlotUI(
                    ChangeDaoCangSlot.wuxingDaoCang.DaoCang - currentSelectingFaShu.SameCost, 0);
                DaoCangSlot.SetUpDaoCangSlotUI(new WuxingDaoCang { Wuxing = ChangeDaoCangSlot.wuxingDaoCang.Wuxing },
                    DaoCangSlot.wuxingDaoCang.DaoCang, DaoCangSlot.sameDaoCangCost, DaoCangSlot.isSelectingSlot,
                    DaoCangSlot.isRequireDaoCang);
                break;
            }
        }
        SetPromotionOrCounterText();
    }
    
    public Sprite GetWuXingIcon(WuXing WuXing)
    {
        return WuXing switch
        {
            WuXing.锐金 => MetalDaoCangIcon,
            WuXing.灵木 => WoodDaoCangIcon,
            WuXing.弱水 => WaterDaoCangIcon,
            WuXing.离火 => FireDaoCangIcon,
            WuXing.厚土 => EarthDaoCangIcon,
            _ => throw new ArgumentOutOfRangeException(nameof(WuXing), WuXing, null)
        };
    }
}
