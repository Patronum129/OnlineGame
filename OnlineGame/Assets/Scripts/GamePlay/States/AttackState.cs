using Model;

namespace GamePlay.States
{
    public class AttackState : State
    {
        private float duration = 0.33f;
        public override void OnEnter(StateMachine _stateMachine)
        {
            base.OnEnter(_stateMachine);
            
            m_Entity.SetTrigger("Attack");
            
            m_Entity.SetSpeed(0);

            stateID = 1;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (time > duration)
            {
                stateMachine.SetNextState(new IdleState());
            }

            if (m_Entity.DetectorWeapon(out var list))
            {
                foreach (var item in list)
                {
                    if (item.MyName != GameModel.MyName)
                    {
                        
                    }
                }
            }
        }
    }
}