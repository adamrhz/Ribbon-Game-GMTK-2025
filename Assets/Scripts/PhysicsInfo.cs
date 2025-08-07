using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ribbon
{
    [Serializable]
    public struct CapsuleSize
    {
        public Vector2 size;
        public Vector2 offset;
    }

    [CreateAssetMenu(fileName = "PhysicsInfo", menuName = "Ribbon/ScriptableObjects/PhysicsInfo", order = 1)]
    public class PhysicsInfo : ScriptableObject
    {
        [Header("Player Physics")]
        public float Acceleration = 20f;
        public float Deceleration = 100f;
        public float Friction = 40f;
        public float MaxSpeed = 9.7f;
        public float TopSpeed = 15f;


        public float SlopeRepelDownHill = .5f;
        public float SlopeRepelUpHill = .25f;

        public CapsuleSize StandingBounds =
            new CapsuleSize
            {
                size = new Vector2(0.4f, .6f),
                offset = new Vector2(0, 0)
            };

        public CapsuleSize SlidingBounds =
            new CapsuleSize
            {
                size = new Vector2(0.2f, .2f),
                offset = new Vector2(0, -.2f)
            };
        public float AirAcceleration = 15f;
        public float AirDeceleration = 80f;
        public float AirDrag = 10f;
        public int JumpAmount = 2;
        public float Gravity = 20f;
        public float JumpStrength = 12.5f;
        public float DoubleJumpMultiplier = 0.6f;
        public float JumpCutoff = 4f;
        public float MaxFallSpeed = 20f;
        public float CoyoteTime = 0.15f;


        public float MaxSwingSpeed = 20f;
        public float SwingAccelerationDown, SwingDecelerationUp;
    }
}