using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ribbon
{
    public class RB_PS_Ground : PlayerState
    {
        public float CoyoteTime;



        public RB_PS_Ground() : base(0)
        {
        }
        public override void OnEnter()
        {
            CoyoteTime = PhysicsInfo.CoyoteTime;
            JumpRequested = false;
        }

        public override void OnExit()
        {
            CoyoteTime = PhysicsInfo.CoyoteTime;
            JumpRequested = false;
        }


        public override void OnFixedUpdate()
        {
            if(JumpRequested)
            {
                Player.YSpeed = PhysicsInfo.JumpStrength;
                Machine.Get<RB_PS_Air>().IsJump = true;
                Machine.Get<RB_PS_Air>().DoubleJump = false;
                Machine.Set<RB_PS_Air>();
                return;
            }
            GroundMovement();
            GroundCheck();

        }

        private void GroundCheck()
        {
            if (!Collision.IsGrounded())
            {
                CoyoteTime -= Time.fixedDeltaTime;
                if(CoyoteTime <= 0)
                {
                    Machine.Set<RB_PS_Air>();
                }
            }
            else
            {
                CoyoteTime = PhysicsInfo.CoyoteTime;
            }
        }

        private void GroundMovement()
        {
            Vector2 MoveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            int Sign = (int)Mathf.Sign(MoveInput.x);
            if (MoveInput.x == 0)
            {
                Player.XSpeed -= Mathf.Sign(Player.XSpeed) * Mathf.Min(PhysicsInfo.Friction * Time.fixedDeltaTime, Mathf.Abs(Player.XSpeed));
            }
            else if (SameDirection(MoveInput.x))
            {
                ApplyAcceleration(PhysicsInfo.Acceleration, Sign);
            }
            else
            {
                ApplyAcceleration(PhysicsInfo.Deceleration, Sign);
            }
        }
    }
}