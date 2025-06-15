using System;
using UnityEngine;
public static class  EventHandler
{
    public static event Action<int> gameHourEvent;

    public static void CallGameHourEvent(int hour)
    {
        gameHourEvent?.Invoke(hour);
    }
    public static event Action<int,int,int> gameDateEvent;

    public static void CallGameDateEvent(int day,int month,int year)
    {
        gameDateEvent?.Invoke(day,month,year);
    }
    public static event Action<GameSeasons> gameSeasonEvent;

    public static void CallGameSeasonEvent(GameSeasons season)
    {
        gameSeasonEvent?.Invoke(season);
    }
    
}
