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
        [Header("Tilemaps")]
        [SerializeField] private Tile PotentialPathTile;//可移动范围
        [SerializeField]private Tile ConfirmPathTile;//确认移动路线
        [SerializeField]private Tile PotentialFaShuPathTile;//法术施法范围
        [SerializeField]private Tile ConfirmFaShuPathTile;//确认施法范围
        
        [Header("Grid")]
        private Grid grid;
        private GridNodes gridNodes;
        private int gridWidth;
        private int gridHeight;
        private int originX;
        private int originY;
        private int combatGridWidth;
        private int combatGridHeight;
        private int combatOriginX;
        private int combatOriginY;
        private Vector2Int lastTargetPos = Vector2Int.zero;

        [Header("Character")] 
        //[SerializeField]private CharacterBase player;
        private CharacterBase currentCharacter;
        public Dictionary<CharacterBase,Vector2Int> CharacterPositionsInCombatDict = new();//储存角色信息以及角色网格坐标
        private List<Vector2Int> CharacterLocationInCombat = new();//储存角色世界坐标
        private List<Vector2Int> potentialSelectingPath = new();//可选中路径范围
        private List<Vector2Int> confirmSelectingPath = new();//已选中路径范围
        private Stack<MovementStep> confirmMovementSteps = new();//已选中路径
        private void OnEnable()
        {
            EventHandler.NewCharactersEnterCombatEvent += OnNewCharactersEnterCombatEvent;
            EventHandler.CombatBeginEvent += OnCombatBeginEvent;
            EventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
            EventHandler.CharacterTurnBeginEvent += OnCharacterTurnBeginEvent;
            EventHandler.CharacterTurnEndEvent += OnCharacterTurnEndEvent;
        }
        
        private void OnDisable()
        {
            EventHandler.NewCharactersEnterCombatEvent -= OnNewCharactersEnterCombatEvent;
            EventHandler.CombatBeginEvent -= OnCombatBeginEvent;
            EventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadEvent;
            EventHandler.CharacterTurnBeginEvent -= OnCharacterTurnBeginEvent;
            EventHandler.CharacterTurnEndEvent -= OnCharacterTurnEndEvent;
        }

        private void OnAfterSceneLoadEvent()
        {
            grid = FindFirstObjectByType<Grid>();
            PotentialPathTile.Tilemap = GameObject.FindWithTag("SelectionTileMap").GetComponent<Tilemap>();
            PotentialFaShuPathTile.Tilemap = GameObject.FindWithTag("SelectionTileMap").GetComponent<Tilemap>();
            ConfirmPathTile.Tilemap = GameObject.FindWithTag("ConfirmationTileMap").GetComponent<Tilemap>();
            ConfirmFaShuPathTile.Tilemap = GameObject.FindWithTag("ConfirmationTileMap").GetComponent<Tilemap>();
        }

        private void OnCombatBeginEvent()
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
            if(characters == null || characters.Count == 0) return;
            CombatManager.Instance.CharactersInCombat.AddRange(characters);
            GetAndSetCharactersInGrid();
        }
        
        private void OnCharacterTurnBeginEvent(CharacterBase character)
        {
            currentCharacter = character;
            if(currentCharacter.CharacterData == null) return;
            currentCharacter.CharacterData.currentMovement = currentCharacter.CharacterData.maxMovementPerTurn;
            if(character.CompareTag("Player"))
                DisplayCharactersMovementPath();
        }
        
        private void OnCharacterTurnEndEvent(CharacterBase character)
        {
            CombatManager.Instance.isCharacterTurnActive = false;
            ClearPotentialTiles();
            ClearConfirmPathTiles();
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
                //在世界坐标上显示瓦片信息因此需要加上原点坐标
                Tile.Tilemap.SetTile(new Vector3Int(tile.x , tile.y , 0), Tile.TileBase);
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
        private void FindPotentialPath(CharacterBase character, int range)
        {
            potentialSelectingPath.Clear();
            //最大距离
            var maxDistance = range * 10;
            //起始点
            var startPos = new AStarNode(CharacterPositionsInCombatDict[character]);
            for (var x = originX; x < gridWidth + originX + 1; x++)
            {
                for (var y = originY; y < gridHeight + originY + 1; y++)
                {
                    GetValidNodeEdge(x, y, true, out var Node);
                    if (Node == null || Node.gridPosition == startPos.gridPosition) continue;
                    if(AStar.Instance.GetDistance(startPos, Node) > maxDistance) continue;
                    potentialSelectingPath.Add(new Vector2Int(x, y));
                }
            }
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
        /// <param name="checkObstacle">是否检测障碍</param>
        /// <param name="Node"></param>
        /// <returns></returns>
        private bool GetValidNodeEdge(int x, int y, bool checkObstacle, out AStarNode Node)
        {
            Node = null;
            var X = x - originX;
            var Y = y - originY;
            if(X < 0 || X >= gridWidth || Y < 0 || Y >= gridHeight)
                return false;
            Node = gridNodes.GetGridNode(X, Y);
            if (checkObstacle)
            {
                Node = Node.isObstacle ? null : Node;
            }
            return Node != null;
        }

        /// <summary>
        /// 获得同时设置所有战斗中角色位于网格地图上对应的格子
        /// </summary>
        public void GetAndSetCharactersInGrid()
        {
            CharacterLocationInCombat.Clear();
            CharacterPositionsInCombatDict.Clear();
            if(CombatManager.Instance.CharactersInCombat.Count<=0) return;
            foreach (var character in CombatManager.Instance.CharactersInCombat)
            {
                //获得角色网格坐标需要减去原点
                var pos = (Vector2Int)grid.WorldToCell(character.transform.position);
                while (CharacterLocationInCombat.Contains(pos) || !GetValidNodeEdge(pos.x, pos.y,true, out var Node))
                {
                    //如果当前位置已有角色或者修正后在障碍中，则随机在上下左右一格检测，直到空白
                    var direction = Random.Range(0,3);
                    pos = direction switch
                    {
                        0 => (pos + Vector2Int.left).x < gridWidth - originX ? pos + Vector2Int.left : pos,
                        1 => (pos + Vector2Int.up).y < gridHeight - originY ? pos + Vector2Int.up : pos,
                        2 => (pos + Vector2Int.right).x < gridWidth - originX ? pos + Vector2Int.right : pos,
                        _ => (pos + Vector2Int.down).y < gridHeight - originY ? pos + Vector2Int.down : pos
                    };
                }
                //更新角色位置为世界坐标需要加上原点以及修正值确保角色在当前格子正中心
                character.transform.position = new Vector2(pos.x + 0.5f, pos.y + 0.5f);
                CharacterPositionsInCombatDict.Add(character, GetGridPosition(pos));
                CharacterLocationInCombat.Add(pos);
                gridNodes.GetGridNode(pos.x - originX, pos.y - originY).isObstacle = true;
            }
        }
        private void ClearPotentialTiles()
        {
            PotentialPathTile.Tilemap.ClearAllTiles();
        }

        private void ClearConfirmPathTiles()
        {
            ConfirmPathTile.Tilemap.ClearAllTiles();
        }

        public void DisplayCharactersMovementPath()
        {
            var range = currentCharacter.CharacterData.currentMovement;
            ClearPotentialTiles();
            ClearConfirmPathTiles();
            if (range <= 0) return;
            FindPotentialPath(currentCharacter, range);
            DisplayPath(potentialSelectingPath, PotentialPathTile);
            CursorManager.Instance.isSelecting = true;
        }
        
        public void CheckInPotentialPath(Vector2Int position)
        {
            if (!potentialSelectingPath.Contains(position)) return;
            confirmMovementSteps.Clear();
            var currentPos = GetWorldPosition(CharacterPositionsInCombatDict[currentCharacter]);
            AStar.Instance.BuildPath(SceneManager.GetActiveScene().name, currentPos, position, confirmMovementSteps);
            confirmSelectingPath = confirmMovementSteps.Select(step => step.gridCoordinates).ToList();
            DisplayPath(confirmSelectingPath, ConfirmPathTile);
            lastTargetPos = position;
            CursorManager.Instance.isConfirm = true;
        }

        public void CheckInConfirmPath(Vector2Int position)
        {
            if (lastTargetPos == position)
            {
                gridNodes.GetGridNode(CharacterPositionsInCombatDict[currentCharacter].x,
                    CharacterPositionsInCombatDict[currentCharacter].y).isObstacle = false;
                currentCharacter.CharacterData.currentMovement -= confirmMovementSteps.Count - 1;
                currentCharacter.GetComponent<CombatMovement>().BuildPath(confirmMovementSteps);
                gridNodes.GetGridNode(position.x - originX, position.y - originY).isObstacle = true;
                CursorManager.Instance.isConfirm = false;
                potentialSelectingPath.Clear();
                confirmSelectingPath.Clear();
            }
            else
            {
                CheckInPotentialPath(position);
            }
        }

        public void SetCharactersInGridPos(CharacterBase character, Vector2Int pos)
        {
            CharacterPositionsInCombatDict[character] = GetGridPosition(pos);
        }

        public Vector2Int GetWorldPosition(Vector2Int position)
        {
            return new Vector2Int(position.x + originX, position.y + originY);
        }
        
        public Vector2Int GetGridPosition(Vector2Int position)
        {
            return new Vector2Int(position.x - originX, position.y - originY);
        }
        
        [ContextMenu("Player Turn End")]
        private void PlayerTurnEnd()
        {
            CursorManager.Instance.isSelecting = false;
            EventHandler.CallCharacterTurnEndEvent(currentCharacter);
        }
    }
    [Serializable]
    public class Tile
    {
        public Tilemap Tilemap;
        public TileBase TileBase;
    }
}
