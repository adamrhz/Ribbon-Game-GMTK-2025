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
        public float GroundRaycastDistance = 1.1f;

        public bool IsGrounded()
        {

            ContactFilter2D ContactFilter = new ContactFilter2D
            {
                layerMask = CollisionMask,
                useTriggers = false
            };
            bool IsGrounded = Physics2D.Raycast(Player.transform.position, Vector3.down, GroundRaycastDistance, CollisionMask);

            Debug.DrawRay(Player.transform.position, Vector3.down * GroundRaycastDistance, IsGrounded ? Color.green : Color.red);
            return IsGrounded;
        }

    }
}
