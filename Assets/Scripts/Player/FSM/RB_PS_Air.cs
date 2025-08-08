using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

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
            if (IsJump || IsJumpDouble)
            {
                if (IsJumpDouble == false)
                {
                    Player.AudioBankHolder.Play("JumpVoice");
                    Visual.Play("Jump");
                }
                Player.AudioBankHolder.Play("Jump");
                Visual.SquashAnimator.Play("Jump", 0, 0);
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
            JumpRequested = false;
            IsJump = false;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            CheckInput();
            SetDirection(Player.XSpeed);
        }

        public void CheckInput()
        {
            if (IsJump && CanAscend && !Input.GetButton(GamePreference.JumpButton))
            {
                CanAscend = false;
            }
        }

        public override void OnFixedUpdate()
        {
            Transform.rotation = Quaternion.Lerp(Transform.rotation,
                Quaternion.identity, 5 * Time.deltaTime);

            if (JumpRequested && CanDoubleJump)
            {
                Player.YSpeed = PhysicsInfo.JumpStrength * PhysicsInfo.DoubleJumpMultiplier;
                IsJump = true;
                IsJumpDouble = true;
                CanDoubleJump = false;
                JumpRequested = false;
                Visual.Play("Double Jump");
                Visual.SquashAnimator.Play("Jump", 0, 0);
                Machine.Set<RB_PS_Air>();
                return;
            }
            AirMovement();
            if (GroundCheck())
            {
                return;
            }
            WallHugCheck();
            Transform.localScale = Vector3.Lerp(Transform.localScale,
                Vector3.one, 10 * Time.deltaTime);
        }

        private void WallHugCheck()
        {
            if (Collision.DoWallCollision() && Player.YSpeed <= PhysicsInfo.JumpCutoff)
            {
                Machine.Get<RB_PS_WallHug>().AttachToWall(Player.Direction);
            }
        }

        private bool GroundCheck()
        {
            if (Player.YSpeed <= PhysicsInfo.JumpCutoff)
            {
                if (Collision.DoAirGroundCollision())
                {
                    Player.YSpeed = 0;
                    Machine.Set<RB_PS_Ground>();
                    Player.GroundSpeed = Player.XSpeed;
                    return true;
                }
            }
            return false;
        }


        private void AirMovement()
        {
            if (IsJump && !CanAscend && Player.YSpeed > PhysicsInfo.JumpCutoff && Machine.PreviousState is not RB_PS_Swing)
            {
                Player.YSpeed = PhysicsInfo.JumpCutoff;
            }
            Player.YSpeed = Mathf.Clamp(Player.YSpeed - PhysicsInfo.Gravity * Time.fixedDeltaTime, -PhysicsInfo.MaxFallSpeed, Mathf.Infinity);

            Vector2 MoveInput = canApplyInput ? Input.GetAxis2D(GamePreference.MoveInput) : Vector2.zero;
            int Sign = (int)Mathf.Sign(MoveInput.x);
            float targetXSpeed = Player.XSpeed;

            if (MoveInput.x == 0)
            {
                targetXSpeed -= Mathf.Sign(targetXSpeed) * Mathf.Min(PhysicsInfo.AirDrag * Time.fixedDeltaTime, Mathf.Abs(targetXSpeed));
            }
            else if (Player.InputDisableTimer <= 0)
            {
                if (Mathf.Approximately(Mathf.Sign(MoveInput.x), Mathf.Sign(targetXSpeed)))
                {
                    if (Mathf.Abs(targetXSpeed) < PhysicsInfo.MaxSpeed)
                        targetXSpeed += Mathf.Sign(MoveInput.x) *
                                        PhysicsInfo.AirAcceleration
                                        * Time.fixedDeltaTime;
                }
                else
                {
                    targetXSpeed += Mathf.Sign(MoveInput.x) *
                                    PhysicsInfo.AirDeceleration
                                    * Time.fixedDeltaTime;
                }
            }

            Player.XSpeed = targetXSpeed;
        }
    }
}