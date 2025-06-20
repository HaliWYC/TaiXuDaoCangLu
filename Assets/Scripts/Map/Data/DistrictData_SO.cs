using System.Collections.Generic;
using UnityEngine;

namespace TXDCL.Map
{
    [CreateAssetMenu(fileName = "DistrictData", menuName = "Map/DistrictData")]
    public class DistrictData_SO : ScriptableObject
    {
        //区域信息，一般由一个场景中的图片组成，图片上有几处可通往的场景
        public string districtName;
        public PlaceType placeType;
        public List<SceneData_SO> Scenes;
    }
}
