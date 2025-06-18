using System.Collections.Generic;
using TXDCL.Astar;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "MapData", menuName = "Map/MapData")]
public class MapData_SO : ScriptableObject
{
    [SceneName]
    public string mapName;
    public AssetReference SceneToLoad;
    public List<TileProperty> tileProperties;
    public GridNodes gridNodes;
    
    public int gridWidth;
    public int gridHeight;
    public int originX;
    public int originY;
}