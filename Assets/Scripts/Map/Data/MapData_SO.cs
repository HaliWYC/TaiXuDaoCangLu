using System.Collections.Generic;
using UnityEngine;

namespace TXDCL.Map
{
    [CreateAssetMenu(fileName = "MapData", menuName = "Map/MapData")]
    public class MapData_SO : ScriptableObject
    {
        //大地图信息，一般由一个场景中的图片组成，图片上有几处可通往的区域
        public string mapName;
        public List<DistrictData_SO> Districts;
    }
}


