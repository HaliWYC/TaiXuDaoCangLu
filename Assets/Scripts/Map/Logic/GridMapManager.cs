using System;
using System.Collections.Generic;

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
                var key = tileDetails.gridX +"x"+tileDetails.gridY+"y"+mapData.SceneToLoad;
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
                    case GridType.CanReach:
                        tileDetails.canReach = tileProperty.boolTypeValue;
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
    }
}

