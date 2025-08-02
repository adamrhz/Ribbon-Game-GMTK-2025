using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.Events;

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

        public int Health = 5;
        public int ButtonCount = 0;

        public Rigidbody2D Rb;
        public PlayerStateMachine Machine;
        public PhysicsInfo PhysicsInfo;
        public PlayerVisual Visual;
        public PlayerCollision Collision;
        public InputManager Input;
        public AudioBankHolder AudioBankHolder;

        public Collider2D PlayerCollider;
        public SpringJoint2D SwingJoint;

        public int Direction = 1;


        public UnityEvent<int> OnHealthChanged = new UnityEvent<int>();
        public UnityEvent<int> OnButtonAmountChanged = new UnityEvent<int>();

        private void Awake()
        {
            Instance = this;
            Application.targetFrameRate = 0;
        }

        public bool IsInvulnerable => _invulnerabilityTimer > 0f;

        private float _invulnerabilityTimer = 0f;

        public void Init()
        {
            Health = 5;
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
            Init();
            Input.BlockInput = true;

            if(!LevelManager.Instance)
            {
                Debug.LogError("LevelManager instance is null. Please ensure it is initialized before Player starts.");
                return;
            }
        }


        public void TriggerDamage()
        {
            if(IsInvulnerable || Machine.IsCurrentState<RB_PS_Death>()) return;
            SetHealth(Health - 1);
            if (Health <= 0)
            {
                TriggerDeath();
                return;
            }
            ToggleInvulnerability(1f);
            Machine.Set<RB_PS_Damage>();
        }

        public void SetHealth(int amount)
        {
            Health = amount;
            OnHealthChanged.Invoke(Health);
        }

        public void ToggleInvulnerability(float v)
        {
            _invulnerabilityTimer = v;
        }

        public void TriggerDeath()
        {
            if(Machine.IsCurrentState<RB_PS_Death>()) return;
            Machine.Set<RB_PS_Death>();
        }

        // Update is called once per frame
        void Update()
        {
            Controllers.Update();
            HandleTimer(ref _invulnerabilityTimer);

            //Debug.LogFormat("rb sqrvelocity: {0}", Rb.velocity.sqrMagnitude);
        }

        public bool HandleTimer(ref float timer)
        {
            if (timer > 0)
            {
                timer = Mathf.Clamp(timer - Time.deltaTime, 0f, timer);
                if(timer <= 0f)
                {
                    return true;
                }
            }

            return false;
        }

        public void AddButton()
        {
            ButtonCount++;
            OnButtonAmountChanged.Invoke(ButtonCount);
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
