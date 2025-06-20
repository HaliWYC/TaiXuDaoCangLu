using Unity.Mathematics;
using UnityEngine;
namespace TXDCL.Character
{
    public class Player : CharacterBase
    {
        private PlayerController playerController;
        private Vector2 inputDirection;
        [SerializeField] private Animator animator;

        protected override void Awake()
        {
            base.Awake();
            playerController = new PlayerController();
            GetComponent<Rigidbody2D>();
        }

        private void OnEnable()
        {
            playerController.Enable();
            EventHandler.BeforeSceneLoadEvent += OnBeforeSceneLoadEvent;
            EventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
            EventHandler.MoveToPositionEvent += OnMoveToPositionEvent;
            EventHandler.BeforeCombatBeginEvent += OnBeforeCombatBeginEvent;
        }

        private void OnBeforeCombatBeginEvent()
        {
            InputDisable();
        }

        private void OnDisable()
        {
            playerController.Disable();
            EventHandler.BeforeSceneLoadEvent -= OnBeforeSceneLoadEvent;
            EventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadEvent;
            EventHandler.MoveToPositionEvent -= OnMoveToPositionEvent;
            EventHandler.BeforeCombatBeginEvent -= OnBeforeCombatBeginEvent;
        }
        
        private void OnBeforeSceneLoadEvent()
        {
            GetComponent<BoxCollider2D>().isTrigger = true;
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

        private void Update()
        {
            inputDirection = playerController.Gameplay.Move.ReadValue<Vector2>();
            SwitchAnimation();
        }

        private void FixedUpdate()
        {
            Move();
        }

        private void Move()
        {
            //设置朝向
            int faceDir = (int)transform.localScale.x;
            if (inputDirection.x > 0)
                faceDir = 1;
            else if (inputDirection.x < 0)
                faceDir = -1;
            transform.localScale = new Vector3(faceDir, transform.localScale.y, transform.localScale.z);

            //移动
            var velocity = inputDirection * (UnityEngine.Time.deltaTime * CharacterData.Speed);
            if (inputDirection.x != 0 && inputDirection.y != 0)
            {
                GetComponent<Rigidbody2D>().linearVelocity = velocity * math.sqrt(2) / 2;
            }

            GetComponent<Rigidbody2D>().linearVelocity = velocity;
        }

        private void SwitchAnimation()
        {
            
        }

        private void InputEnable()
        {
            playerController.Enable();
        }
        private void InputDisable()
        {
            playerController.Disable();
        }
    }
}
