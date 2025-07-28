using System.Collections.Generic;
using UnityEngine;

namespace TXDCL.Inventory
{
    [CreateAssetMenu(fileName = "ItemDetailList", menuName = "Inventory/ItemDetailList")]
    public class ItemDetailList : ScriptableObject
    {
        public List<ItemDetails> ItemList;
    }
}
