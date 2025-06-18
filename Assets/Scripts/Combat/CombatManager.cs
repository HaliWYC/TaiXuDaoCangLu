using System;
using System.Collections.Generic;
using System.Linq;
using TXDCL.Astar;
using UnityEngine;
using UnityEngine.Tilemaps;
using TXDCL.Character;
using TXDCL.Map;
using Unity.Mathematics;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace TXDCL.Combat
{
    public class CombatManager : Singleton<CombatManager>
    {
        public CharacterBase player;
        public Tile PotentialPath;
        public Tile ComfirmPath;
        public Tile PotentialFaShuPath;
        public Tile ComfirmFaShuPath;
        public Tile PotentialTile;

        private GridNodes gridNodes;
        private int gridWidth;
        private int gridHeight;
        private int originX;
        private int originY;
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
        }

        private void OnDisable()
        {
            EventHandler.NewCharactersEnterCombatEvent -= OnNewCharactersEnterCombatEvent;
            EventHandler.BeforeCombatBeginEvent -= OnBeforeCombatBeginEvent;
        }
        
        private void OnBeforeCombatBeginEvent()
        {
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
                OnBeforeCombatBeginEvent();
                GetAndSetCharactersInGrid();
                DisplayPath(
                    FindPotentialPath(player, player.CharacterData.maxMovementPerTurn,
                        new Vector2Int(gridWidth, gridHeight), new Vector2Int(originX, originY)), PotentialPath);
            }
        }

        private void DisplayPath(List<Vector2Int> path, Tile Tile)
        {
            foreach (var tile in path)
            {
                Tile.Tilemap.SetTile(new Vector3Int(tile.x + originX, tile.y + originY, 0), Tile.TileBase);
            }
        }

        private List<Vector2Int> FindPotentialPath(CharacterBase character, int range, Vector2Int gridDimensions, Vector2Int gridOrigin)
        {
            //获得可移动距离
            var maxDistance = range * 10;
            var startPos = new AStarNode(CharacterPositionsInCombatDict.GetValueOrDefault(character));
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

        private void GetGridDimensions(out Vector2Int gridDimensions, out Vector2Int gridOrigin)
        {
            gridDimensions = new Vector2Int(gridWidth, gridHeight);
            gridOrigin = new Vector2Int(originX, originY);
        }
        
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
        
        private bool GetValidNodeEdge(int x, int y, bool checkObstacle, out AStarNode Node)
        {
            Node = null;
            if(x < 0 || x >= gridWidth || y < 0 || y >= gridHeight)
                return false;
            Node = gridNodes.GetGridNode(x, y);
            //检测目标格子是否是障碍或者超过网格范围
            if (checkObstacle)
            {
                Node = Node.isObstacle ? null : Node;
            }
            return Node != null;
        }

        private void GetAndSetCharactersInGrid()
        {
            CharacterLocationInCombat.Clear();
            CharacterPositionsInCombatDict.Clear();
            foreach (var character in CharactersInCombat)
            {
                //获得角色网格坐标需要减去原点
                var pos = new Vector2Int((int)(character.transform.position.x - originX),
                    (int)(character.transform.position.y - originY));
                while (CharacterLocationInCombat.Contains(pos)|| !GetValidNodeEdge(pos.x, pos.y,true, out var Node))
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
                    new Vector2(pos.x + originX + PositionModifier(character.transform.position.x),
                        pos.y + originY + PositionModifier(character.transform.position.y));
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
