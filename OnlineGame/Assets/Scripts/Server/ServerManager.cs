using System;
using System.Net;
using Utilitys;

namespace Server
{
    public class ServerManager : BaseSingleton<ServerManager>
    {
        private UDPServer m_UDPServer;

        protected override void Awake()
        {
            base.Awake();
            
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            m_UDPServer = new UDPServer();
            
            m_UDPServer.Start();
        }

        public void SendRpc(byte[] buff)
        {
            m_UDPServer.SendRpc(buff);
        }

        public void SendTarget(byte[] buff,IPEndPoint ipEndPoint)
        {
            m_UDPServer.SendTarget(buff,ipEndPoint);
        }
    }
}