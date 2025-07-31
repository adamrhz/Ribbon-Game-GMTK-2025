using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ribbon
{
    public class RB_PS_Air : PlayerState
    {
        public bool IsJump = false;
        public bool DoubleJump = false;
        public bool CanAscend = false;

        public override void OnEnter()
        {
            if (IsJump)
            {
                JumpRequested = false;
                CanAscend = true;
            }
        }

        public override void OnExit()
        {
            IsJump = false;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            CheckInput();
        }

        public void CheckInput()
        {

            if (IsJump && CanAscend && !Input.GetKey(KeyCode.Space))
            {
                CanAscend = false;
            }
        }

        public override void OnFixedUpdate()
        {
            if(JumpRequested && !DoubleJump)
            {
                Player.YSpeed = PhysicsInfo.JumpStrength/1.25f;
                IsJump = true;
                DoubleJump = true;
                Machine.Set<RB_PS_Air>();
                return;
            }
            AirMovement();
            GroundCheck();

        }
        private void GroundCheck()
        {
            Debug.Log("Checking if grounded in air state");
            if (Collision.IsGrounded() && Player.YSpeed <= 0)
            {
                Player.YSpeed = 0;
                Debug.Log("Grounded in air state, switching to ground state");
                Machine.Set<RB_PS_Ground>();
            }
        }


        private void AirMovement()
        {
            if(IsJump && !CanAscend && Player.YSpeed > PhysicsInfo.JumpCutoff)
            {
                Player.YSpeed = PhysicsInfo.JumpCutoff;
            }
            Player.YSpeed = Mathf.Clamp(Player.YSpeed - PhysicsInfo.Gravity * Time.fixedDeltaTime, -PhysicsInfo.MaxFallSpeed, Mathf.Infinity);


            Vector2 MoveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            int Sign = (int)Mathf.Sign(MoveInput.x);
            if (MoveInput.x == 0)
            {
                Player.XSpeed -= Mathf.Sign(Player.XSpeed) * Mathf.Min(PhysicsInfo.Friction * Time.fixedDeltaTime, Mathf.Abs(Player.XSpeed));
            }
            else if (SameDirection(MoveInput.x))
            {
                ApplyAcceleration(PhysicsInfo.AirAcceleration, Sign);
            }
            else
            {
                ApplyAcceleration(PhysicsInfo.AirDeceleration, Sign);
            }
        }
    }
}