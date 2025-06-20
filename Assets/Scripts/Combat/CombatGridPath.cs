using System;
using System.Collections.Generic;
using TXDCL.Astar;
using TXDCL.Character;
using TXDCL.Map;
using TXDCL.Time;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace TXDCL.Combat
{
    public class CombatGridPath : Singleton<CombatGridPath>
    {
        public Tile PotentialPath;//可移动范围
        public Tile ComfirmPath;//确认移动路线
        public Tile PotentialFaShuPath;//法术施法范围
        public Tile ComfirmFaShuPath;//确认施法范围
        
        private Grid grid;
        private GridNodes gridNodes;
        private int gridWidth;
        private int gridHeight;
        private int originX;
        private int originY;
        public CharacterBase currentCharacter;
        private int combatGridWidth;
        private int combatGridHeight;
        private int combatOriginX;
        private int combatOriginY;
        
        public List<CharacterBase> CharactersInCombat = new();
        private Dictionary<CharacterBase,Vector2Int> CharacterPositionsInCombatDict = new();
        private List<Vector2Int> CharacterLocationInCombat = new();
        private void OnEnable()
        {
            EventHandler.NewCharactersEnterCombatEvent += OnNewCharactersEnterCombatEvent;
            EventHandler.BeforeCombatBeginEvent += OnBeforeCombatBeginEvent;
            EventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
        }

        private void OnDisable()
        {
            EventHandler.NewCharactersEnterCombatEvent -= OnNewCharactersEnterCombatEvent;
            EventHandler.BeforeCombatBeginEvent -= OnBeforeCombatBeginEvent;
            EventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadEvent;
        }

        private void OnAfterSceneLoadEvent()
        {
            grid = FindFirstObjectByType<Grid>();
        }

        private void OnBeforeCombatBeginEvent()
        {
            TimeManager.Instance.gameClockPause = true;
            GridMapManager.Instance.GetGridDimensions(SceneManager.GetActiveScene().name, out var mapData);
            gridNodes = mapData.gridNodes;
            gridWidth = mapData.gridWidth;
            gridHeight = mapData.gridHeight;
            originX = mapData.originX;
            originY = mapData.originY;
        }
        
        private void OnNewCharactersEnterCombatEvent(List<CharacterBase> characters)
        {
            CharactersInCombat.AddRange(characters);
            GetAndSetCharactersInGrid();
        }
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.J))
            {
                EventHandler.CallBeforeCombatBeginEvent();
                //OnBeforeCombatBeginEvent();
                GetAndSetCharactersInGrid();
                DisplayPath(
                    FindPotentialPath(currentCharacter, currentCharacter.CharacterData.maxMovementPerTurn,
                        new Vector2Int(gridWidth, gridHeight), new Vector2Int(originX, originY)), PotentialPath);
            }
        }
        /// <summary>
        /// 展示路径
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="Tile">显示的瓦片地图信息</param>
        private void DisplayPath(List<Vector2Int> path, Tile Tile)
        {
            Tile.Tilemap.ClearAllTiles();
            foreach (var tile in path)
            {
                Tile.Tilemap.SetTile(new Vector3Int(tile.x + originX, tile.y + originY, 0), Tile.TileBase);
            }
        }
        /// <summary>
        /// 根据输入的角色和范围去获得一个以角色为中心的范围内所有可前往的格子
        /// </summary>
        /// <param name="character">角色</param>
        /// <param name="range">范围</param>
        /// <param name="gridDimensions"></param>
        /// <param name="gridOrigin"></param>
        /// <returns></returns>
        private List<Vector2Int> FindPotentialPath(CharacterBase character, int range, Vector2Int gridDimensions, Vector2Int gridOrigin)
        {
            //最大距离
            var maxDistance = range * 10;
            //起始点
            var startPos = new AStarNode(CharacterPositionsInCombatDict[character]);
            var PotentialPath = new List<Vector2Int>();
            for (var x = gridOrigin.x; x < gridDimensions.x + 1; x++)
            {
                for (var y = gridOrigin.y; y < gridDimensions.y + 1; y++)
                {
                    GetValidNodeEdge(x, y, true, out var Node);
                    if (Node == null || Node.gridPosition == startPos.gridPosition) continue;
                    if(AStar.Instance.GetDistance(startPos, Node) > maxDistance) continue;
                    PotentialPath.Add(new Vector2Int(x, y));
                }
            }
            return PotentialPath;
        }
        
        /// <summary>
        /// 获得地图网格区域
        /// </summary>
        /// <param name="gridDimensions"></param>
        /// <param name="gridOrigin"></param>
        private void GetGridDimensions(out Vector2Int gridDimensions, out Vector2Int gridOrigin)
        {
            gridDimensions = new Vector2Int(gridWidth, gridHeight);
            gridOrigin = new Vector2Int(originX, originY);
        }
        /// <summary>
        /// 获得战斗区域
        /// </summary>
        /// <param name="gridDimensions"></param>
        /// <param name="gridOrigin"></param>
        private void GetCombatGridDimensions(out Vector2Int gridDimensions, out Vector2Int gridOrigin)
        {
            var gridWidth = CharacterLocationInCombat.Select(x => x.x).ToArray();
            var gridHeight = CharacterLocationInCombat.Select(x => x.y).ToArray();
            gridDimensions = new Vector2Int(Mathf.Max(gridWidth) + 10, Mathf.Max(gridHeight) + 10);
            gridOrigin = new Vector2Int(Mathf.Min(gridWidth) - 10, Mathf.Min(gridHeight) - 10);
            combatGridWidth = gridDimensions.x;
            combatGridHeight = gridDimensions.y;
            combatOriginX = gridOrigin.x;
            combatOriginY = gridOrigin.y;
        }
        
        
        /// <summary>
        /// 检测当前坐标下的格子是否是障碍或者超过网格范围
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="checkObstacle"></param>
        /// <param name="Node"></param>
        /// <returns></returns>
        private bool GetValidNodeEdge(int x, int y, bool checkObstacle, out AStarNode Node)
        {
            Node = null;
            if(x < 0 || x >= gridWidth || y < 0 || y >= gridHeight)
                return false;
            Node = gridNodes.GetGridNode(x, y);
            if (checkObstacle)
            {
                Node = Node.isObstacle ? null : Node;
            }
            return Node != null;
        }

        /// <summary>
        /// 获得同时设置所有战斗中角色位于网格地图上对应的格子
        /// </summary>
        private void GetAndSetCharactersInGrid()
        {
            CharacterLocationInCombat.Clear();
            CharacterPositionsInCombatDict.Clear();
            foreach (var character in CharactersInCombat)
            {
                //获得角色网格坐标需要减去原点
                var Pos = grid.WorldToCell(character.transform.position);
                var pos = new Vector2Int(Pos.x-originX, Pos.y-originY);
                while (CharacterLocationInCombat.Contains(pos) || !GetValidNodeEdge(pos.x, pos.y,true, out var Node))
                {
                    //如果当前位置已有角色或者修正后在障碍中，则随机在上下左右一格检测，直到空白
                    var direction = Random.Range(0,3);
                    pos = direction switch
                    {
                        0 => (pos + Vector2Int.left).x < gridWidth ? pos + Vector2Int.left : pos,
                        1 => (pos + Vector2Int.up).x < gridWidth ? pos + Vector2Int.up : pos,
                        2 => (pos + Vector2Int.right).x < gridWidth ? pos + Vector2Int.right : pos,
                        _ => (pos + Vector2Int.down).x < gridWidth ? pos + Vector2Int.down : pos
                    };
                }
                //更新角色位置为世界坐标需要加上原点以及修正值确保角色在当前格子正中心
                character.transform.position =
                    new Vector2(pos.x + originX + PositionModifier(Pos.x),
                        pos.y + originY + PositionModifier(Pos.y));
                CharacterPositionsInCombatDict.Add(character, pos);
                CharacterLocationInCombat.Add(pos);
            }
        }
        
        private float PositionModifier(float value)
        {
            return value / value * 0.5f;
        }
    }
    [Serializable]
    public class Tile
    {
        public Tilemap Tilemap;
        public TileBase TileBase;
    }
}
