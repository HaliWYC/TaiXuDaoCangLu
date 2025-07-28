using UnityEngine;

namespace TXDCL.Inventory
{
    public class ItemDetails : ScriptableObject
    {
        public int ID;
        public string Name;
        public Sprite Icon;
        public ItemRarity ItemRarity;
        public MiniRarity MiniRarity;
        public ItemType itemType;
        public int Price;
        [TextArea]
        public string Description;
        
        [Header("Function")]
        public bool canStack;
        public int stackSize;
        public bool canTrade;
        [Range(0,1f)]
        public float TradeProportion;
        public bool canUseInCombat;
    }
}

