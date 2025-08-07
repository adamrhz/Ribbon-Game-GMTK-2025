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
        public float GroundRaycastDistance = 1.1f, AirRaycastDistance = 0.5f, WallRaycastDistance = .25f;

        public float WallRaycastSpacing = .25f;


        public bool IsGrounded;

        public bool DoGroundCollision()
        {
            var groundRay = Physics2D.Raycast(Player.transform.position , 
                Vector3.down, GroundRaycastDistance,
                CollisionMask);

            if (groundRay)
            {
                Player.Rb.position = Vector3.ProjectOnPlane(Player.Rb.position, Vector3.up) 
                                            + (groundRay.point.y + 0.5f) * Vector3.up;
                Player.SurfaceAngle = CalculateAngle(groundRay.normal);
                Debug.DrawRay(Player.transform.position, -groundRay.normal * GroundRaycastDistance, Color.green);
                if (groundRay.collider.TryGetComponent(out RWorldObject2D worldObject))
                {
                    worldObject.OnPlayerStand.Invoke(Player);
                }

                return true;
            }
            
            return false;
        }
        public bool DoSlideCeilingCollision()
        {
            var groundRay = Physics2D.Raycast(Player.transform.position,
                Vector3.up, GroundRaycastDistance/4,
                CollisionMask);
            Debug.DrawRay(Player.transform.position, Vector3.up * GroundRaycastDistance/4, Color.blue);

            if (groundRay)
            {
                return true;
            }

            return false;
        }


        public bool DoWallCollision()
        {

            Vector2 center = Player.transform.position;

            var upper = Physics2D.Raycast(center + Vector2.up * WallRaycastSpacing,
                Vector2.right * Player.Direction, WallRaycastDistance,
                CollisionMask);
            var lower = Physics2D.Raycast(center + Vector2.down * WallRaycastSpacing,
                Vector2.right * Player.Direction, WallRaycastDistance,
                CollisionMask);

            if (upper && lower)
            {

                var winnerPoint = Vector3.Lerp(upper.point, lower.point, 0.5f);
                var winnerNormal = Vector3.Lerp(upper.normal, lower.normal, 0.5f);

                if(Mathf.Abs(Vector2.Dot(winnerNormal, Vector2.right)) > .9f && upper.collider.gameObject.tag == "WallJumpable"){
                    return true;
                }

            }

            return false;
        }
        
        public bool DoAirGroundCollision()
        {
            var airRay = Physics2D.Raycast(Player.transform.position, 
                Vector3.down, AirRaycastDistance,
                CollisionMask);

            if (airRay)
            {
                Player.transform.position = airRay.point + Vector2.up * .5f;
                Player.SurfaceAngle = CalculateAngle(airRay.normal);
                Debug.DrawRay(Player.transform.position, Vector3.down * GroundRaycastDistance, Color.green);
                if (airRay.collider.TryGetComponent(out RWorldObject2D worldObject))
                {
                    worldObject.OnPlayerLand.Invoke(Player);
                }
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
