using UnityEngine;

namespace TXDCL.Astar
{
    public class MovementStep
    {
        [SceneName]
        public string sceneName;
        public Vector2Int gridCoordinates;
        public int hour;
        public int minute;
        public int second;
    }
}
