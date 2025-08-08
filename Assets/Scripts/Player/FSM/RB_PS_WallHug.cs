using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ribbon
{
    public class RB_PS_WallHug : PlayerState
    {

        public int PreviousWallDirection = 0;
        public bool CanAttach = true;

        public RB_PS_WallHug() : base(7)
        {
        }

        public void ResetWallCling()
        {
            CanAttach = true;
            PreviousWallDirection = 0;
        }

        public bool CanAttachToWall(int WallDirection)
        {
            return (WallDirection != PreviousWallDirection) && CanAttach;
        }

        public void AttachToWall(int WallDirection)
        {
            if (!CanAttachToWall(WallDirection))
            {
                return;
            }
            PreviousWallDirection = WallDirection;
            Machine.Set<RB_PS_WallHug>();
        }

        public override void OnEnter()
        {
            if(Player.YSpeed > 0)
            {
                Player.YSpeed = PhysicsInfo.JumpCutoff/2;
            }else if(Player.YSpeed < 0)
            {
                Player.YSpeed = 0;
            }
            Player.XSpeed = 0;
        }


        public override void OnExit()
        {
        }

        public override void OnUpdate()
        {
            CheckInput();
        }

        public void CheckInput()
        {
            if (Input.GetButtonDown(GamePreference.JumpButton))
            {
                Debug.Log("Bruh");
                Player.Direction = -PreviousWallDirection;
                Player.XSpeed = Mathf.Cos(45) * PhysicsInfo.JumpStrength * -PreviousWallDirection;
                Player.YSpeed = Mathf.Sin(45) * PhysicsInfo.JumpStrength;
                Machine.Set<RB_PS_Air>();
                Machine.Get<RB_PS_Air>().CanDoubleJump = false;
                Player.ObjectDisableInput(.15f);
                Player.AudioBankHolder.Play("JumpVoice");
                Visual.Play("Double Jump");
            }
        }

        public override void OnFixedUpdate()
        {


            Player.YSpeed = Mathf.Clamp(Player.YSpeed - PhysicsInfo.Gravity/6 * Time.fixedDeltaTime, -PhysicsInfo.MaxFallSpeed, Mathf.Infinity);
            if(Player.YSpeed < -PhysicsInfo.Gravity)
            {
                CanAttach = false;
                Machine.Set<RB_PS_Air>();
                return;
            }
            GroundCheck();




        }
        private void GroundCheck()
        {
            if (Collision.DoAirGroundCollision() && Player.YSpeed <= 0)
            {
                Player.YSpeed = 0;
                Machine.Set<RB_PS_Ground>();
                Player.GroundSpeed = Player.XSpeed;
            }else if (!Collision.DoWallCollision())
            {
                Machine.Set<RB_PS_Air>();
            }
        }

    }
}