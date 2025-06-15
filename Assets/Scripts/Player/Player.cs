using Unity.Mathematics;
using UnityEngine;

namespace TXDCL.Character
{
    public class Player : CharacterBase
    {
        private PlayerController playerController;
        public Vector2 inputDirection;
        public CharacterData test;
        public int Speed;
        

        protected override void Awake()
        {
            base.Awake();
            playerController = new PlayerController();
            GetComponent<Rigidbody2D>();
        }

        private void OnEnable()
        {
            playerController.Enable();
        }

        private void OnDisable()
        {
            playerController.Disable();
        }

        private void Update()
        {
            inputDirection = playerController.Gameplay.Move.ReadValue<Vector2>();
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
            var velocity = inputDirection * (Time.deltaTime * Speed);
            if (inputDirection.x != 0 && inputDirection.y != 0)
            {
                GetComponent<Rigidbody2D>().linearVelocity = velocity * math.sqrt(2) / 2;
            }

            GetComponent<Rigidbody2D>().linearVelocity = velocity;
        }
    }
}
