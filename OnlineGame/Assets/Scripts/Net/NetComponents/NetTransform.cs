using GamePlay;
using Helper;
using Model;
using Net.Actions;
using Net.Datas;
using UnityEngine;

namespace Net.NetComponents
{
    public class NetTransform : MonoBehaviour
    {
        private bool m_IsInit;

        private Entity m_Entity;
        public void Init()
        {
            m_Entity = GetComponent<Entity>();

            NetActions.SyncTransformHandle += Handle;

            m_IsInit = true;
        }
        
        private void FixedUpdate()
        {
            if (!m_IsInit || !m_Entity.IsLocalPlayer) return;

            if (m_Entity.IsDie) return;
            
            SyncTransform();
        }

        private void SyncTransform()
        {
            var msg = new SyncTransformMsg();

            msg.playerName = GameModel.MyName;
            msg.syncPosition = new SyncVector2((Vector2)transform.position);
            msg.syncScale = new SyncVector2((Vector2)transform.localScale);

            if (GameModel.IsServer)
            {
                MessageManager.Singleton.SendSyncTransformMsg(msg,true);
            }
            else
            {
                MessageManager.Singleton.SendSyncTransformMsg(msg);
            }
        }

        private void Handle(SyncTransformMsg syncTransformMsg)
        {
            if (syncTransformMsg.playerName == m_Entity.MyName)
            {
                if (syncTransformMsg.playerName != GameModel.MyName)
                {
                    transform.position = new Vector3(syncTransformMsg.syncPosition.x,syncTransformMsg.syncPosition.y,0);
                    transform.localScale = new Vector3(syncTransformMsg.syncScale.x,syncTransformMsg.syncScale.y,1);
                }
            }
        }
    }
}