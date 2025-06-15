using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TimeUI : MonoBehaviour
{
    public RectTransform dayNightImage;
    public RectTransform clockParent;
    public Image seasonImage;
    public TextMeshProUGUI dateText;
    public Sprite[] seasonSprites;

    private List<GameObject> clockBlocks = new();

    private void Awake()
    {
        for (var i = 0; i < clockParent.childCount; i++)
        {
            clockBlocks.Add(clockParent.GetChild(i).gameObject);
            clockParent.GetChild(i).gameObject.SetActive(false);
        }
    }
    private void OnEnable()
    {
        EventHandler.gameHourEvent += OnGameHourEvent;
        EventHandler.gameSeasonEvent += OnGameSeasonEvent;
        EventHandler.gameDateEvent += OnGameDateEvent;
    }
    private void OnDisable()
    {
        EventHandler.gameHourEvent -= OnGameHourEvent;
        EventHandler.gameSeasonEvent -= OnGameSeasonEvent;
        EventHandler.gameDateEvent -= OnGameDateEvent;
    }
    
    private void OnGameHourEvent(int hour)
    {
        var index = hour / 2;
        for (var i = 0; i < clockBlocks.Count; i++)
        {
            clockBlocks[i].SetActive(i < index + 1);
        }
        SwitchDayNightImageRotation(hour);
    }

    private void SwitchDayNightImageRotation(int hour)
    {
        var target = new Vector3(0, 0, hour * 30 - 90);
        dayNightImage.DORotate(target, 2f, RotateMode.Fast);
    }
    private void OnGameDateEvent(int day, int month, int year)
    {
        var time = day / 10 != 1 ? day % 10 : day;
        var date = day.ToString();
        date += time switch
        {
            1 => "st",
            2 => "nd",
            3 => "rd",
            _ => "th"
        };
        date += " of ";
        switch (month)
        {
            case 1:
                date += "January";
                break;
            case 2:
                date += "February";
                break;
            case 3:
                date += "March";
                break;
            case 4:
                date += "April";
                break;
            case 5:
                date += "May";
                break;
            case 6:
                date += "June";
                break;
            case 7:
                date += "July";
                break;
            case 8:
                date += "August";
                break;
            case 9:
                date += "September";
                break;
            case 10:
                date += "October";
                break;
            case 11:
                date += "November";
                break;
            case 12:
                date += "December";
                break;
        }

        date += ", ";
        date += year.ToString();
        dateText.text = date;
    }
    
    private void OnGameSeasonEvent(GameSeasons season)
    {
        seasonImage.sprite = seasonSprites[(int)season];
    }
    
}
