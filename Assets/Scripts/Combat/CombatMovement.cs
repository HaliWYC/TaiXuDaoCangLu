using System;
using System.Collections;
using System.Collections.Generic;
using TXDCL.Astar;
using TXDCL.Time;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

namespace TXDCL.Combat
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Animator))]
    public class CombatMovement : MonoBehaviour
    {
        public Vector3Int currentPosition;
        public Vector3Int targetPosition;
        private Grid grid;
        private TimeSpan gameTime => TimeManager.Instance.currentGameTime;
        
        private Stack<MovementStep> movementSteps = new();

        private void OnEnable()
        {
            EventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
        }

        private void OnDisable()
        {
            EventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadEvent;
        }

        private void OnAfterSceneLoadEvent()
        {
            grid = FindFirstObjectByType<Grid>();
        }
        
        [ContextMenu("Move")]
        private void Movement()
        {
            if (movementSteps.Count <= 1) return;
            var movementStep = movementSteps.Pop();
            transform.DOMove(GetWorldPosition((Vector3Int)movementStep.gridCoordinates), 0.3f).SetEase(Ease.Linear).onComplete =
                Movement;
        }
        
        
        [ContextMenu("Test")]
        public void BuildPath()
        {
            movementSteps.Clear();
            AStar.Instance.BuildPath(SceneManager.GetActiveScene().name, (Vector2Int)currentPosition,
                (Vector2Int)targetPosition, movementSteps);

            if (movementSteps.Count > 1)
            {
                UpdateTimeOnPath();
            }
        }

        private void UpdateTimeOnPath()
        {
            var time = gameTime;
            foreach (var step in movementSteps)
            {
                step.hour = time.Hours;
                step.minute = time.Minutes;
                step.second = time.Seconds;

                var nextStepTime = new TimeSpan(0, 0, 1);
                time = time.Add(nextStepTime);
            }
        }

        private Vector3 GetWorldPosition(Vector3Int gridPosition)
        {
            var gridPos = grid.CellToWorld(gridPosition);
            return new Vector3(gridPos.x + Settings.gridCellSize / 2f, gridPos.y,
                gridPos.z + Settings.gridCellSize / 2f);
        }
    }
}
