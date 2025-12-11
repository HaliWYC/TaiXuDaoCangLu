#region BasicConstant

public enum WuXing
{
    锐金,灵木,弱水,离火,厚土
}

public enum ShuXing
{
    甲木,乙木,丙火,丁火,戊土,己土,庚金,辛金,壬水,癸水,阳雷,阴雷,混沌,魔道,妖道
}

public enum MiniJingjieLevel
{
    前期, 中期, 后期, 巅峰, 大圆满
}
public enum JingjieLevel
{
    凡人, 炼气, 筑基, 结丹, 元婴, 化神
}

public enum MiniRarity
{
    初级, 中级, 高级, 终极
}

public enum Rarity
{
    残缺, 凡级, 人级, 地级, 天级, 特殊
}

public enum ItemRarity
{
    一品,二品,三品,四品,五品,六品,七品,八品,九品,极品
}

#endregion

#region XiuLian

#region GongFa

public enum GongFaType//功法类型：元婴前仅能有一个主功法，元婴后元婴可再修炼一门功法
{
    Main,Subsidiary
}

#endregion
#region FaShu

public enum FaShuType//法术类型，如常规法术、凡人武功或体质类法术、神识法术等
{
    法术, 武功, 神识, 法宝
}
public enum FaShuTarget//法术释放目标
{
    Self, Enemy, Ally, Any
}

public enum FaShuDuration//法术施法时间，如单次释放或需要持续施法
{
    Once,Sustainable
}

public enum FaShuLevel//随着对法术的领悟进行突破，领悟需要消耗时间以及精力
{
    零, 壹, 贰, 叁, 肆, 伍, 陆, 柒, 捌, 玖
}
public enum FaShuProficiency//随着法术有效使用次数进行叠加，速率可受多方面影响，如功法、Buff等
{
    初窥门径, 一知半解, 半生不熟, 融会贯通, 游刃有余, 炉火纯青, 登峰造极, 臻至化境, 出神入化, 太虚之境
}

public enum FaShuDerivativeTrackType
{
    ReleaserMobile/*从施法者位置移动至目标位置*/,ReleaserFixed/*从施法者位置不移动*/,SpecificMobile/*从特定位置移动至目标位置*/,SpecificFixed/*从特定位置不移动*/
}
#endregion
#endregion

#region Effect

public enum EffectTarget
{
    Self,Enemy,Ally,Any
}
public enum EffectDuration
{
    Once,Sustainable,Permanent
}

#endregion

#region Inventory

public enum PropertyType
{
    MaxAge,MaxHealth,MaxMana,Attack,Reaction,Speed,MaxMovementPerTurn,MaxDaocangPerTurn,
    MetalLingGen,WoodLingGen,WaterLingGen,FireLingGen,EarthLingGen,
    ShenShi,ShenShiStrength,
    Strength,Fitness,Eloquence,Wisdom,Luck,Comprehension,
    MainGongFaBasicSpeed,MainGongFaAdditionalSpeed,SubGongFaBasicSpeed
}

public enum ItemType
{
    法宝,消耗品,任务物品,其他物品,储物袋
}

public enum FaBaoType
{
    武器,防具,饰品,挂件,坐骑
}
public enum ConsumablesType
{
    丹药,符箓,丹方,炼器图纸,制符图纸,阵法图纸
}
public enum QuestItemType
{
    
}
public enum OtherItemType
{
    草药,炼器材料,制符材料,常规材料
}

public enum StorageBagType
{
    法宝,消耗品,任务物品,其他物品,万能
}

public enum CaoYaoStateType
{
    未处理,块状,粉状,液状
}

#endregion

#region Combat

public enum CharacterFactionType
{
    Player,PlayerAlly,NPC,Enemy
}

#endregion

#region Map
public enum GridType
{
    CanDig,CanDrop,CanDestroy,CanAttack,CanLeave,Obstacle
}

public enum PlaceType
{
    Mountain/*无名山：包含矿藏、草药，小概率埋藏宝藏*/,Town/*小镇：集市可进行交易，旅店可供休息*/,
    ChengChi/*城池：拍卖行、贸易中心进行交易、洞府可供修炼、休息，主建筑可领取悬赏及炼丹、炼器、制符、阵法房*/,
    ZongMen/*宗门：功法、法术阁学习功法及法术，洞府可供修炼，药园采集草药、山脉开采矿藏，主建筑领取悬赏，广场进行切磋，炼丹、炼器、制符，阵法房供精进*/, 
    Pass/*关卡*/,Maze/*迷宫*/,Fortress/*要塞*/,Cave/*洞穴*/, 
    Teleport/*传送阵*/
}


#endregion
public enum GameSeasons
{
    Spring,Summer,Autumn,Winter
}