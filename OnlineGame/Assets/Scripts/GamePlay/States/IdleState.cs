namespace GamePlay.States
{
    public class IdleState : State
    {
        public override void OnEnter(StateMachine _stateMachine)
        {
            base.OnEnter(_stateMachine);
            
            m_Entity.SetTrigger("Idle");
            
            m_Entity.SetSpeed(0);
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (m_Entity.Dir.magnitude != 0)
            {
                stateMachine.SetNextState(new RunState());
            }
        }
    }
}