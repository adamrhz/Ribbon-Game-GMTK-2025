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
            rbVelocityTarget = Vector2.Lerp(rbVelocityTarget, Vector2.Scale(Target.Rb.velocity, new Vector2(4, 1)),
                3 * Time.deltaTime);
            Vector3 playerPivot = (Vector2)Target.transform.position + rbVelocityTarget * (3 * Time.fixedDeltaTime);
            
            swingSwitchTarget = Mathf.Lerp(swingSwitchTarget, Target.Machine.CurrentState is RB_PS_Swing ? .9f : Mathf.Lerp(swingSwitchTarget, 0, 204 * Time.deltaTime),
                7 * Time.deltaTime);

            var finalPoint = Vector3.Lerp(playerPivot, swingPoint, swingSwitchTarget);
            
            transform.position = Vector3.Lerp(transform.position, new Vector3(finalPoint.x, Mathf.Max(finalPoint.y, MinimumYPoint), -ZDepth),
                10 * Time.deltaTime);
        }
        
    }
}