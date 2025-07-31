using UnityEngine;
namespace Ribbon
{
    public class PlayerStateMachine : StateMachine<PlayerState>
    {
        public override void Add(PlayerState state)
        {
            base.Add(state);
            state.Machine = this;
            state.Player = GetComponent<Player>();
            state.Transform = transform;
        }
        public void Init()
        {
            Add(new RB_PS_Ground());
            Add(new RB_PS_Air());
            Add(new RB_PS_Swing());
            Initialize<RB_PS_Ground>();
        }

        public void Update()
        {
            CurrentState.OnUpdate();
        }

        public void FixedUpdate()
        {
            CurrentState.OnFixedUpdate();
        }
    }
}