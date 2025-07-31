using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ribbon
{
    public class Player : MonoBehaviour
    {
        public float XSpeed
        {
            get => Rb.velocity.x;

            set
            {
                /*Vector3 newVelocity = Rb.velocity;
                newVelocity.x = value;
                Rb.velocity = newVelocity;*/

                Rb.velocity = new Vector2(value, Rb.velocity.y);
            }
        }
        public float YSpeed
        {
            get => Rb.velocity.y;
            set
            {
                Vector3 newVelocity = Rb.velocity;
                newVelocity.y = value;
                Rb.velocity = newVelocity;
            }
        }

        public float xSpeed;

        [field: SerializeField] 
        public float SurfaceAngle { get; set; }

        [field: SerializeField] public float GroundSpeed;
        
        public Rigidbody2D Rb;
        public PlayerStateMachine Machine;
        public PhysicsInfo PhysicsInfo;
        public PlayerVisual Visual;
        public PlayerCollision Collision;
        public InputManager Input;


        private void Awake()
        {
            Init();
            Machine.Init();
        }

        private void Init()
        {
            Rb = GetComponent<Rigidbody2D>();
            Machine = GetComponent<PlayerStateMachine>();
            Visual= GetComponent<PlayerVisual>();
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
