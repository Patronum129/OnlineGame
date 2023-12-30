using System;
using System.Net;
using UnityEngine;
using Utilitys;

namespace Client
{
    public class ClientManager : BaseSingleton<ClientManager>
    {
        private UDPClient m_UDPClient;
        
        public void Init(string ip)
        {
            m_UDPClient = new UDPClient();

            m_UDPClient.Ip = ip;
            
            m_UDPClient.Start();
        }

        public void Send(byte[] buff)
        {
            m_UDPClient.Send(buff);
        }
    }
}