using Ribbon;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ribbon
{
    [Serializable]
    public class PlayerCollision
    {
        public Player Player;
        public LayerMask CollisionMask;
        public float GroundRaycastDistance = 1.1f, AirRaycastDistance = 0.5f;

        public bool IsGrounded;

        public bool DoGroundCollision()
        {
            var groundRay = Physics2D.Raycast(Player.transform.position , 
                Vector3.down, GroundRaycastDistance,
                CollisionMask);

            if (groundRay)
            {
                Player.transform.position = groundRay.point + Vector2.up * .5f;
                Player.SurfaceAngle = CalculateAngle(groundRay.normal);
                Debug.DrawRay(Player.transform.position, -groundRay.normal * GroundRaycastDistance, Color.green);
                return true;
            }
            
            return false;
        }
        
        public bool DoAirCollision()
        {
            var airRay = Physics2D.Raycast(Player.transform.position, 
                Vector3.down, AirRaycastDistance,
                CollisionMask);

            if (airRay && Player.Rb.velocity.y <= 0)
            {
                Player.transform.position = airRay.point + Vector2.up * .5f;
                Player.SurfaceAngle = CalculateAngle(airRay.normal);
                Debug.DrawRay(Player.transform.position, Vector3.down * GroundRaycastDistance, Color.green);
                return true;
            }
            
            return false;
        }
        
        public float CalculateAngle(Vector2 normal)
        {
            var angle = Vector2.SignedAngle(Vector2.up, normal) > 0 ? 
                Vector2.SignedAngle(Vector2.up, normal) : 
                360 + Vector2.SignedAngle(Vector2.up, normal);
            return angle;
        }
    }
}
