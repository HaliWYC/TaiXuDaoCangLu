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

    public void ExecuteFaShu(FaShuData FaShu,CharacterBase from, List<CharacterBase> targetCharacters)
    {
        if(targetCharacters.Count <=0) return;
        //TODO：判断一下法术的目标
        foreach (var character in targetCharacters)
        {
            foreach (var effects in FaShu.EffectDatas)
            {
                effects.OnEffectCreate(from, character);
            }
        }
    }
}
