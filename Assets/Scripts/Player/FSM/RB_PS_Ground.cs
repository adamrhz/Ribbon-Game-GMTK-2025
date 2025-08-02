using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ribbon
{
    public class RB_PS_Ground : PlayerState
    {
        public float CoyoteTime;

        private bool _isIdle;
        private string _idleAnim = "IdleMad";
        private float _idleTimer;
        public RB_PS_Ground() : base(0)
        {
        }
        public override void OnEnter()
        {
            ZAngle = 0;
            Transform.localScale = Vector3.one;
            CoyoteTime = PhysicsInfo.CoyoteTime;
            JumpRequested = false;
            
            Visual.SquashAnimator.Play("SwitchDir", 0, 0);
        }

        public override void OnExit()
        {
            CoyoteTime = PhysicsInfo.CoyoteTime;
            JumpRequested = false;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            if(Player.GroundSpeed == 0 && !_isIdle)
            {
                _isIdle = true;
            }
            else if(Player.GroundSpeed != 0 && _isIdle)
            {
                _isIdle = false;
                _idleTimer = 0;
            }

            if (_isIdle && !Visual.IsPlaying(_idleAnim))
            {

                _idleTimer += Time.deltaTime;
                if(_idleTimer >= 6f)
                {
                    Visual.Play(_idleAnim);
                    _idleTimer = 0;
                }
            }
            SetDirection(Player.GroundSpeed);
        }

        public override void OnFixedUpdate()
        {
            if(JumpRequested)
            {
                Player.YSpeed = PhysicsInfo.JumpStrength;
                Machine.Get<RB_PS_Air>().IsJump = true;
                Machine.Get<RB_PS_Air>().CanDoubleJump = false;
                Machine.Get<RB_PS_Air>().IsJumpDouble = false;
                Machine.Set<RB_PS_Air>();
                return;
            }
            GroundCheck();
            GroundMovement();
            
            Player.XSpeed = Mathf.Cos(Player.SurfaceAngle * Mathf.Deg2Rad) * Player.GroundSpeed;
            Player.YSpeed = Mathf.Sin(Player.SurfaceAngle * Mathf.Deg2Rad) * Player.GroundSpeed;
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

            float maxSpeed = PhysicsInfo.MaxSpeed + Mathf.Sin(Player.SurfaceAngle * Mathf.Deg2Rad) * 3;
            
            if (MoveInput.x == 0)
            {
                Player.GroundSpeed -= Mathf.Sign(Player.GroundSpeed) * Mathf.Min(PhysicsInfo.Friction * Time.fixedDeltaTime, Mathf.Abs(Player.GroundSpeed));
            }
            else if (MoveInput.x > 0)
            {
                if (Player.GroundSpeed < 0)
                {
                    Player.GroundSpeed += PhysicsInfo.Deceleration * Time.fixedDeltaTime;
                }
                else if (Player.GroundSpeed < maxSpeed)
                {
                    Player.GroundSpeed = Mathf.Min(Player.GroundSpeed + PhysicsInfo.Acceleration * Time.fixedDeltaTime, maxSpeed);
                }
            }
            else
            {
                if (Player.GroundSpeed > 0)
                {
                    Player.GroundSpeed -= PhysicsInfo.Deceleration * Time.fixedDeltaTime;
                }
                else if (Player.GroundSpeed > -maxSpeed)
                {
                    Player.GroundSpeed = Mathf.Max(Player.GroundSpeed - PhysicsInfo.Acceleration * Time.fixedDeltaTime, -maxSpeed);
                }
            }
        }
    }
}