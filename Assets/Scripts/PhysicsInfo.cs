using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ribbon
{
    [CreateAssetMenu(fileName = "PhysicsInfo", menuName = "Ribbon/ScriptableObjects/PhysicsInfo", order = 1)]
    public class PhysicsInfo : ScriptableObject
    {

        [Header("Player Physics")]
        public float Acceleration = 10f;          
        public float Deceleration = 40f;   
        public float Friction = 12.5f;      
        public float MaxSpeed = 40f;




        public float AirAcceleration = 10f;
        public float AirDeceleration = 40f;
        public int JumpAmount = 2;
        public float Gravity = 30f;       
        public float JumpStrength = 15f;  
        public float JumpCutoff = 8f;  
        public float MaxFallSpeed = 30f;  
        public float CoyoteTime = 0.15f;

    }
}