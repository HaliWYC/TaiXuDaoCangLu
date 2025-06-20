using System.Collections.Generic;
using UnityEngine;

namespace TXDCL.Map
{
    [CreateAssetMenu(fileName = "WorldData", menuName = "Map/WorldData")]
    public class WorldData_SO : ScriptableObject
    {
        //世界地图信息，一般由一个场景中的图片组成，图片上有几处可通往的大地图
        public string worldName;
        public List<MapData_SO> Maps;
    }
}
