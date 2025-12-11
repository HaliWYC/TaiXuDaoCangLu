using System.Collections.Generic;
using TXDCL.Inventory;
using UnityEngine;

namespace TXDCL.Inventory
{
    [CreateAssetMenu(fileName = "DanFangDetails", menuName = "Inventory/ConsumeItem/DanFang")]
    public class DanFangDetails : ConsumablesDetails
    {
        /// <summary>
        /// 丹方信息
        /// </summary>
        public List<CaoYaoDetails> MainCaoYaoList;
        public List<CaoYaoDetails> SubCaoYaoList;
        public List<CaoYaoDetails> GuideCaoYaoList;
        public DanYaoDetails ProductDanYao;
    }
}
