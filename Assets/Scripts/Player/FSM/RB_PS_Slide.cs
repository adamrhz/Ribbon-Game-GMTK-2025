using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ribbon
{
    public class RB_PS_Slide : RB_PS_Ground
    {

        public RB_PS_Slide() : base(3)
        {
        }
        public override void OnEnter()
        {
            base.OnEnter();
            CapsuleCollider2D Collider = Player.PlayerCollider as CapsuleCollider2D;
            if (!Collider)
            {
                return;
            }
            Collider.offset = PhysicsInfo.SlidingBounds.offset;
            Collider.size = PhysicsInfo.SlidingBounds.size;
        }


        public override void OnExit()
        {
            base.OnExit();
            CapsuleCollider2D Collider = Player.PlayerCollider as CapsuleCollider2D;
            if (!Collider)
            {
                return;
            }
            Collider.offset = PhysicsInfo.StandingBounds.offset;
            Collider.size = PhysicsInfo.StandingBounds.size;
        }

        public override void OnUpdate()
        {
            CheckInput();
            SetDirection(Player.GroundSpeed);
        }

        public void CheckInput()
        {
            if (Input.BlockInput)
            {
                return;
            }
            if (Collision.DoSlideCeilingCollision())
            {
                return;
            }
            if (Input.GetAxis2D(GamePreference.MoveInput).y >= 0)
            {
                Machine.Set<RB_PS_Ground>();
                return;
            }

            if (Input.GetButtonDown("Jump"))
            {
                JumpRequested = true;
            }
        }

        public override void OnFixedUpdate()
        {
            if (JumpRequested)
            {
                Player.YSpeed = PhysicsInfo.JumpStrength;
                Machine.Get<RB_PS_Air>().IsJump = true;
                Machine.Get<RB_PS_Air>().CanDoubleJump = false;
                Machine.Get<RB_PS_Air>().IsJumpDouble = false;
                Machine.Set<RB_PS_Air>();
                return;
            }


            SlopeRepel();
            if (!Machine.IsCurrentState<RB_PS_Slide>())
            {
                return;
            }
            GroundCheck();
            if (Collision.DoSlideCeilingCollision() && Mathf.Abs(Player.GroundSpeed) < 3)
            {
                Player.GroundSpeed = Player.Direction * 3;
            }



            Player.XSpeed = Mathf.Cos(Player.SurfaceAngle * Mathf.Deg2Rad) * Player.GroundSpeed;
            Player.YSpeed = Mathf.Sin(Player.SurfaceAngle * Mathf.Deg2Rad) * Player.GroundSpeed;

        }
        private void SlopeRepel()
        {
            if (Player.GroundSpeed != 0)
            {
                Player.GroundSpeed -= Mathf.Sign(Player.Direction) * Mathf.Min(PhysicsInfo.Friction/10 * Time.fixedDeltaTime, Mathf.Abs(Player.GroundSpeed));
                if (Mathf.Abs(Player.GroundSpeed) < .0001f)
                {
                    Player.GroundSpeed = 0;
                }
            }
            float AngleValue = Time.fixedDeltaTime * Mathf.Sin(Player.SurfaceAngle * Mathf.Deg2Rad);
            if (Mathf.Sign(Player.GroundSpeed) == Mathf.Sign(AngleValue))
            {
                AngleValue *= PhysicsInfo.SlopeRepelUpHill * 3;
            }
            else
            {
                AngleValue *= PhysicsInfo.SlopeRepelDownHill * 2;
            }

            Player.GroundSpeed -= AngleValue;

            if (Mathf.Abs(Player.GroundSpeed) > PhysicsInfo.TopSpeed)
            {
                Player.GroundSpeed = PhysicsInfo.TopSpeed * Player.Direction;
            }
            if (Mathf.Abs(Player.GroundSpeed) < 1 && AngleValue == 0)
            {
                Machine.Set<RB_PS_Ground>();
                return;
            }

        }
        private bool GroundCheck()
        {
            if (!Collision.DoGroundCollision())
            {
                CoyoteTime -= Time.fixedDeltaTime;
                if (CoyoteTime <= 0)
                {
                    Machine.Set<RB_PS_Air>();
                    return false;
                }
            }
            else
            {
                CoyoteTime = PhysicsInfo.CoyoteTime;
            }
            return true;
        }

    }
}