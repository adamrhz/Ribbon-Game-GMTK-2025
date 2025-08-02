using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ribbon
{
    [CreateAssetMenu(fileName = "PhysicsInfo", menuName = "Ribbon/ScriptableObjects/PhysicsInfo", order = 1)]
    public class PhysicsInfo : ScriptableObject
    {
        [Header("Player Physics")]
        public float Acceleration = 20f;          
        public float Deceleration = 100f;   
        public float Friction = 40f;      
        public float MaxSpeed = 9.7f;

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