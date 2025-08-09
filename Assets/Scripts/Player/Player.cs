using System;
using System.Collections.Generic;
using TXDCL.Combat;
using TXDCL.XiuLian.FuShu;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TXDCL.Character
{
    public class Player : CharacterBase
    {
        private PlayerController playerController;
        private Vector2 inputDirection;
        private FaShuData currentSelectingFaShu;
        private bool isCombating;
        protected override void Awake()
        {
            base.Awake();
            playerController = new();
            playerController.Gameplay.FaShu1.started += SelectFaShu;
            playerController.Gameplay.FaShu2.started += SelectFaShu;
            playerController.Gameplay.FaShu3.started += SelectFaShu;
            playerController.Gameplay.FaShu4.started += SelectFaShu;
            playerController.Gameplay.FaShu5.started += SelectFaShu;
            playerController.Gameplay.FaShu6.started += SelectFaShu;
            playerController.Gameplay.FaShu7.started += SelectFaShu;
            playerController.Gameplay.FaShu8.started += SelectFaShu;
            playerController.Gameplay.FaShu9.started += SelectFaShu;
            playerController.Gameplay.FaShu0.started += SelectFaShu;
        }
        private void OnEnable()
        {
            InputEnable();
            EventHandler.BeforeSceneLoadEvent += OnBeforeSceneLoadEvent;
            EventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
            EventHandler.MoveToPositionEvent += OnMoveToPositionEvent;
            EventHandler.CombatBeginEvent += OnCombatBeginEvent;
            EventHandler.CharacterTurnBeginEvent += OnCharacterTurnBeginEvent;
        }
        private void OnDisable()
        {
            InputDisable();
            EventHandler.BeforeSceneLoadEvent -= OnBeforeSceneLoadEvent;
            EventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadEvent;
            EventHandler.MoveToPositionEvent -= OnMoveToPositionEvent;
            EventHandler.CombatBeginEvent -= OnCombatBeginEvent;
            EventHandler.CharacterTurnBeginEvent -= OnCharacterTurnBeginEvent;
        }
        
        private void OnBeforeSceneLoadEvent()
        {
            InputDisable();
        }
        private void OnAfterSceneLoadEvent()
        {
            InputEnable();
        }
        private void OnMoveToPositionEvent(Vector3 position)
        {
            transform.position = position;
        }

        protected override void OnCombatBeginEvent()
        {
            base.OnCombatBeginEvent();
            isCombating = true;
            GetComponent<BoxCollider2D>().isTrigger = true;
        }
        private void Update()
        {
            inputDirection = playerController.Gameplay.Move.ReadValue<Vector2>();
            SwitchAnimation();
        }

        private void FixedUpdate()
        {
            if (!isCombating)
                Move();
        }

        private void Move()
        {
            //设置朝向
            SetPlayerFacingDirection(inputDirection.x);
            //移动
            var velocity = inputDirection * (UnityEngine.Time.deltaTime * CharacterData.Speed);
            isMoving = velocity.magnitude > 0;
            if (inputDirection.x != 0 && inputDirection.y != 0)
            {
                rigidBody2D.linearVelocity = velocity * math.sqrt(2) / 2;
            }
            rigidBody2D.linearVelocity = velocity;
        }

        private void SwitchAnimation()
        {
            animator.SetBool("isMoving", isMoving);
        }
        private void InputEnable()
        {
            playerController.Enable();
        }
        private void InputDisable()
        {
            playerController.Disable();
        }
        private void SelectFaShu(InputAction.CallbackContext FaShu)
        {
            //TODO:制作攻击的UI选择
            var index = Convert.ToInt32(FaShu.action.name[5].ToString());
            CombatUI.Instance.FaShuPanelUI.SelectFaShuSlot(index);
        }
    }
}
