using System;
using System.Collections.Generic;
using TXDCL.Character;
using TXDCL.Map;
using TXDCL.XiuLian.FuShu;
using UnityEngine;
public static class  EventHandler
{
    #region Time

    public static event Action<int> GameHourEvent;
    public static void CallGameHourEvent(int hour)
    {
        GameHourEvent?.Invoke(hour);
    }
    public static event Action<int,int,int> GameDateEvent;
    public static void CallGameDateEvent(int day,int month,int year)
    {
        GameDateEvent?.Invoke(day,month,year);
    }
    public static event Action<GameSeasons> GameSeasonEvent;
    public static void CallGameSeasonEvent(GameSeasons season)
    {
        GameSeasonEvent?.Invoke(season);
    }

    #endregion

    #region SceneManagement

    public static event Action<SceneData_SO, Vector3> SceneLoadedEvent;

    public static void CallSceneLoadedEvent(SceneData_SO mapData, Vector3 targetPos)
    {
        SceneLoadedEvent?.Invoke(mapData,targetPos);
    }
    public static event Action BeforeSceneLoadEvent;
    public static void CallBeforeSceneLoadEvent()
    {
        BeforeSceneLoadEvent?.Invoke();
    }
    public static event Action AfterSceneLoadEvent;
    public static void CallAfterSceneLoadEvent()
    {
        AfterSceneLoadEvent?.Invoke();
    }
    
    public static event Action<Vector3> MoveToPositionEvent;
    public static void CallMoveToPositionEvent(Vector3 targetPos)
    {
        MoveToPositionEvent?.Invoke(targetPos);
    }
    #endregion

    #region Combat
    public static event Action BeforeCombatBeginEvent;
    public static void CallBeforeCombatBeginEvent()
    {
        BeforeCombatBeginEvent?.Invoke();
    }
    public static event Action AfterCombatBeginEvent;
    public static void CallAfterCombatBeginEvent()
    {
        AfterCombatBeginEvent?.Invoke();
    }
    
    public static event Action<List<CharacterBase>> NewCharactersEnterCombatEvent;
    public static void CallNewCharactersEnterCombatEvent(List<CharacterBase> characters)
    {
        NewCharactersEnterCombatEvent?.Invoke(characters);
    }
    
    public static event Action<CharacterBase> CharacterTurnBeginEvent;
    public static void CallCharacterTurnBeginEvent(CharacterBase character)
    {
        CharacterTurnBeginEvent?.Invoke(character);
    }
    public static event Action<CharacterBase> CharacterTurnEndEvent;
    public static void CallCharacterTurnEndEvent(CharacterBase character)
    {
        CharacterTurnEndEvent?.Invoke(character);
    }

    public static event Action<FaShuData> AfterFaShuReleasedEvent;
    public static void CallAfterFaShuReleasedEvent(FaShuData faShuData)
    {
        AfterFaShuReleasedEvent?.Invoke(faShuData);
    }

    #endregion

    #region Effect

    public static event Action<CharacterBase,CharacterBase> OnEffectCreateEvent;
    public static void CallOnEffectCreateEvent(CharacterBase from, CharacterBase to)
    {
        OnEffectCreateEvent?.Invoke(from,to);
    }
    public static event Action OnEffectExecuteEvent;
    public static void CallOnEffectExecuteEvent()
    {
        OnEffectExecuteEvent?.Invoke();
    }
    public static event Action<CharacterBase> OnEffectEndEvent;
    public static void CallOnEffectEndEvent(CharacterBase currentTarget)
    {
        OnEffectEndEvent?.Invoke(currentTarget);
    }

    #endregion

    #region Inventory

    public static Action<CharacterBase> UpdateInventoryUIEvent;

    public static void CallUpdateInventoryUIEvent(CharacterBase character)
    {
        UpdateInventoryUIEvent?.Invoke(character);
    }
    
    #endregion
}