using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ribbon
{
    public class Player : MonoBehaviour
    {

        public static Player Instance { get; private set; }
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
            set => Rb.velocity = new Vector2(Rb.velocity.x, value);
        }

        public float xSpeed;

        [field: SerializeField] 
        public float SurfaceAngle { get; set; }

        [field: SerializeField] public float GroundSpeed;
        
        public PlayerControllers Controllers = new PlayerControllers();

        public Rigidbody2D Rb;
        public PlayerStateMachine Machine;
        public PhysicsInfo PhysicsInfo;
        public PlayerVisual Visual;
        public PlayerCollision Collision;
        public InputManager Input;
        public AudioBankHolder AudioBankHolder;

        public SpringJoint2D SwingJoint;

        public int Direction = 1;
        private void Awake()
        {
            Instance = this;
            //Application.targetFrameRate = 30;
        }
        
        public void Init()
        {
            Direction = 1;
            Rb = GetComponent<Rigidbody2D>();
            Machine = GetComponent<PlayerStateMachine>();
            Visual = GetComponent<PlayerVisual>();
            Machine.Init();
            Controllers.Init(this);
            transform.position = GameObject.Find("SpawnPoint")?.transform.position ?? Vector3.zero;
        }

        // Start is called before the first frame update
        void Start()
        {
            if(LevelManager.Instance == null)
            {
                Init();
                Debug.LogError("LevelManager instance is null. Please ensure it is initialized before Player starts.");
                return;
            }
        }

        // Update is called once per frame
        void Update()
        {
            Controllers.Update();
            
            //Debug.LogFormat("rb sqrvelocity: {0}", Rb.velocity.sqrMagnitude);
        }


        public void OnObject(RWorldObject2D Obj, bool ToAir)
        {
            if (ToAir)
            {
                Machine.Get<RB_PS_Air>().CanDoubleJump = true;
                Machine.Get<RB_PS_Air>().IsJump = false;
                Machine.Set<RB_PS_Air>(); return;
            }
        }
    }
}
