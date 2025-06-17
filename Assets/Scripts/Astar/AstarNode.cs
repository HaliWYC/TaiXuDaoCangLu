using UnityEngine;
using System;
namespace TXDCL.Astar
{
    public class AStarNode : IComparable<AStarNode>
    {
        public Vector2Int gridPosition;//网格坐标
        public int gCost = 0;//距离起点的距离
        public int hCost = 0;//距离目标点的距离
        public int FCost => gCost + hCost;//当前格子的值
        public bool isObstacle = false; //当前格子是否是障碍
        public AStarNode parentNode;

        public AStarNode(Vector2Int pos)
        {
            gridPosition = pos;
            parentNode = null;
        }
        
        /// <summary>
        /// //比较最低的Fcost的值，返回-1，0，1
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(AStarNode other)
        {
            var result = FCost.CompareTo(other.FCost);
            if (result == 0)
            {
                result = hCost.CompareTo(other.hCost);
            }

            return result;
        }
    }
}
