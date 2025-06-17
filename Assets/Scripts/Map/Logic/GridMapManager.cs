using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TXDCL.Map
{
    public class GridMapManager : Singleton<GridMapManager> 
    {
        public List<MapData_SO> miniMaps = new();
        
        public Dictionary<string, TileDetails> tileDetailsDict = new();

        protected override void Awake()
        {
            base.Awake();
            foreach (var mapData in miniMaps)
            {
                InitTileDetailsDict(mapData);
            }
        }

        private void InitTileDetailsDict(MapData_SO mapData)
        {
            foreach (var tileProperty in mapData.tileProperties)
            {
                var tileDetails = new TileDetails
                {
                    gridX = tileProperty.tileCoordinates.x,
                    gridY = tileProperty.tileCoordinates.y
                };
                var key = tileDetails.gridX + "x" + tileDetails.gridY + "y" + mapData.mapName;
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

        public TileDetails GetTileDetails(string key)
        {
            return tileDetailsDict.GetValueOrDefault(key);
        }

        /// <summary>
        /// 获得网格数据
        /// </summary>
        /// <param name="sceneName">场景名字</param>
        /// <param name="gridDimensions">网格范围</param>
        /// <param name="gridOrigin">网格初始点</param>
        /// <returns></returns>
        public bool GetGridDimensions(string sceneName, out Vector2Int gridDimensions, out Vector2Int gridOrigin )
        {
            gridDimensions = Vector2Int.zero;
            gridOrigin = Vector2Int.zero;

            foreach (var map in miniMaps.Where(map => map.mapName == sceneName))
            {
                gridDimensions.x = map.gridWidth;
                gridDimensions.y = map.gridHeight;

                gridOrigin.x = map.originX;
                gridOrigin.y = map.originY;
                return true;
            }
            return false;
        }
    }
}

