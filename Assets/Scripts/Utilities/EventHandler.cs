using System;
using System.Collections.Generic;
using TXDCL.Character;
using TXDCL.Map;
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
    
    public static event Action<List<CharacterBase>> NewCharactersEnterCombatEvent;
    public static void CallNewCharactersEnterCombatEvent(List<CharacterBase> characters)
    {
        NewCharactersEnterCombatEvent?.Invoke(characters);
    }

    #endregion
}