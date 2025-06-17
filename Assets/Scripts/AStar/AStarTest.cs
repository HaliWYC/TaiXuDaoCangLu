using System;
using System.Collections.Generic;
using TXDCL.Astar;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class AStarTest : MonoBehaviour
{
    public AStar astar;
    [Header("用于测试")] 
    public Vector2Int startPos;
    public Vector2Int targetPos;
    public Tilemap displayTilemap;
    public TileBase displayTile;
    public bool displayStartAndTarget;
    public bool displayPath;
    private Stack<MovementStep> testSteps;

    private void Awake()
    {
        astar = GetComponent<AStar>();
        testSteps = new Stack<MovementStep>();
    }

    private void Update()
    {
        ShowPath();
    }

    private void ShowPath()
    {
        if (displayTilemap != null && displayTile != null)
        {
            if (displayStartAndTarget)
            {
                displayTilemap.SetTile((Vector3Int)startPos, displayTile);
                displayTilemap.SetTile((Vector3Int)targetPos, displayTile);
            }
            else
            {
                displayTilemap.SetTile((Vector3Int)startPos, null);
                displayTilemap.SetTile((Vector3Int)targetPos, null);
            }

            if (displayPath)
            {
                var sceneName = SceneManager.GetActiveScene().name;
                astar.BuildPath(sceneName,startPos,targetPos,testSteps);
                foreach (var step in testSteps)
                {
                    displayTilemap.SetTile((Vector3Int)step.gridCoordinates, displayTile);
                }
            }
            else
            {
                if (testSteps.Count <= 0) return;
                foreach (var step in testSteps)
                {
                    displayTilemap.SetTile((Vector3Int)step.gridCoordinates, null);
                }
                testSteps.Clear();
            }
        }
    }
}