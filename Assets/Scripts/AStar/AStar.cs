using System.Collections.Generic;
using UnityEngine;
using TXDCL.Map;

namespace TXDCL.Astar
{
    public class AStar : MonoBehaviour
    {
        private GridNodes gridNodes;
        private AStarNode startNode;
        private AStarNode targetNode;
        private int gridWidth;
        private int gridHeight;
        private int originX;
        private int originY;

        private List<AStarNode> openNodesList = new();//当前选中Node周围的4个Nodes
        private HashSet<AStarNode> closedNodesList = new();
        private bool pathFound = false;

        /// <summary>
        /// 构建路径更新每一步
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="steps"></param>
        public void BuildPath(string sceneName,Vector2Int start, Vector2Int end, Stack<MovementStep> steps)
        {
            pathFound = false;
            if (!GenerateGridNodes(sceneName, start, end)) return;
            //查找最短路径
            if (FindShortestPath())
            {
                //构建移动路径
                UpdatePathOnMovementStepStack(sceneName,steps);
            }
        }
        

        private bool GenerateGridNodes(string sceneName, Vector2Int start, Vector2Int end)
        {
            if (GridMapManager.Instance.GetGridDimensions(sceneName, out var gridDimension, out var gridOrigin))
            {
                gridNodes = new GridNodes(gridDimension.x, gridDimension.y);
                gridWidth = gridDimension.x;
                gridHeight = gridDimension.y;
                originX = gridOrigin.x;
                originY = gridOrigin.y;
                openNodesList = new List<AStarNode>();
                closedNodesList = new HashSet<AStarNode>();
            }
            else
            {
                return false;
            }
            //gridNode的范围是从0，0开始，所以要减去初始点坐标得到实际坐标
            startNode = gridNodes.GetGridNode(start.x-originX, start.y-originY);
            targetNode = gridNodes.GetGridNode(end.x-originX, end.y-originY);

            for (var x = 0; x < gridWidth; x++)
            {
                for (var y = 0; y < gridHeight; y++)
                {
                    var key = (x + originX + "x" + y + originY + "y" + sceneName);
                    var tileDetails =  GridMapManager.Instance.GetTileDetails(key);
                    if (tileDetails == null) continue;
                    var node = gridNodes.GetGridNode(x, y);
                    if (tileDetails.obstacle)
                    {
                        node.isObstacle = true;
                    }
                }
            }
            
            return true;
        }

        /// <summary>
        /// 找到最短路径，同时添加到CloseNodeList里面
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        private bool FindShortestPath()
        {
            //添加起始点
            openNodesList.Add(startNode);
            
            while (openNodesList.Count > 0)
            {
                //节点排序
                openNodesList.Sort();
                var node = openNodesList[0];
                closedNodesList.Add(node);
                openNodesList.RemoveAt(0);
                if (targetNode == node)
                {
                    pathFound = true;
                    break;
                }
                
                EvaluateNeighboursNode(node);
            }

            return pathFound;
        }

        /// <summary>
        /// 评估上下左右四个点，并生成对应消耗值
        /// </summary>
        /// <param name="currentNode"></param>
        private void EvaluateNeighboursNode(AStarNode currentNode)
        {
            var currentNodePos = currentNode.gridPosition;

            for (var x = -1; x <= 1; x++)
            {
                for (var y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0 || x!=0 && y!=0) continue;
                    var neighbourNode = GetValidNeighbourNode(currentNodePos.x + x, currentNodePos.y + y);
                    if (neighbourNode != null)
                    {
                        neighbourNode.gCost = currentNode.gCost + GetDistance(currentNode, neighbourNode);
                        neighbourNode.hCost = GetDistance(neighbourNode, targetNode);
                        neighbourNode.parentNode = currentNode;
                        openNodesList.Add(neighbourNode);
                    }
                }
            }
            
        }


        private AStarNode GetValidNeighbourNode(int x, int y)
        {
            //检测当前格子是否超过网格范围
            if(x < 0 || x >= gridWidth || y < 0 || y >= gridHeight)
                return null;
            var node = gridNodes.GetGridNode(x, y);
            //检测目标格子是否是障碍或者已在路径中
            if(node.isObstacle || closedNodesList.Contains(node))
                return null;
            return node;
        }

        
        /// <summary>
        /// 返回任意两点的距离值
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private int GetDistance(AStarNode a, AStarNode b)
        {
            var xDistance = Mathf.Abs(a.gridPosition.x - b.gridPosition.x);
            var yDistance = Mathf.Abs(a.gridPosition.y - b.gridPosition.y);

            return (xDistance + yDistance) * 10;
        }
        
        /// <summary>
        /// 更新每一步坐标
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="stepStack"></param>
        private void UpdatePathOnMovementStepStack(string sceneName, Stack<MovementStep> stepStack)
        {
            var nextNode = targetNode;
            while (nextNode != null)
            {
                var nextStep = new MovementStep
                {
                    sceneName = sceneName,
                    gridCoordinates = new Vector2Int
                        { x = nextNode.gridPosition.x + originX, y = nextNode.gridPosition.y + originY }
                };
                stepStack.Push(nextStep);
                nextNode = nextNode.parentNode;
            }
        }
    }
}

