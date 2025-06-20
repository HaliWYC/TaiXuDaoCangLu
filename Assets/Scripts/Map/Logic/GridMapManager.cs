using System;
using System.Collections.Generic;
using System.Linq;
using TXDCL.Astar;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TXDCL.Map
{
    public class GridMapManager : Singleton<GridMapManager> 
    {
        public List<SceneData_SO> miniMaps = new();

        private Dictionary<string, TileDetails> tileDetailsDict = new();
        

        protected override void Awake()
        {
            base.Awake();
            foreach (var mapData in miniMaps)
            {
                InitTileDetailsDict(mapData);
            }
            GenerateMapGrid();
        }
        private void InitTileDetailsDict(SceneData_SO mapData)
        {
            foreach (var tileProperty in mapData.tileProperties)
            {
                var tileDetails = new TileDetails
                {
                    gridX = tileProperty.tileCoordinates.x,
                    gridY = tileProperty.tileCoordinates.y
                };
                var key = tileDetails.gridX + "x" + tileDetails.gridY + "y" + mapData.sceneName;
                if (GetTileDetails(key) != null)
                {
                    tileDetails = GetTileDetails(key);
                }
                switch (tileProperty.gridType)
                {
                    case GridType.CanDig:
                        tileDetails.canDig = tileProperty.boolTypeValue;
                        break;
                    case GridType.CanDrop:
                        tileDetails.canDrop = tileProperty.boolTypeValue;
                        break;
                    case GridType.CanDestroy:
                        tileDetails.canDestroy = tileProperty.boolTypeValue;
                        break;
                    case GridType.CanAttack:
                        tileDetails.canAttack = tileProperty.boolTypeValue;
                        break;
                    case GridType.CanLeave:
                        tileDetails.canLeave = tileProperty.boolTypeValue;
                        break;
                    case GridType.Obstacle:
                        tileDetails.obstacle = tileProperty.boolTypeValue;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (GetTileDetails(key) != null)
                {
                    tileDetailsDict[key] = tileDetails;
                }
                else
                {
                    tileDetailsDict.Add(key, tileDetails);
                }
            }
        }

        private TileDetails GetTileDetails(string key)
        {
            return tileDetailsDict.GetValueOrDefault(key);
        }

        private void GenerateMapGrid()
        {
            foreach (var map in miniMaps.Where(map => map.gridNodes == null))
            {
                map.gridNodes = new GridNodes(map.gridWidth,map.gridHeight);
                for (var x = 0; x < map.gridWidth; x++)
                {
                    for (var y = 0; y < map.gridHeight; y++)
                    {
                        var key = (x + map.originX) + "x" + (y + map.originY) + "y" + map.sceneName;
                        var tileDetails =  GetTileDetails(key);
                        if (tileDetails == null) continue;
                        var node = map.gridNodes.GetGridNode(x, y);
                        node.isObstacle = tileDetails.obstacle;
                    }
                }
            }
        }

        /// <summary>
        /// 获得网格数据
        /// </summary>
        /// <param name="sceneName">场景名字</param>
        /// <param name="gridDimensions">网格范围</param>
        /// <param name="gridOrigin">网格初始点</param>
        /// <returns></returns>
        public bool GetGridDimensions(string sceneName, out SceneData_SO mapData)
        {
            mapData = miniMaps.Find(m=>m.sceneName == sceneName);
            return mapData != null;
        }
    }
}

