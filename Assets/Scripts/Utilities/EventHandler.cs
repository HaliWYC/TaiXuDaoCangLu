using System;
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

    public static event Action<MapData_SO, Vector3> SceneLoadedEvent;

    public static void CallSceneLoadedEvent(MapData_SO mapData, Vector3 targetPos)
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
    
}