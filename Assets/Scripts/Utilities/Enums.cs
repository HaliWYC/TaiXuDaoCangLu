#region BasicConstant

public enum WuXing
{
    锐金,灵木,弱水,离火,厚土
}

public enum ShuXing
{
    甲木,乙木,丙火,丁火,戊土,己土,庚金,辛金,壬水,癸水,混沌,魔道,妖道
}

public enum MiniJingjieLevel
{
    前期,中期,后期,巅峰,大圆满
}
public enum JingjieLevel
{
    凡人,炼气,筑基,结丹,元婴,化神
}

public enum MiniQuality
{
    初级,中级,高级,终极
}

public enum QualityType
{
    残,凡,人,地,天
}

#endregion

#region XiuLian

#region GongFa

public enum GongFaType
{
    Main,Subsidiary
}

#endregion


#region FaShu

public enum FaShuType
{
    Normal, ShenShi, ShenTong, MiShu
}

public enum FaShuPurposeType
{
    Attack,Defense,Abilities
}

public enum FaShuTargetType
{
    Self,Enemy,Ally
}
public enum FaShuTargetRangeType
{
    Single,Range,All
}

public enum FaShuDurationType
{
    Once,Sustainable
}

#endregion

#region ShenTong

public enum ShenTongDurationType
{
    Once,Sustainable,Permanent
}

#endregion

#endregion

public enum PropertyType
{
    MaxAge,MaxHealth,MaxMana,Attack,Reaction,Speed,MaxMovementPerTurn,MaxDaocangPerTurn,
    MetalLingGen,WoodLingGen,WaterLingGen,FireLingGen,EarthLingGen,
    ShenShi,ShenShiStrength,
    Strength,Fitness,Eloquence,Wisdom,Luck,Comprehension,
    MainGongFaBasicSpeed,MainGongFaAdditionalSpeed,SubGongFaBasicSpeed
}

#region Map
public enum GridType
{
    CanDig,CanDrop,CanDestroy,CanAttack,CanLeave,Obstacle
}

public enum PlaceType
{
    Mountain/*无名山*/,Town/*小镇*/,ChengChi/*城池*/,ZongMen/*宗门*/,
    Pass/*关卡*/,Maze/*迷宫*/,Fortress/*要塞*/,Cave/*洞穴*/, 
    Teleport/*传送阵*/
}


#endregion

public enum GameSeasons
{
    Spring,Summer,Autumn,Winter
}