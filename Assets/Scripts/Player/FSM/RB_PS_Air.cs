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
        public bool IsJumpDouble = false;

        private bool canApplyInput = false;

        public RB_PS_Air() : base(1)
        {
        }
        public override void OnEnter()
        {
            if (IsJump)
            {
                if (IsJumpDouble == false)
                {
                    Visual.SetTrigger("Jump");
                }
                
                JumpRequested = false;
                CanAscend = true;
            }
            else
            {
                CanAscend = false;
            }

            if (Machine.PreviousState is not RB_PS_Air or RB_PS_Swing)
            {
                CanDoubleJump = true;
            }

            canApplyInput = false;
            Player.StartCoroutine(ReenableInputs());
        }

        private IEnumerator ReenableInputs()
        {
            yield return new WaitForSeconds(0.05f);
            canApplyInput = true;
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
            Transform.rotation = Quaternion.Lerp(Transform.rotation,
                Quaternion.identity, 5 * Time.deltaTime);
            
            if(JumpRequested && CanDoubleJump)
            {
                Debug.Log("Double Jump requested");
                Player.YSpeed = PhysicsInfo.JumpStrength/1.5f;
                IsJump = true;
                IsJumpDouble = true;
                CanDoubleJump = false;
                JumpRequested = false;
                Visual.Play("Double Jump");
                Machine.Set<RB_PS_Air>();
                return;
            }

            AirMovement();
            GroundCheck();
                      

            Transform.localScale = Vector3.Lerp(Transform.localScale,
                Vector3.one, 10 * Time.deltaTime);
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
            if(IsJump && !CanAscend && Player.YSpeed > PhysicsInfo.JumpCutoff && Machine.PreviousState is not RB_PS_Swing)
            {
                Debug.Log("CutOff");
                Player.YSpeed = PhysicsInfo.JumpCutoff;
            }
            Player.YSpeed = Mathf.Clamp(Player.YSpeed - PhysicsInfo.Gravity * Time.fixedDeltaTime, -PhysicsInfo.MaxFallSpeed, Mathf.Infinity);

            Vector2 MoveInput = canApplyInput ? Input.GetAxis2D("Move") : Vector2.zero;
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