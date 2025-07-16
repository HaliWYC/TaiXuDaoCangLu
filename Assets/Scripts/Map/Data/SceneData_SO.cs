using System.Collections.Generic;
using TXDCL.Astar;
using TXDCL.Character;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace TXDCL.Map
{
    [CreateAssetMenu(fileName = "SceneData", menuName = "Map/SceneData")]
    public class SceneData_SO : ScriptableObject
    {
        //场景信息，一般由瓦片地图组成
        [SceneName]
        public string sceneName;
        public AssetReference SceneToLoad;
        public List<TileProperty> tileProperties;
        public GridNodes gridNodes;
        public List<CharacterTransform> Characters;
    
        public int gridWidth;
        public int gridHeight;
        public int originX;
        public int originY;
    }

    [System.Serializable]
    public class CharacterTransform
    {
        public GameObject character;
        public Vector3 transform;
    }
}
