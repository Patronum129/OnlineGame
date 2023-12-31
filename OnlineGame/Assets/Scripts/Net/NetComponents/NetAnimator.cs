using GamePlay;
using Helper;
using Model;
using Net.Actions;
using Net.Datas;
using UnityEngine;

namespace Net.NetComponents
{
    public class NetAnimator : MonoBehaviour
    {
        private bool m_IsInit;

        private Entity m_Entity;
        
        public void Init()
        {
            m_Entity = GetComponent<Entity>();
            
            m_IsInit = true;

            NetActions.SyncAnimatorHandle += Handle;
        }
        
        public void SyncAnimator(string _name)
        {
            if (!m_IsInit || !m_Entity.IsLocalPlayer) return;

            if (m_Entity.IsDie) return;
            
            switch (_name)
            {
                case "Idle":
                    SendMsg(0);
                    break;
                case "Run":
                    SendMsg(1);
                    break;
                case "Attack":
                    SendMsg(2);
                    break;
            }
        }

        private void SendMsg(int i)
        {
            var msg = new SyncAnimatorMsg
            {
                playerName = m_Entity.MyName,
                triggerID = i
            };

            if (GameModel.IsServer)
            {
                MessageManager.Singleton.SendSyncAnimatorMsg(msg,true);
            }
            else
            {
                MessageManager.Singleton.SendSyncAnimatorMsg(msg);
            }
        }
        
        private void Handle(SyncAnimatorMsg syncAnimatorMsg)
        {
            if (m_Entity.IsDie) return;
            
            if (syncAnimatorMsg.playerName == m_Entity.MyName)
            {
                if (syncAnimatorMsg.playerName != GameModel.MyName)
                {
                    switch (syncAnimatorMsg.triggerID)
                    {
                        case 0:
                            m_Entity.SetTrigger("Idle");
                            break;
                        case 1:
                            m_Entity.SetTrigger("Run");
                            break;
                        case 2:
                            m_Entity.SetTrigger("Attack");
                            break;
                    }
                }
            }
        }
    }
}