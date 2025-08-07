using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

namespace Ribbon
{
    public class RB_PS_Swing : PlayerState
    {
        public float RelativeSpeed;
        public GameObject SwingingTarget;

        private float calculatedDistance;


        public int Direction;
        public RB_PS_Swing() : base(2)
        {
        }
        public override void OnEnter()
        {
            Rb.gravityScale = 0;
            JumpRequested = false;
            SwingTangent = Rb.velocity.normalized;
            swingLerpIntensity = 0;
            if (SwingingTarget && SwingingTarget.TryGetComponent(out Rigidbody2D rb2d))
            {
                Player.SwingJoint.connectedBody = rb2d;
                Player.SwingJoint.enabled = true;
                calculatedDistance = Vector3.Distance(SwingingTarget.transform.position, Player.transform.position);
                Vector3 vel = Vector3.ProjectOnPlane(Rb.velocity, (SwingingTarget.transform.position-Player.transform.position).normalized);

                int Direction = (int)Mathf.Sign(Vector3.Dot(vel, Vector3.ProjectOnPlane(Rb.velocity, (SwingingTarget.transform.position - Player.transform.position).normalized)));
                RelativeSpeed = vel.magnitude * Direction;
                // ???
                // RelativeSpeed = 0;
            }
            else
            {
                Machine.Set<RB_PS_Air>();
                return;
            }

            Visual.ToggleIndicatorLine(null);
            Player.SwingJoint.frequency = 0.01f;

            if(UnityEngine.Random.Range(0, 2) == 0)
            {
                Player.AudioBankHolder.Play("Swing");
            }

            if (SwingingTarget.TryGetComponent(out SwingGoal SwingGoal))
            {
                SwingGoal.OnStartSwinging(Player);
            }
        }

        public override void OnExit()
        {


            Visual.ToggleRibbonAttachLine(null);
            Player.SwingJoint.enabled = false;
            Player.SwingJoint.connectedBody = null;


            if (SwingingTarget.TryGetComponent(out SwingGoal SwingGoal))
            {
                SwingGoal.OnStopSwinging(Player);
            }

            SwingingTarget = null;
            
            Rb.gravityScale = 1;
            Machine.Get<RB_PS_Air>().CanDoubleJump = true;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            CheckInput();
            if(SwingingTarget != null)
            {
                Visual.ToggleRibbonAttachLine(SwingingTarget.transform.position);
            }
            Player.SwingJoint.distance = Mathf.Lerp(Player.SwingJoint.distance, calculatedDistance / 1.2f / (1 + Rb.velocity.magnitude / 40), 1.2f * Time.deltaTime);
            Player.SwingJoint.frequency = Mathf.Lerp(Player.SwingJoint.frequency, 10, 0.8f * Time.deltaTime);

            Transform.localScale = Vector3.Lerp(Transform.localScale,
                Vector3.Lerp(Vector3.one, new Vector3(1.5f, 0.7f, 1f), Mathf.Clamp01(Rb.velocity.sqrMagnitude / 200)),
                10 * Time.deltaTime);

            swingLerpIntensity = Mathf.Lerp(swingLerpIntensity, 14, 4 * Time.deltaTime);


            SetDirection(Direction);

        }

        public void CheckInput()
        {
            if (!Input.GetButton("Jump"))
            {

                Machine.Get<RB_PS_Air>().IsJump = false;
                Machine.Get<RB_PS_Air>().JumpRequested = false;
                Machine.Set<RB_PS_Air>();
                Visual.Play("Double Jump");
                Player.ObjectDisableInput(.25f);
            }
        }

        public override void OnFixedUpdate()
        {
            if(!Machine.IsCurrentState<RB_PS_Swing>()){return;}
            ZAngle = Mathf.LerpAngle(ZAngle, SwingAngle * Mathf.Rad2Deg, 5 * (1 + Rb.velocity.magnitude / 3) * Time.deltaTime);
            AirMovement();
            if (!Machine.IsCurrentState<RB_PS_Swing>()) { return; }
            GroundCheck();

        }
        private void GroundCheck()
        {
            if (Collision.DoAirGroundCollision())
            {
                Player.YSpeed = 0;
                Player.GroundSpeed = RelativeSpeed;
                Machine.Set<RB_PS_Ground>();
                Player.GroundSpeed = Player.XSpeed;
            }
        }
        private float previousSwingAngle;
        public float SwingAngle;
        public float deltaAngle;

        public Vector2 SwingTangent, SwingNormal;
        private float swingLerpIntensity;
        private Vector2 previousVelocity;

        private void AirMovement()
        {
            previousVelocity = Rb.velocity;
            
            SwingNormal = (SwingingTarget.transform.position - Player.transform.position).normalized;
            SwingAngle = Mathf.Atan2(-SwingNormal.x, SwingNormal.y);
            deltaAngle = Mathf.DeltaAngle(previousSwingAngle * Mathf.Rad2Deg, SwingAngle * Mathf.Rad2Deg) * Mathf.Deg2Rad;
            previousSwingAngle = SwingAngle;

            float ropeLength = Vector3.Distance(SwingingTarget.transform.position, Player.transform.position);
            float angularAcceleration = (PhysicsInfo.Gravity / ropeLength) * Mathf.Sin(SwingAngle); //adam youre getting the sine of a deg angle using a rad function
            
            RelativeSpeed += angularAcceleration * Time.fixedDeltaTime;
            Vector2 MoveInput = Input.GetAxis2D("Move");
            int Sign = (int)Mathf.Sign(MoveInput.x);
            
            //if (MoveInput.x == 0)
            //{
            //    RelativeSpeed -= Mathf.Sign(RelativeSpeed) * Mathf.Min(PhysicsInfo.AirDrag * Time.fixedDeltaTime, Mathf.Abs(RelativeSpeed));
            //}
            //else if (SameDirection(MoveInput.x, RelativeSpeed))
            //{
            //    ApplyAcceleration(PhysicsInfo.AirAcceleration, Sign, ref RelativeSpeed);
            //}
            //else
            //{
            //    ApplyAcceleration(PhysicsInfo.AirDeceleration, Sign, ref RelativeSpeed);
            //}

            // you do know there's a perpendicular function in unity2d right
            // Vector2 tangent = Vector3.Cross(toPivot, Vector3.forward);
            SwingTangent = -Vector2.Perpendicular(SwingNormal.normalized);

            Vector2 tangentialVelocity = SwingTangent * (RelativeSpeed * ropeLength);
            Vector2 gravityVelocity = new(0, PhysicsInfo.Gravity * Time.fixedDeltaTime);
            //Too lazy this shit is ass frl;
            //Strix do it pls -Adam
            Rb.velocity += Vector2.down * (PhysicsInfo.Gravity / ropeLength * 2.6f * Time.fixedDeltaTime);
            Rb.AddForce(SwingTangent * MoveInput.x * (200 * ropeLength) * Time.fixedDeltaTime, ForceMode2D.Force);
            Rb.velocity = Vector2.ClampMagnitude(Rb.velocity, PhysicsInfo.MaxSwingSpeed);
            Debug.DrawLine(Transform.position, (Vector2)Transform.position + SwingTangent.normalized);

            Direction = (int)Mathf.Sign(Vector2.Dot(Rb.velocity, SwingTangent));
            if(SwingingTarget.TryGetComponent(out SwingGoal SwingGoal))
            {
                SwingGoal.WhileSwinging(Player);
            }
        }
    }
}