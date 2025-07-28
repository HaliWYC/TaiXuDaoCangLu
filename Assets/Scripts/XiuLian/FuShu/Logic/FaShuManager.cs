using System;
using System.Collections.Generic;
using TXDCL.Character;
using TXDCL.XiuLian.FuShu;
using Unity.Cinemachine;
using UnityEngine;

public class FaShuManager : Singleton<FaShuManager>
{
    public FaShuDataList FaShuDataList;
    private Dictionary<int, FaShuData> FaShuDataDict = new();

    protected override void Awake()
    {
        base.Awake();
        InitializeData();
    }

    private void InitializeData()
    {
        if(FaShuDataList == null) return;
        foreach (var FaShu in FaShuDataList.FaShuDatas)
        {
            FaShuDataDict.Add(FaShu.ID, FaShu);
        }
    }
    public FaShuData GetFaShuData(int id)
    {
        return FaShuDataDict.GetValueOrDefault(id);
    }

    public bool CheckReleaseFaShuConditions(CharacterData characterData, FaShuData faShuData)
    {
        //检测法术冷却和准备时间
        if (faShuData.CurrentCoolDownTime > 0 || faShuData.currentPrepareTurns < faShuData.MaxPrepareTurns)
            return false;
        //检测法术基础消耗
        return faShuData.HealthCost < characterData.currentHealth &&
               faShuData.StaminaCost <= characterData.currentStamina &&
               faShuData.ManaCost <= characterData.currentMana && faShuData.JingShenLiCost <= characterData.JingShenLi;
        //TODO:后续用UI来检测道藏消耗
        //TODO:执行实际消耗
    }
    
    public void ExecuteFaShu(FaShuData FaShu,CharacterBase from, List<CharacterBase> targetCharacters)
    {
        if (targetCharacters.Count <= 0 || !CheckReleaseFaShuConditions(from.CharacterData, FaShu)) return;
        //TODO：判断一下法术的目标
        foreach (var character in targetCharacters)
        {
            foreach (var effects in FaShu.EffectDatas)
            {
                effects.OnEffectCreate(from, character);
            }
        }
    }

    // public bool CheckReleaseFaShuDaoCangCosts(CharacterData characterData, FaShuData faShuData)
    // {
    //     var enoughDaoCang = false;
    //     var enoughSameDaoCang = false;
    //     var enoughDiffDaoCang = false;
    //     //如果无明确需求要某几个属性道藏时则随机选取
    //     if (faShuData.DaoCangCosts.Count <= 0)
    //     {
    //         enoughDaoCang = true;
    //         enoughSameDaoCang = characterData.currentMetalDaocang >= faShuData.SameCost ||
    //                             characterData.currentWoodDaocang >= faShuData.SameCost ||
    //                             characterData.currentWaterDaocang >= faShuData.SameCost ||
    //                             characterData.currentFireDaocang >= faShuData.SameCost ||
    //                             characterData.currentEarthDaocang >= faShuData.SameCost;
    //         enoughDiffDaoCang =
    //             characterData.currentMetalDaocang + characterData.currentWoodDaocang +
    //             characterData.currentWaterDaocang + characterData.currentFireDaocang +
    //             characterData.currentEarthDaocang >= faShuData.DifCost;
    //     }
    //     else
    //     {
    //         var difTotalCost = 0;
    //         foreach (var cost in faShuData.DaoCangCosts)
    //         {
    //             switch (cost.Wuxing.currentWuXing)
    //             {
    //                 case WuXing.锐金:
    //                     if (characterData.currentMetalDaocang >= cost.DaoCang)
    //                     {
    //                         enoughDaoCang = true;
    //                         if(characterData.currentMetalDaocang >= cost.DaoCang+ faShuData.SameCost)
    //                         {
    //                             enoughSameDaoCang = true;
    //                         }
    //                     }
    //                     break;
    //                 case WuXing.灵木:
    //                     if (characterData.currentWoodDaocang < cost.DaoCang)
    //                     {
    //                         enoughDaoCang = false;
    //                         if(characterData.currentWoodDaocang >= cost.DaoCang+ faShuData.SameCost)
    //                         {
    //                             enoughSameDaoCang = true;
    //                         }
    //                     }
    //                     break;
    //                 case WuXing.弱水:
    //                     if (characterData.currentWaterDaocang < cost.DaoCang)
    //                     {
    //                         enoughDaoCang = false;
    //                         if(characterData.currentWaterDaocang >= cost.DaoCang+ faShuData.SameCost)
    //                         {
    //                             enoughSameDaoCang = true;
    //                         }
    //                     }
    //                     break;
    //                 case WuXing.离火:
    //                     if (characterData.currentFireDaocang < cost.DaoCang)
    //                     {
    //                         enoughDaoCang = false;
    //                         if(characterData.currentFireDaocang >= cost.DaoCang+ faShuData.SameCost)
    //                         {
    //                             enoughSameDaoCang = true;
    //                         }
    //                     }
    //                     break;
    //                 case WuXing.厚土:
    //                     if (characterData.currentEarthDaocang < cost.DaoCang)
    //                     {
    //                         enoughDaoCang = false;
    //                         if(characterData.currentEarthDaocang >= cost.DaoCang+ faShuData.SameCost)
    //                         {
    //                             enoughSameDaoCang = true;
    //                         }
    //                     }
    //                     break;
    //                 default:
    //                     throw new ArgumentOutOfRangeException();
    //             }
    //         }
    //     }
    // }
}
