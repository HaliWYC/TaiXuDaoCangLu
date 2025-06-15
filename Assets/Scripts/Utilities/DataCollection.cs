using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Wuxing
{
    public WuXing currentWuXing;
    public WuXing promoteWuXing=> currentWuXing switch {
        WuXing.锐金=>WuXing.弱水,
        WuXing.灵木=>WuXing.离火,
        WuXing.弱水=>WuXing.灵木,
        WuXing.离火=>WuXing.厚土,
        _=>WuXing.锐金
    };
    public WuXing counterWuXing=> currentWuXing switch {
        WuXing.锐金=>WuXing.灵木,
        WuXing.灵木=>WuXing.厚土,
        WuXing.弱水=>WuXing.离火,
        WuXing.离火=>WuXing.锐金,
        _=>WuXing.弱水
    };
}

[System.Serializable]
public class Property
{
    public PropertyType propertyType;
    public float value;
}

public class TileProperty
{
    public Vector2Int tileCoordinates;
    public GridType gridType;
    public bool boolTypeValue;
}

[System.Serializable]
public class Jingjie
{
    public int ID => (int)miniJingjieLevel + (int)JingjieLevel * 5;
    public MiniJingjieLevel miniJingjieLevel;
    public JingjieLevel JingjieLevel;
    public JingjieData JingjieData;
}

[System.Serializable]
public class JingjieData:ScriptableObject
{
    public int NextEXP;//下一级升级所需经验
    public int MaxAge;//寿元
    public int maxHealth;//气血
    public int maxMana;//最大法力
    public int attack;//攻击
    public int maxMovementPerTurn;//每回合移动力
    public int maxDaocangPerTurn;//每回合总道藏获取量
}

// [System.Serializable]
// public class WuXingInfor : Wuxing
// {
//     public int minAttack;
//     public int maxAttack;
//     public int minDefence;
//     public int maxDefence;
// }
// public class WuxingMultiAttack
// {
//     public List<Wuxing> wuxings;
//     public int Value;
// }
// public class WuxingAttack:Wuxing
// {
//     public int Value;
// }
// public class WuxingMultiDefense
// {
//     public List<Wuxing> wuxings;
//     public int Value;
// }
// public class WuxingDefense:Wuxing
// {
//     public int Value;
// }
