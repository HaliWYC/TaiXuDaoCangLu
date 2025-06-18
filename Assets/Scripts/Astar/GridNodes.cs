using UnityEngine;

namespace TXDCL.Astar
{
    public class GridNodes
    {
        public int width;
        public int height;
        private AStarNode[,] gridNode;
        
        /// <summary>
        /// 构造函数初始化节点范围
        /// </summary>
        /// <param name="width">地图宽度</param>
        /// <param name="height">地图高度</param>
        public GridNodes (int width, int height)
        {
            this.width = width;
            this.height = height;
            gridNode = new AStarNode[width, height];
            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    gridNode[x, y] = new AStarNode(new Vector2Int(x, y));
                }
            }
        }

        public AStarNode GetGridNode(int xPos, int yPos)
        {
            if (xPos < width && yPos < height)
            {
                return gridNode[xPos, yPos];
            }
            Debug.Log("超出网格范围");
            return null;
        }
    }
}
