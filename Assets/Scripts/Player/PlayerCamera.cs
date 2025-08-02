using System;
using UnityEngine;

namespace Ribbon
{
    public sealed class PlayerCamera : MonoBehaviour
    {
        public Player Target;
        public float ZDepth = 10;

        private Vector3 swingPoint;
        private float swingSwitchTarget;

        private Vector2 rbVelocityTarget;
        public float MinimumYPoint;

        public Vector2 XYLimits;

        public bool Intro = false;
        public Vector3 discreteTarget;
        public float IntroLerpSpeed = 15f;

        public void Start()
        {
            transform.SetParent(null);
            Target.Machine.OnChangeState += MachineOnChangeState;
        }

        private void MachineOnChangeState(PlayerState oldState, PlayerState newState)
        {
            if (newState is RB_PS_Swing swing)
            {
                swingPoint = swing.SwingingTarget.transform.position;
            }
        }

        public void Update()
        {
            Vector2 levelBorders = new Vector2(XYLimits.x, XYLimits.y);
            if (!Intro)
            {
                rbVelocityTarget = Vector2.Lerp(rbVelocityTarget, Vector2.Scale(Target.Rb.velocity, new Vector2(4, 1)),
                    3 * Time.deltaTime);
                Vector3 playerPivot = (Vector2)Target.Rb.position + rbVelocityTarget * (3 * Time.fixedDeltaTime);

                swingSwitchTarget = Mathf.Lerp(swingSwitchTarget, Target.Machine.CurrentState is RB_PS_Swing ? .9f : Mathf.Lerp(swingSwitchTarget, 0, 204 * Time.deltaTime),
                    7 * Time.deltaTime);

                var finalPoint = Vector3.Lerp(playerPivot, swingPoint, swingSwitchTarget);

                transform.position = Vector3.Lerp(transform.position, new Vector3(Mathf.Max(finalPoint.x, levelBorders.x), Mathf.Max(finalPoint.y, levelBorders.y), -ZDepth),
                    10 * Time.deltaTime);
            }
            else
            {
                if (discreteTarget != Vector3.zero)
                {
                    rbVelocityTarget = Vector2.zero;
                    swingPoint = Target.transform.position;
                    transform.position = Vector3.MoveTowards(transform.position, new Vector3(discreteTarget.x, Mathf.Max(discreteTarget.y, levelBorders.y), -ZDepth), IntroLerpSpeed * Time.deltaTime);
                    swingSwitchTarget = 0;
                }
            }
        }

        public void ForcePosition(Vector3 discreteTarget)
        {
            transform.position = new(discreteTarget.x, discreteTarget.y -ZDepth);
        }
    }
}