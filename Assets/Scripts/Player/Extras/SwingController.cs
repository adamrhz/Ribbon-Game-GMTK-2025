using System;
using System.Collections;
using UnityEngine;

namespace Ribbon
{
    [Serializable]
    public class SwingController : PlayerController
    {
        public LayerMask SwingingMask;
        public float SwingingRadius;

        public GameObject SwingingTarget;



        public override void Init(Player player)
        {
            base.Init(player);
            // Additional initialization for SwingController can be added here
        }



        public override void OnUpdate()
        {
            if (Player.Machine.IsCurrentState<RB_PS_Air>())
            {
                RaycastHit2D[] hit =  Physics2D.CircleCastAll(Player.transform.position, SwingingRadius, Vector2.down, 0.1f, SwingingMask);
                SwingingTarget = GetClosest(hit);
                if (SwingingTarget)
                {
                    Debug.DrawLine(Player.transform.position, SwingingTarget.transform.position, Color.cyan);
                    if (Player.Input.GetButtonDown("Jump"))
                    {
                        Player.Machine.Get<RB_PS_Swing>().SwingingTarget = SwingingTarget;
                        Player.Machine.Set<RB_PS_Swing>();
                    }
                }

            }
            else if(SwingingTarget != null)
            {
                SwingingTarget = null;
            }

        }

        private GameObject GetClosest(RaycastHit2D[] hits)
        {
            GameObject closest = null;
            float closestDistance = Mathf.Infinity;

            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider != null && hit.collider.gameObject != null)
                {
                    if (hit.distance < closestDistance)
                    {
                        closest = hit.collider.gameObject;
                        closestDistance = hit.distance;
                    }
                }
            }

            return closest;
        }
    }
}