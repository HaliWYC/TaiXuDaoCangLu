using UnityEngine;

public class Settings 
{
    public const float FadeDuration = 0.35f;
    public const float TargetAlpha = 0.8f;

    [Header("Combat")] 
    public const float WuxingCounterWuxing = 1.5f;
    public const float WuxingCounteredWuxing= 0.5f;
    public const float WuxingPromote = 1.2f;
    public const float WuxingCounter = 0.8f;
    public const float Defense = 217f;
    public const float WuxingDefense = 217f;
    public const float Accuracy = 217f;
    public const float Abrasion = 0.05f;
    public const float Crit = 217f;
    public const float Fatality = 217f;
    public const float Tenacity = 217f;
    public const float HuiXin = 217f;
    public const float HuiYi = 217f;
    public const float Dodge = 217f;
    public const float Penetrate = 217f;
    public const float PerfectAccuracy = 217f;
    public const float WoundCreate = 217f;
    public const float Counter = 217f;

    [Header("Level")] 
    public const float Fanren = 0.8f;
    public const float Lianqi = 1f;
    public const float Zhuji = 1.2f;
    public const float Jiedan = 1.5f;
    public const float Yuanying = 1.8f;
    public const float Huashen = 2f;

    [Header("Time")] 
    public const float secondThreshold = 0.02f;//数值越小时间越快
    public const float combatTimeModifier = 0.1f;//进入战斗后时间放慢倍率
    public const int secondHold = 29;// 29秒后为1分钟
    public const int minuteHold = 29;// 29分钟后为1时辰
    public const int hourHold = 11;// 11时辰后为1天
    public const int dayHold = 30;// 30天后为1个月
    public const int monthHold = 3;// 3个月后为1个季
    public const int seasonHold = 4;// 4个季后为1年
}
