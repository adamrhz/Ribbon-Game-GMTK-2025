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
            get
            {
                return Rb.velocity.x;
            }

            set
            {
                Vector3 newVelocity = Rb.velocity;
                newVelocity.x = value;
                Rb.velocity = newVelocity;
            }
        }
        public float YSpeed
        {
            get
            {
                return Rb.velocity.y;
            }
            set
            {
                Vector3 newVelocity = Rb.velocity;
                newVelocity.y = value;
                Rb.velocity = newVelocity;
            }
        }

        public Rigidbody2D Rb;
        public PlayerStateMachine Machine;
        public PhysicsInfo PhysicsInfo;

        public PlayerCollision Collision;


        private void Awake()
        {
            Init();
            Machine.Init();
        }

        private void Init()
        {
            Rb = GetComponent<Rigidbody2D>();
            Machine = GetComponent<PlayerStateMachine>();
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
