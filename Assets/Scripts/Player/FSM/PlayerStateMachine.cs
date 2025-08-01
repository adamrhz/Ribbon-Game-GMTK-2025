using UnityEngine;
namespace Ribbon
{
    public class PlayerStateMachine : StateMachine<PlayerState>
    {
        public delegate void StateSwitchEvent(PlayerState oldState, PlayerState newState);
        public event StateSwitchEvent OnChangeState;
        
        public override void Add(PlayerState state)
        {
            base.Add(state);
            state.Machine = this;
            state.Player = GetComponent<Player>();
        }

        public override void Set<G>(bool TriggerEnter = true)
        {
            base.Set<G>(TriggerEnter);
            OnChangeState?.Invoke(PreviousState, CurrentState);
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