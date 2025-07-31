using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ribbon
{
    public class RB_PS_Air : PlayerState
    {
        public bool IsJump = false;
        public bool CanDoubleJump = false;
        public bool CanAscend = false;

        public RB_PS_Air() : base(1)
        {
        }
        public override void OnEnter()
        {
            if (IsJump)
            {
                Visual.SetTrigger("Jump");
                JumpRequested = false;
                CanAscend = true;
            }
            else
            {

                CanAscend = false;
            }

            if (Machine.PreviousState is not RB_PS_Air)
            {
                CanDoubleJump = true;
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
            if (IsJump && CanAscend && !Input.GetButton("Jump"))
            {
                CanAscend = false;
            }
        }

        public override void OnFixedUpdate()
        {
            if(JumpRequested && CanDoubleJump)
            {
                Player.YSpeed = PhysicsInfo.JumpStrength/2f;
                IsJump = true;
                CanDoubleJump = false;
                JumpRequested = false;
                Machine.Set<RB_PS_Air>();
                return;
            }
            AirMovement();
            GroundCheck();

        }
        private void GroundCheck()
        {
            Debug.Log("Checking if grounded in air state");
            if (Collision.DoAirCollision() && Player.YSpeed <= 0)
            {
                Player.YSpeed = 0;
                Debug.Log("Grounded in air state, switching to ground state");
                Machine.Set<RB_PS_Ground>();
                Player.GroundSpeed = Player.XSpeed;
            }
        }


        private void AirMovement()
        {
            if(IsJump && !CanAscend && Player.YSpeed > PhysicsInfo.JumpCutoff)
            {
                Player.YSpeed = PhysicsInfo.JumpCutoff;
            }
            Player.YSpeed = Mathf.Clamp(Player.YSpeed - PhysicsInfo.Gravity * Time.fixedDeltaTime, -PhysicsInfo.MaxFallSpeed, Mathf.Infinity);


            Vector2 MoveInput = Input.GetAxis2D("Move");
            int Sign = (int)Mathf.Sign(MoveInput.x);
            float targetXSpeed = Player.XSpeed;
            
            if (MoveInput.x == 0)
            {
                Player.XSpeed -= Mathf.Sign(Player.XSpeed) * Mathf.Min(PhysicsInfo.AirDrag * Time.fixedDeltaTime, Mathf.Abs(Player.XSpeed));
            }
            else if (SameDirection(MoveInput.x, Player.XSpeed))
            {
                ApplyAcceleration(PhysicsInfo.AirAcceleration, Sign, ref targetXSpeed);
                Player.XSpeed = targetXSpeed;
            }
            else
            {
                ApplyAcceleration(PhysicsInfo.AirDeceleration, Sign, ref targetXSpeed);
                Player.XSpeed = targetXSpeed;
            }
        }
    }
}