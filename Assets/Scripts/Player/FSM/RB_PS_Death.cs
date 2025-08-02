using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ribbon
{
    public class RB_PS_Death : PlayerState
    {
        public float DeathTimer = 2f;

        public RB_PS_Death() : base(4)
        {
        }
        public override void OnEnter()
        {
            DeathTimer = 2f;
            LevelManager.Instance.TimerActive = false;
            Input.BlockInput = true;
            Player.PlayerCollider.enabled = false;
            Rb.velocity = new Vector2(0, 10);
        }


        public override void OnExit()
        {
            Player.SetHealth(5);
            Input.BlockInput = false;
            LevelManager.Instance.TimerActive = true;
            Player.ToggleInvulnerability(1);
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

            if (DeathTimer > 0)
            {
                DeathTimer -= Time.fixedDeltaTime;
            }
            else
            {
                Player.PlayerCollider.enabled = true;
                Player.Rb.velocity = Vector2.zero;
                Player.Rb.angularVelocity = 0f;
                Player.transform.position = GameObject.Find("SpawnPoint")?.transform.position ?? Vector3.zero;
                Player.Machine.Set<RB_PS_Ground>();
            }



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