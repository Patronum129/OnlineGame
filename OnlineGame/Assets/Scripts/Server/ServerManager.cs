using System;
using Utilitys;

namespace Server
{
    public class ServerManager : BaseSingleton<ServerManager>
    {
        private UDPServer m_UDPServer;

        private void Start()
        {
            m_UDPServer = new UDPServer();
            
            m_UDPServer.Start();
        }
    }
}