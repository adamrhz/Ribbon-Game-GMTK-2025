using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ribbon
{
    public class PlayerState : State
    {
        public Player Player;
        public PlayerStateMachine Machine;
        public Transform Transform;
        public Rigidbody2D Rb => Player.Rb;
        public PhysicsInfo PhysicsInfo => Player.PhysicsInfo;
        public PlayerCollision Collision => Player.Collision;

        public override void OnEnter()
        {

        }

        public override void OnExit()
        {

        }


        public void ApplyAcceleration(float acceleration, int Sign)
        {
            Player.XSpeed = Mathf.Clamp(Player.XSpeed + (acceleration * Sign * Time.fixedDeltaTime), -PhysicsInfo.MaxSpeed, PhysicsInfo.MaxSpeed);
        }

        public bool SameDirection(float XInput)
        {
            return Player.XSpeed == 0 || Mathf.Sign(XInput) == Mathf.Sign(Player.XSpeed);
        }

    }
}