using UnityEngine;

namespace GamePlay.States
{
    public abstract class State
    {
        protected Entity m_Entity;

        protected int stateID;
        
        protected float time { get; set; }
        protected float fixedtime { get; set; }
        protected float latetime { get; set; }
    
        public StateMachine stateMachine;
    
        public virtual void OnEnter(StateMachine _stateMachine)
        {
            stateMachine = _stateMachine;

            m_Entity = GetComponent<Entity>();
        }
    
    
        public virtual void OnUpdate()
        {
            time += Time.deltaTime;

            if (!m_Entity.IsLocalPlayer) return;
                
            if (Input.GetMouseButtonDown(0) && stateID != 1)
            {
                stateMachine.SetNextState(new AttackState());
            }
        }
    
        public virtual void OnFixedUpdate()
        {
            fixedtime += Time.deltaTime;
        }
        public virtual void OnLateUpdate()
        {
            latetime += Time.deltaTime;
        }
    
        public virtual void OnExit()
        {
    
        }
    
        #region Passthrough Methods
    
        /// <summary>
        /// Removes a gameobject, component, or asset.
        /// </summary>
        /// <param name="obj">The type of Component to retrieve.</param>
        protected static void Destroy(UnityEngine.Object obj)
        {
            UnityEngine.Object.Destroy(obj);
        }
    
        /// <summary>
        /// Returns the component of type T if the game object has one attached, null if it doesn't.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected T GetComponent<T>() where T : Component { return stateMachine.GetComponent<T>(); }
    
        /// <summary>
        /// Returns the component of Type <paramref name="type"/> if the game object has one attached, null if it doesn't.
        /// </summary>
        /// <param name="type">The type of Component to retrieve.</param>
        /// <returns></returns>
        protected Component GetComponent(System.Type type) { return stateMachine.GetComponent(type); }
    
        /// <summary>
        /// Returns the component with name <paramref name="type"/> if the game object has one attached, null if it doesn't.
        /// </summary>
        /// <param name="type">The type of Component to retrieve.</param>
        /// <returns></returns>
        protected Component GetComponent(string type) { return stateMachine.GetComponent(type); }
        #endregion
    }
}