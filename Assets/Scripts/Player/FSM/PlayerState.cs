using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ribbon
{
    public class PlayerState : State
    {
        public int StateNumber = -1;
        public Player Player;
        public PlayerStateMachine Machine;
        public Transform Transform;
        public Rigidbody2D Rb => Player.Rb;
        public PhysicsInfo PhysicsInfo => Player.PhysicsInfo;
        public PlayerCollision Collision => Player.Collision;
        public PlayerVisual Visual => Player.Visual;
        public InputManager Input => Player.Input;
        public bool JumpRequested = false;

        public PlayerState(int number = -1)
        {
            StateNumber = number;
        }
        public override void OnEnter()
        {

        }

        public override void OnExit()
        {

        }
        public override void OnUpdate()
        {
            if (Input.GetButtonDown("Jump"))
            {
                JumpRequested = true;
            }
        }

        public void ApplyAcceleration(float acceleration, int Sign, ref float Speed)
        {
            if (Mathf.Abs(Speed) < PhysicsInfo.MaxSpeed)
                Speed = Mathf.Clamp(Speed + acceleration * Sign * Time.fixedDeltaTime, -PhysicsInfo.MaxSpeed, PhysicsInfo.MaxSpeed);
        }

        public bool SameDirection(float XInput, float Speed)
        {
            return Speed == 0 || Mathf.Sign(XInput) == Mathf.Sign(Speed);
        }

    }
}