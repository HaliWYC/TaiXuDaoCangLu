using TXDCL.Inventory;
using UnityEngine;

namespace TXDCL.Inventory
{
    [CreateAssetMenu(fileName = "CaoYaoDetails",menuName = "Inventory/Other/CaoYao")]
    public class CaoYaoDetails : OtherItemDetails
    {
        /// <summary>
        /// 草药信息
        /// </summary>
        public int YaoXing;//药性
        public CaoYaoStateType CaoYaoStateType;//草药形态
        
    }
}
