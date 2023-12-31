namespace GamePlay.States
{
    public class RunState : State
    {
        public override void OnEnter(StateMachine _stateMachine)
        {
            base.OnEnter(_stateMachine);
            
            m_Entity.SetTrigger("Run");
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (m_Entity.Dir.magnitude == 0)
            {
                stateMachine.SetNextState(new IdleState());
            }
            else
            {
                m_Entity.SetSpeed(3);
            }
        }
    }
}