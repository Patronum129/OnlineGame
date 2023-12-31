using Helper;
using Model;
using Net.Actions;
using UnityEngine;

namespace GamePlay
{
    public class Bomb : MonoBehaviour
    {
        public int ID;
        
        private bool m_IsDestory;

        private void Start()
        {
            NetActions.BombHandle += Handel;
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if(m_IsDestory) return;
            
            var entity = col.GetComponent<Entity>();
            
            if (entity != null)
            {
                if (!entity.IsLocalPlayer) return;
                
                m_IsDestory = true;

                SendDie();
                
                entity.Die();
            }
        }

        private void SendDie()
        {
            if (GameModel.IsServer)
            {
                GetComponent<Animator>().SetTrigger("Bomb");
                
                GameObject.Destroy(transform.gameObject,1f);

                MessageManager.Singleton.SendBombMsg(ID, true);
            }
            else
            {
                MessageManager.Singleton.SendBombMsg(ID);
            }
        }

        private void Handel(int _id)
        {
            if (ID == _id)
            {
                GetComponent<Animator>().SetTrigger("Bomb");
                
                GameObject.Destroy(transform.gameObject,1f);

                if (GameModel.IsServer)
                {
                    MessageManager.Singleton.SendBombMsg(ID, true);
                }
            }
        }
    }
}