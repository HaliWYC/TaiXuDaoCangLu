using System;
using UnityEngine;

namespace TXDCL.Time
{
    public class TimeManager : Singleton<TimeManager>
    {
        private int gameSeconds, gameMinutes, gameHours, gameDay, gameMonth, gameYear;
        private GameSeasons gameSeason = GameSeasons.Spring;
        public bool gameClockPause, isCombat;
        private float tikTime;
        private int combatTime;
        public TimeSpan currentGameTime => new (gameHours, gameMinutes, gameSeconds);
        protected override void Awake()
        {
            base.Awake();
            NewGameTime();
        }

        private void Start()
        {
            EventHandler.CallGameHourEvent(gameHours);
            EventHandler.CallGameDateEvent(gameDay, gameMonth, gameYear);
            EventHandler.CallGameSeasonEvent(gameSeason);
        }

        private void Update()
        {
            if (gameClockPause) return;
            tikTime += UnityEngine.Time.deltaTime;
            if (!(tikTime >= Settings.secondThreshold)) return;
            tikTime -= Settings.secondThreshold;
            if (Input.GetKey(KeyCode.T))
            {
                for (var i = 0; i < 60; i++)
                {
                    UpdateGameTime();
                }
            }

            if (isCombat)
            {
                combatTime++;
                if (combatTime >= Settings.combatTimeModifier)
                {
                    combatTime = 0;
                    UpdateGameTime();
                    return;
                }
            }

            UpdateGameTime();
        }

        private void NewGameTime()
        {
            gameSeconds = 0;
            gameMinutes = 0;
            gameHours = 2;
            gameDay = 1;
            gameMonth = 1;
            gameYear = 1;
            combatTime = 0;
            gameSeason = GameSeasons.Spring;
            gameClockPause = false;
            isCombat = false;
        }

        private void UpdateGameTime()
        {
            gameSeconds++;
            if (gameSeconds > Settings.secondHold)
            {
                gameSeconds = 0;
                gameMinutes++;
                //Debug.Log("Minutes: "+ gameMinutes+" Hours: "+ gameHours+" Days: "+ gameDay);
                if (gameMinutes > Settings.minuteHold)
                {
                    gameHours++;
                    gameMinutes = 0;
                    if (gameHours > Settings.hourHold)
                    {
                        gameDay++;
                        gameHours = 0;
                        if (gameDay > Settings.dayHold)
                        {
                            gameMonth++;
                            gameDay = 1;
                            if (gameMonth > Settings.monthHold)
                            {
                                gameSeason++;
                                gameMonth = 1 + (int)gameSeason * 3;
                                if ((int)gameSeason > Settings.seasonHold)
                                {
                                    gameYear++;
                                    gameSeason = 0;
                                }

                                EventHandler.CallGameSeasonEvent(gameSeason);
                            }
                        }

                        //TODO 结算自动吸取道藏
                        EventHandler.CallGameDateEvent(gameDay, gameMonth, gameYear);
                    }

                    EventHandler.CallGameHourEvent(gameHours);
                }
            }

        }
    }
}
