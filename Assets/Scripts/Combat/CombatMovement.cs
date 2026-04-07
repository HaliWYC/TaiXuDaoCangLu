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
        public bool arriveTargetPosition;//移动停下后才可释放法术
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
            grid = FindAnyObjectByType<Grid>();
        }
        
        public void BuildPath(Stack<MovementStep> movementSteps, bool isPlayer)
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
            arriveTargetPosition = false;
            Movement(movementSteps, isPlayer);
        }
        private void Movement(Stack<MovementStep> movementSteps, bool isPlayer)
        {
            if (movementSteps.Count <= 0 || character.CharacterData.currentMovement <= 0)
            {
                character.isMoving = false;
                CombatGridManager.Instance.SetGridObstacle(CombatGridManager.Instance.CharacterPositionsInCombatDict[character], true);
                arriveTargetPosition = true;
                if (!isPlayer) return;
                CombatGridManager.Instance.DisplayCharactersMovementPath();
                return;
            }
            var movementStep = movementSteps.Pop();
            var targetPos = movementStep.gridCoordinates;
            character.SetCharacterFacingDirection(CombatGridManager.Instance.GetGridPosition(targetPos).x - CombatGridManager.Instance.CharacterPositionsInCombatDict[character].x);
            transform.DOMove(GetWorldPosition((Vector3Int)targetPos), CalculateMovementSpeed()).SetEase(Ease.Linear).onComplete = () =>
            {
                character.isMoving = true;
                character.CharacterData.currentMovement--;
                CombatGridManager.Instance.SetCharactersInGridPos(character, targetPos);
                Movement(movementSteps, isPlayer);
            };
        }
        /// <summary>
        /// 根据生成的最短路径进行移动并且在到达最小范围时停下
        /// </summary>
        /// <param name="movementSteps"></param>
        /// <param name="isPlayer"></param>
        /// <param name="minimumRange">最小范围</param>
        public void BuildPath(Stack<MovementStep> movementSteps, bool isPlayer, int minimumRange)
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
            arriveTargetPosition = false;
            Movement(movementSteps, isPlayer, minimumRange);
        }
        private void Movement(Stack<MovementStep> movementSteps, bool isPlayer, int minimumRange)
        {
            if (movementSteps.Count <= 0 || movementSteps.Count <= minimumRange || character.CharacterData.currentMovement <= 0)
            {
                CombatGridManager.Instance.SetGridObstacle(CombatGridManager.Instance.CharacterPositionsInCombatDict[character], true);
                character.isMoving = false;
                arriveTargetPosition = true;
                if (!isPlayer) return;
                CombatGridManager.Instance.DisplayCharactersMovementPath();
                return;
            }
            var movementStep = movementSteps.Pop();
            var targetPos = movementStep.gridCoordinates;
            character.SetCharacterFacingDirection(CombatGridManager.Instance.GetGridPosition(targetPos).x - CombatGridManager.Instance.CharacterPositionsInCombatDict[character].x);
            transform.DOMove(GetWorldPosition((Vector3Int)targetPos), CalculateMovementSpeed()).SetEase(Ease.Linear).onComplete = () =>
            {
                character.isMoving = true;
                character.CharacterData.currentMovement--;
                CombatGridManager.Instance.SetCharactersInGridPos(character, targetPos);
                Movement(movementSteps, isPlayer, minimumRange);
            };
        }

        private float CalculateMovementSpeed()
        {
            return 2.5f / (((int)character.CharacterData.Jingjie.JingjieLevel + 1) * 5 + ((int)character.CharacterData.Jingjie.miniJingjieLevel + 1) * 1);
        }
        
        private Vector3 GetWorldPosition(Vector3Int gridPosition)
        {
            var gridPos = grid.CellToWorld(gridPosition);
            return new Vector3(gridPos.x + Settings.gridCellSize / 2f, gridPos.y + Settings.gridCellSize / 2f,
                gridPos.z + Settings.gridCellSize / 2f);
        }
    }
}
