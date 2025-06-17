using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "MapData", menuName = "Map/MapData")]
public class MapData_SO : ScriptableObject
{
    public AssetReference SceneToLoad;
    public List<TileProperty> tileProperties;
}