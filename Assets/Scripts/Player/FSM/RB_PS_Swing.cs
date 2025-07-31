using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ribbon
{
    public class RB_PS_Swing : PlayerState
    {
        public float RelativeSpeed;
        public GameObject SwingingTarget;

        public RB_PS_Swing() : base(2)
        {
        }
        public override void OnEnter()
        {
            if (SwingingTarget && SwingingTarget.TryGetComponent(out Rigidbody2D rb2d))
            {
                Player.SwingJoint.connectedBody = rb2d;
                Player.SwingJoint.enabled = true;
                Player.SwingJoint.distance = Vector3.Distance(SwingingTarget.transform.position, Player.transform.position);
                Vector3 vel = Vector3.ProjectOnPlane(Rb.velocity, (SwingingTarget.transform.position-Player.transform.position).normalized);

                int Direction = (int)Mathf.Sign(Vector3.Dot(vel, Vector3.ProjectOnPlane(Rb.velocity, (SwingingTarget.transform.position - Player.transform.position).normalized)));
                RelativeSpeed = vel.magnitude * Direction;
                RelativeSpeed = 0;
            }
            else
            {
                Machine.Set<RB_PS_Air>();
            }
        }

        public override void OnExit()
        {
            Player.SwingJoint.enabled = false;
            Player.SwingJoint.connectedBody = null;
            SwingingTarget = null;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            CheckInput();
        }

        public void CheckInput()
        {
            if (!Input.GetButton("Jump"))
            {
                Machine.Get<RB_PS_Air>().IsJump = false;
                Machine.Set<RB_PS_Air>();
            }
        }

        public override void OnFixedUpdate()
        {
            if(!Machine.IsCurrentState<RB_PS_Swing>())
            {
                return;
            }
            AirMovement();
            GroundCheck();

        }
        private void GroundCheck()
        {
            Debug.Log("Checking if grounded in air state");
            if (Collision.DoAirCollision())
            {
                Player.YSpeed = 0;
                Player.GroundSpeed = RelativeSpeed;
                Debug.Log("Grounded in air state, switching to ground state");
                Machine.Set<RB_PS_Ground>();
                Player.GroundSpeed = Player.XSpeed;
            }
        }


        private void AirMovement()
        {

            Vector2 toPivot = (SwingingTarget.transform.position - Player.transform.position).normalized;
            float angle = Mathf.Atan2(-toPivot.x, toPivot.y) * Mathf.Rad2Deg;
            float ropeLength = Vector3.Distance(SwingingTarget.transform.position, Player.transform.position);
            float angularAcceleration = (-PhysicsInfo.Gravity / ropeLength) * Mathf.Sin(angle) - (0.1f * RelativeSpeed);


            RelativeSpeed += angularAcceleration * Time.fixedDeltaTime;

            Debug.Log(angle);
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

            Vector2 tangent = Vector3.Cross(toPivot, Vector3.forward);

            Vector2 tangentialVelocity = tangent * (RelativeSpeed * ropeLength);
            Vector2 gravityVelocity = new(0, PhysicsInfo.Gravity * Time.fixedDeltaTime);
            //Too lazy this shit is ass frl;
            //Strix do it pls -Adam
            Rb.velocity = tangentialVelocity;

        }
    }
}