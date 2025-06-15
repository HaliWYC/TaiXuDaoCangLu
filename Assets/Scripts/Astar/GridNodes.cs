using UnityEngine;

namespace Astar
{
    public class GridNodes
    {
        private int width;
        private int height;
        private AstarNode[,] gridNode;
        
        /// <summary>
        /// 构造函数初始化节点范围
        /// </summary>
        /// <param name="width">地图宽度</param>
        /// <param name="height">地图高度</param>
        public GridNodes (int width, int height)
        {
            this.width = width;
            this.height = height;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    gridNode[x, y] = new AstarNode(new Vector2Int(x, y));
                }
            }
        }

        public AstarNode GetGridNode(int xPos, int yPos)
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
