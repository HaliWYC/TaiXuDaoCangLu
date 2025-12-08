using System.Collections.Generic;
using TXDCL.XiuLian.FuShu;
using UnityEngine;

namespace TXDCL.Inventory
{
    [CreateAssetMenu(fileName = "FaBaoDetails", menuName = "Inventory/FaBaoDetails")]
    public class FaBaoDetails : ItemDetails
    {
        public FaBaoType faBaoType;//法宝类型，如武器、服饰、挂件、坐骑等
        public float currentEndurance;//当前耐久
        public float currentMaxEndurance;//当前最大耐久，即修理后可达到的最大耐久
        public float maxEndurance;//最大耐久，即法宝崭新最大耐久
        public float minEnduranceDepreciation;//最低最大耐久损耗，即每次修理后最低减少的最大耐久值
        public bool ConstantEndurance;//是否为永恒耐久，即不会随着使用而消耗耐久
        public List<Property> properties;//属性信息
        public List<FaShuData> FaShuDatas;//法宝自带法术，即使用法宝后释放的法术
    }
}

