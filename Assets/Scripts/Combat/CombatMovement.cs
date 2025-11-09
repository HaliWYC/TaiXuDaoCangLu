using System;
using System.Collections.Generic;
using TXDCL.Astar;
using TXDCL.Time;
using UnityEngine;
using DG.Tweening;
using TXDCL.Character;

namespace TXDCL.Combat
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Animator))]
    public class CombatMovement : MonoBehaviour
    {
        private Grid grid;
        private CharacterBase character;
        private void Awake()
        {
            character = GetComponent<CharacterBase>();
        }

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
        
        private void Movement(Stack<MovementStep> movementSteps, bool isPlayer)
        {
            if (movementSteps.Count <= 0)
            {
                CombatGridManager.Instance.SetGridObstacle(
                    CombatGridManager.Instance.CharacterPositionsInCombatDict[character], true);
                if (!isPlayer) return;
                CombatGridManager.Instance.DisplayCharactersMovementPath();
                CombatUI.Instance.FadeCombatPanel(1f);
                character.isMoving = false;
                return;
            }
            var movementStep = movementSteps.Pop();
            var targetPos = movementStep.gridCoordinates;
            character.SetPlayerFacingDirection(CombatGridManager.Instance.GetGridPosition(targetPos).x -
                                               CombatGridManager.Instance.CharacterPositionsInCombatDict[character].x);
            transform.DOMove(GetWorldPosition((Vector3Int)targetPos), 0.3f).SetEase(Ease.Linear).onComplete = () =>
            {
                character.isMoving = true;
                character.CharacterData.currentMovement--;
                CombatGridManager.Instance.SetCharactersInGridPos(character, targetPos);
                Movement(movementSteps, isPlayer);
            };

        }
        public void BuildPath(Stack<MovementStep> movementSteps, bool isPlayer, int MovementRange)
        {
            if (movementSteps.Count < 1) return;
            var time = TimeManager.Instance.currentGameTime;
            foreach (var step in movementSteps)
            {
                step.hour = time.Hours;
                step.minute = time.Minutes;
                step.second = time.Seconds;

                var nextStepTime = new TimeSpan(0, 0, 1);
                time = time.Add(nextStepTime);
            }

            CombatGridManager.Instance.SetGridObstacle(
                CombatGridManager.Instance.CharacterPositionsInCombatDict[character], false);
            character.CharacterData.currentMovement++;
            Movement(movementSteps, isPlayer);
        }

        private Vector3 GetWorldPosition(Vector3Int gridPosition)
        {
            var gridPos = grid.CellToWorld(gridPosition);
            return new Vector3(gridPos.x + Settings.gridCellSize / 2f, gridPos.y + Settings.gridCellSize / 2f,
                gridPos.z + Settings.gridCellSize / 2f);
        }
    }
}
