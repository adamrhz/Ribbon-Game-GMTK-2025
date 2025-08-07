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

        public float LockOnCoolDownTimer = 0;



        public override void Init(Player player)
        {
            base.Init(player);
            // Additional initialization for SwingController can be added here
        }



        public override void OnUpdate()
        {
            Player.Visual.ToggleIndicatorLine(null);
            if (SwingingTarget && !Player.DefinitiveInputLock)
            {

                if (Player.Machine.IsCurrentState<RB_PS_Air>())
                {
                    if (Player.Input.GetButtonDown("Jump"))
                    {
                        Player.Machine.Get<RB_PS_Swing>().SwingingTarget = SwingingTarget;
                        Player.Machine.Set<RB_PS_Swing>();
                    }

                    Player.Visual.ToggleIndicatorLine(SwingingTarget.transform.position);
                }

                if(Vector2.Distance(Player.transform.position, SwingingTarget.transform.position) > SwingingRadius)
                {
                    SwingingTarget = null;
                }

            }


                LockOnCoolDownTimer -= Time.deltaTime;
            RaycastHit2D[] hit = Physics2D.CircleCastAll(Player.transform.position, SwingingRadius, Vector2.down, 0.1f, SwingingMask);
            GameObject NewTarget = GetClosest(hit);

            LockOnCoolDownTimer -= Time.deltaTime;
            if (NewTarget != SwingingTarget)
            {
                if (LockOnCoolDownTimer <= 0)
                {
                    SwingingTarget = NewTarget;
                    if (SwingingTarget != null)
                    {
                        LockOnCoolDownTimer = 0.25f;
                    }
                }
            }
            if (SwingingTarget)
            {
                Debug.DrawLine(Player.transform.position, SwingingTarget.transform.position, Color.cyan);
            }


        }

        private GameObject GetClosest(RaycastHit2D[] hits)
        {
            GameObject closest = null;
            float closestDistance = Mathf.Infinity;
            float closestDotDistance = Mathf.Infinity;

            Vector2 input = Player.Input.GetAxis2D(GamePreference.MoveInput).normalized;
            if(input.magnitude == 0)
            {
                input = new(Player.Direction, 0);
            }
            Debug.DrawLine(Player.transform.position, (Vector2)Player.transform.position+ input, Color.green);
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider != null && hit.collider.gameObject != null)
                {
                    Vector2 direction = hit.point - (Vector2)Player.transform.position;

                    float dotValue = Vector2.Dot(direction.normalized, input.normalized);
                    if (input.magnitude > 0 && dotValue < 0.1f)
                        continue;
                    float dotDistance = hit.distance/dotValue;

                    Debug.DrawLine(Player.transform.position, hit.point, Color.red);
                    if (dotDistance < closestDotDistance)
                    {
                        closest = hit.collider.gameObject;
                        closestDistance = hit.distance;
                        closestDotDistance = dotDistance;
                    }
                }
            }

            return closest;
        }
    }
}