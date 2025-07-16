using System.Collections.Generic;
using UnityEngine;

namespace TXDCL.XiuLian.FuShu
{
    [CreateAssetMenu(fileName = "FaShuData", menuName = "XiuLian/FaShu/FaShuDataList")]
    public class FaShuDataList : ScriptableObject
    {
        public List<FaShuData> FaShuDatas = new();
    }
}
