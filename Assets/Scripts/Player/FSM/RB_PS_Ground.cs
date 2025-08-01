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
            
            Visual.SquashAnimator.Play("SwitchDir", 0, 0);
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
                Machine.Get<RB_PS_Air>().CanDoubleJump = false;
                Machine.Set<RB_PS_Air>();
                return;
            }
            GroundMovement();

            Player.XSpeed = Mathf.Cos(Player.SurfaceAngle * Mathf.Deg2Rad) * Player.GroundSpeed;
            Player.YSpeed = Mathf.Sin(Player.SurfaceAngle * Mathf.Deg2Rad) * Player.GroundSpeed;
            
            GroundCheck();
        }

        private void GroundCheck()
        {
            if (!Collision.DoGroundCollision())
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
            Vector2 MoveInput = Input.GetAxis2D("Move");
            
            Debug.LogFormat("speed dir: {0}, input dir: {1}", Math.Sign(Player.GroundSpeed), Math.Sign(MoveInput.x));
            
            if (MoveInput.x == 0)
            {
                Player.GroundSpeed -= Mathf.Sign(Player.GroundSpeed) * Mathf.Min(PhysicsInfo.Friction * Time.fixedDeltaTime, Mathf.Abs(Player.GroundSpeed));
            }
            else if (SameDirection(MoveInput.x, Player.GroundSpeed))
            {
                ApplyAcceleration(PhysicsInfo.Acceleration, MoveInput.x, ref Player.GroundSpeed);
            }
            else
            {
                ApplyAcceleration(PhysicsInfo.Deceleration, MoveInput.x, ref Player.GroundSpeed);
            }
        }
    }
}