using System;
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
        public Transform Transform => Player.transform;

        public float ZAngle
        {
            get => Transform.eulerAngles.z;
            set => Transform.eulerAngles = new Vector3(Transform.eulerAngles.x, Transform.eulerAngles.y, value);
        }
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
            if (Input.GetButtonDown(GamePreference.JumpButton))
            {
                JumpRequested = true;
            }
        }
        public void SetDirection(float xspeed)
        {
            if (Mathf.Abs(xspeed) > .001f)
            {
                Player.Direction = (int)Mathf.Sign(xspeed);
            }
        }

        public void ApplyAcceleration(float acceleration, float Input, ref float Speed)
        {
            if (Mathf.Abs(Speed) < PhysicsInfo.MaxSpeed)
                Speed = Mathf.Clamp(Speed + acceleration * Mathf.Sign(Input) * Time.fixedDeltaTime, -PhysicsInfo.MaxSpeed, PhysicsInfo.MaxSpeed);
        }

        public bool SameDirection(float XInput, float Speed)
        {
            return Math.Sign(XInput) == Math.Sign(Speed);
        }

    }
}