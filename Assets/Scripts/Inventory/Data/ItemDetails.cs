using System;
using UnityEngine;

namespace TXDCL.Inventory
{
    public class ItemDetails : ScriptableObject
    {
        public int ID;//ID
        public string Name;//物品名称
        public Sprite Icon;//物品图片
        public Rarity Rarity;//如残缺、凡级、人级...
        public ItemRarity ItemRarity;//物品品质，如一品、两品...
        public ItemType itemType;//物品类型，如法宝、消耗品...
        public int Price;//物品价格
        [TextArea]
        public string Description;//物品描述
        
        [Header("Function")]
        public bool canStack;//是否可堆叠
        public int stackSize;//可堆叠数量
        public bool canTrade;//是否可交易
        public bool canCarryOn;//是否可携带进战斗
        [Range(0,1f)]
        public float TradeProportion;//交易比例
    }
}

