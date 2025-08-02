using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ribbon
{
    public class RB_PS_Damage : PlayerState
    {

        public RB_PS_Damage() : base(5)
        {
        }
        public override void OnEnter()
        {

            Player.Rb.velocity = new(5 * -Player.Direction, 5);

            Input.BlockInput = true;
        }


        public override void OnExit()
        {
            Input.BlockInput = false;
        }

        public override void OnUpdate()
        {
        }

        public void CheckInput()
        {
        }

        public override void OnFixedUpdate()
        {


            Player.YSpeed = Mathf.Clamp(Player.YSpeed - PhysicsInfo.Gravity * Time.fixedDeltaTime, -PhysicsInfo.MaxFallSpeed, Mathf.Infinity);
            GroundCheck();



        }
        private void GroundCheck()
        {
            if (Collision.DoAirCollision() && Player.YSpeed <= 0)
            {
                Player.YSpeed = 0;
                Machine.Set<RB_PS_Ground>();
                Player.GroundSpeed = Player.XSpeed;
            }
        }

    }
}