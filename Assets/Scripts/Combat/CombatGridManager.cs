using System;
using System.Collections.Generic;
using TXDCL.Astar;
using TXDCL.Character;
using TXDCL.Map;
using TXDCL.Time;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using TXDCL.XiuLian.FuShu;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace TXDCL.Combat
{
    public class CombatGridManager : Singleton<CombatGridManager>
    {
        [Header("Tilemaps")]
        [SerializeField] private Tile PotentialPathTile;//可移动瓦片
        [SerializeField]private Tile ConfirmPathTile;//确认移动瓦片
        [SerializeField]private Tile PotentialFaShuPathTile;//法术可施法瓦片
        [SerializeField]private Tile ConfirmFaShuPathTile;//确认法术施法瓦片
        private List<Vector2Int> potentialSelectingPath = new();//可选中路径范围
        private List<Vector2Int> confirmSelectingPath = new();//已选中路径范围
        private Stack<MovementStep> confirmMovementSteps = new();//已选中路径
        private List<Vector2Int> potentialFaShuSelectingPath = new();//可选中施法范围
        private List<Vector2Int> confirmFaShuSelectingPath = new();//已选中施法范围
        
        [Header("Grid")]
        private Grid grid;
        private GridNodes gridNodes;
        private int gridWidth;
        private int gridHeight;
        private int originX;
        private int originY;
        // private int combatGridWidth;
        // private int combatGridHeight;
        // private int combatOriginX;
        // private int combatOriginY;
        private Vector2Int lastTargetPos = Vector2Int.zero;
        private Vector2Int lastFaShuTargetPos = Vector2Int.zero;

        [Header("Character")]
        //[SerializeField]private CharacterBase player;
        public CharacterBase currentCharacter;//当前进行回合的角色
        public bool canCurrentCharacterCastFaShu;
        private FaShuData currentFaShuData;//当前选择的法术
        public readonly Dictionary<CharacterBase,Vector2Int> CharacterPositionsInCombatDict = new();//储存角色信息以及角色网格坐标
        private void OnEnable()
        {
            EventHandler.NewCharactersEnterCombatEvent += OnNewCharactersEnterCombatEvent;
            EventHandler.BeforeCombatBeginEvent += OnBeforeCombatBeginEvent;
            EventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
            EventHandler.CharacterTurnBeginEvent += OnCharacterTurnBeginEvent;
            EventHandler.CharacterTurnEndEvent += OnCharacterTurnEndEvent;
        }
        
        private void OnDisable()
        {
            EventHandler.NewCharactersEnterCombatEvent -= OnNewCharactersEnterCombatEvent;
            EventHandler.BeforeCombatBeginEvent -= OnBeforeCombatBeginEvent;
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
            if(characters == null || characters.Count == 0) return;
            CombatManager.Instance.CharactersInCombat.AddRange(characters);
            GetAndSetCharactersInGrid();
        }
        
        private void OnCharacterTurnBeginEvent(CharacterBase character)
        {
            //Debug.Log(character.CharacterData.characterName);
            if (character != GameManager.Instance.Player) return;
            currentCharacter = character;
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
        /// <param name="Position">起始点</param>
        /// <param name="range">范围</param>
        /// <param name="gridDimensions"></param>
        /// <param name="gridOrigin"></param>
        /// <returns></returns>
        public List<Vector2Int> FindPotentialPath(Vector2Int Position, int range, bool isFaShu)
        {
            var path = new List<Vector2Int>();
            path.Clear();
            //最大距离
            var maxDistance = range * 10;
            //起始点
            var startPos = new AStarNode(Position);
            for (var x = Position.x - range; x < Position.x + range + 1; x++)
            {
                for (var y = Position.y - range; y < Position.y + range + 1; y++)
                {
                    GetValidNodeEdge(x, y, !isFaShu, out var Node);
                    if (Node == null || !isFaShu && Node.gridPosition == startPos.gridPosition) continue;
                    if(AStar.Instance.GetDistance(startPos, Node) > maxDistance) continue;
                    path.Add(GetWorldPosition(new Vector2Int(x, y)));
                }
            }
            return path;
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
        // /// <summary>
        // /// 获得战斗区域
        // /// </summary>
        // /// <param name="gridDimensions"></param>
        // /// <param name="gridOrigin"></param>
        // private void GetCombatGridDimensions(out Vector2Int gridDimensions, out Vector2Int gridOrigin)
        // {
        //     var gridWidth = CharacterLocationInCombat.Select(x => x.x).ToArray();
        //     var gridHeight = CharacterLocationInCombat.Select(x => x.y).ToArray();
        //     gridDimensions = new Vector2Int(Mathf.Max(gridWidth) + 10, Mathf.Max(gridHeight) + 10);
        //     gridOrigin = new Vector2Int(Mathf.Min(gridWidth) - 10, Mathf.Min(gridHeight) - 10);
        //     combatGridWidth = gridDimensions.x;
        //     combatGridHeight = gridDimensions.y;
        //     combatOriginX = gridOrigin.x;
        //     combatOriginY = gridOrigin.y;
        // }
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
        public void GetAndSetCharactersInGrid()
        {
            CharacterPositionsInCombatDict.Clear();
            if(CombatManager.Instance.CharactersInCombat.Count<=0) return;
            foreach (var character in CombatManager.Instance.CharactersInCombat)
            {
                //设置最大循环数防止死循环
                var LoopMaxCount = 0;
                //获得角色网格坐标需要减去原点
                var pos = GetGridPosition((Vector2Int)grid.WorldToCell(character.transform.position));
                while (CharacterPositionsInCombatDict.ContainsValue(pos) || !GetValidNodeEdge(pos.x, pos.y,true, out var Node) && LoopMaxCount < 9999 )
                {
                    //如果当前位置已有角色或者修正后在障碍中，则随机在上下左右一格检测，直到空白
                    var direction = Random.Range(0,3);
                    pos = direction switch
                    {
                        0 => (pos + Vector2Int.left).x < gridWidth ? pos + Vector2Int.left : pos,
                        1 => (pos + Vector2Int.up).y < gridHeight ? pos + Vector2Int.up : pos,
                        2 => (pos + Vector2Int.right).x < gridWidth ? pos + Vector2Int.right : pos,
                        _ => (pos + Vector2Int.down).y < gridHeight ? pos + Vector2Int.down : pos
                    };
                    LoopMaxCount++;
                }
                //更新角色位置为世界坐标需要加上原点以及修正值确保角色在当前格子正中心
                var worldPos = GetWorldPosition(pos);
                character.transform.position = new Vector2(worldPos.x + 0.5f, worldPos.y + 0.5f);
                CharacterPositionsInCombatDict.Add(character, pos);
                gridNodes.GetGridNode(pos.x, pos.y).isObstacle = true;
            }
        }
        /// <summary>
        /// 清空可选择瓦片地图的所有瓦片
        /// </summary>
        public void ClearPotentialTiles()
        {
            PotentialPathTile.Tilemap.ClearAllTiles();
        }
        /// <summary>
        /// 清空已选择瓦片地图的所有瓦片
        /// </summary>
        public void ClearConfirmPathTiles()
        {
            ConfirmPathTile.Tilemap.ClearAllTiles();
        }
        /// <summary>
        /// 根据当前角色（仅玩家显示）的剩余移动力显示可移动的所有位置
        /// </summary>
        public void DisplayCharactersMovementPath()
        {
            var range = currentCharacter.CharacterData.currentMovement;
            //清楚两个瓦片地图所有瓦片
            ClearPotentialTiles();
            ClearConfirmPathTiles();
            //重置位置
            lastTargetPos = Vector2Int.zero;
            if (range <= 0 || !CharacterPositionsInCombatDict.ContainsKey(currentCharacter)) return;
            //找到并显示路径
            potentialSelectingPath = FindPotentialPath(CharacterPositionsInCombatDict[currentCharacter], range, false);
            DisplayPath(potentialSelectingPath, PotentialPathTile);
            //完全显示战斗UI面版
            CombatUI.Instance.FadeCombatPanel(1f);
            CursorManager.Instance.isSelecting = true;
            canCurrentCharacterCastFaShu = true;
        }
        /// <summary>
        /// 判断鼠标点击的网格位置是否在角色可移动的范围网格内，如果是则生成并储存对应的最短路径瓦片路径
        /// </summary>
        /// <param name="position">鼠标点击的网格位置</param>
        public void CheckInPotentialMovementPath(Vector2Int position)
        {
            if (!potentialSelectingPath.Contains(position)) return;
            canCurrentCharacterCastFaShu = false;
            DaoCangPanelUI.Instance.ResetDaoCangPanelUI();
            confirmMovementSteps.Clear();
            var currentPos = GetWorldPosition(CharacterPositionsInCombatDict[currentCharacter]);
            AStar.Instance.BuildPath(SceneManager.GetActiveScene().name, currentPos, position, confirmMovementSteps);
            confirmSelectingPath = confirmMovementSteps.Select(step => step.gridCoordinates).ToList();
            DisplayPath(confirmSelectingPath, ConfirmPathTile);
            CombatUI.Instance.FadeCombatPanel(0.5f);
            lastTargetPos = position;
            CursorManager.Instance.isConfirm = true;
            
        }
        /// <summary>
        /// 判断鼠标点击网格位置是否在储存的已选择的瓦片地图路径中，如果是则移动到目标点上，如果不是则重新生成并储存新的路径
        /// </summary>
        /// <param name="position"></param>
        public void CheckInConfirmMovementPath(Vector2Int position)
        {
            if (lastTargetPos == position)
            {
                currentCharacter.GetComponent<CombatMovement>().BuildPath(confirmMovementSteps, true,
                    currentCharacter.CharacterData.currentMovement);
                CursorManager.Instance.isConfirm = false;
                potentialSelectingPath.Clear();
                confirmSelectingPath.Clear();
            }
            else
            {
                CheckInPotentialMovementPath(position);
            }
        }
        /// <summary>
        /// 根据法术信息显示可释放的法术范围（仅显示玩家的）
        /// </summary>
        /// <param name="faShuData"></param>
        public void DisplayFaShuReleasePath(FaShuData faShuData)
        {
            ClearPotentialTiles();
            ClearConfirmPathTiles();
            currentFaShuData = faShuData;
            if (currentFaShuData.ReleaseRange < 0 || !CharacterPositionsInCombatDict.ContainsKey(currentCharacter)) return;
            potentialFaShuSelectingPath = FindPotentialPath(CharacterPositionsInCombatDict[currentCharacter], currentFaShuData.ReleaseRange, true);
            DisplayPath(potentialFaShuSelectingPath, PotentialFaShuPathTile);
            CombatUI.Instance.FadeCombatPanel(0.5f);
            CursorManager.Instance.isCastingFaShu = true;
        }
        /// <summary>
        /// 根据鼠标点击网格位置判断是否在法术可释放范围中，如果是则显示法术覆盖范围，如果不是则重新选择
        /// </summary>
        /// <param name="startPos"></param>
        public void DisplayFaShuConfirmPath(Vector2Int targetPos)
        {
            ClearConfirmPathTiles();
            if (currentFaShuData.Range < 0 || !potentialFaShuSelectingPath.Contains(targetPos)) return;
            confirmFaShuSelectingPath = FindPotentialPath(GetGridPosition(targetPos), currentFaShuData.Range, true);
            DisplayPath(confirmFaShuSelectingPath, ConfirmFaShuPathTile);
            CombatUI.Instance.FadeCombatPanel(0.5f);
            lastFaShuTargetPos = targetPos;
            CursorManager.Instance.isConfirm = true;
        }
        /// <summary>
        /// 检测所有在法术范围内的角色并执行法术效果
        /// </summary>
        /// <param name="position"></param>
        public void CheckFaShuConfirmTargets(Vector2Int position)
        {
            if (lastFaShuTargetPos == position)
            {
                //获得范围内所有的目标
                var NewPos = new Vector3(position.x + 0.5f, position.y + 0.5f);
                FaShuManager.Instance.ReleaseFaShu(currentFaShuData, NewPos, currentCharacter, GetAllGridInCombatDict(confirmFaShuSelectingPath));
                currentCharacter.SetCharacterFacingDirection(NewPos.x - currentCharacter.transform.position.x);
                currentCharacter.animator.SetTrigger("CastFaShu");
                CursorManager.Instance.isCastingFaShu = false;
                CursorManager.Instance.isConfirm = false;
                ClearPotentialTiles();
                ClearConfirmPathTiles();
                GameManager.Instance.ResetGameCameraLenInGridSize();
                CombatUI.Instance.FadeCombatPanel(1f);
                DisplayCharactersMovementPath();
            }
            else
            {
                DisplayFaShuConfirmPath(position);
            }
        }

        public void SetGridObstacle(Vector2Int position, bool isObstacle)
        {
            gridNodes.GetGridNode(position.x, position.y).isObstacle = isObstacle;
        }
        /// <summary>
        /// 设置角色在目标网格位置
        /// </summary>
        /// <param name="character"></param>
        /// <param name="pos">目标位置的世界坐标</param>
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

        public List<CharacterBase> GetAllGridInCombatDict(List<Vector2Int> path)
        {
            return CharacterPositionsInCombatDict.Keys.Where(character =>
                path.Contains(GetWorldPosition(CharacterPositionsInCombatDict[character]))).ToList();
        }
    }
    [Serializable]
    public class Tile
    {
        public Tilemap Tilemap;
        public TileBase TileBase;
    }
}
