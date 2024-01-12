using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using GamePlay;
using Helper;
using Model;
using UnityEngine;

namespace Server
{
    public class UDPServer
    {
        private UdpClient m_UdpListener;
        
        private int m_Port = 8899;

        private HashSet<IPEndPoint> m_Clients;

        /// <summary>
        /// Start
        /// </summary>
        public void Start()
        {
            try
            {
                m_Clients = new HashSet<IPEndPoint>();

                //Create Listener
                m_UdpListener = new UdpClient(m_Port);

                Debug.Log("UDP Server Start!" + ":" + m_Port);

                Accept();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// Listen
        /// </summary>
        public async void Accept()
        {
            try
            {
                var result = await m_UdpListener.ReceiveAsync();

                IPEndPoint client = result.RemoteEndPoint;

                if (!m_Clients.Contains(client))
                {
                    m_Clients.Add(client);
                    Debug.Log("Client Connected：" + client);
                }

                MessageManager.Singleton.CopyToData(result.Buffer,result.Buffer.Length);
                
                Accept();
            }
            catch(Exception e)
            {
                try
                {
                    GameManager.Singleton.ShowWinPanel("Game Over");
                    MessageManager.Singleton.ExitGameMsgRpcMsg(true);
                }
                catch (Exception exception)
                {
                   
                }
                
                Debug.Log($"Accept: {e.Message}");
                
                m_UdpListener.Close();//Stop Listening for Client Connections
            }
        }

        public void ClearIP()
        {
            m_Clients.Clear();
        }
        
        /// <summary>
        /// Send to Specific Client
        /// </summary>
        public async void SendTarget(byte[] data, IPEndPoint remote)
        {
            await m_UdpListener.SendAsync(data, data.Length, remote);
        }

        /// <summary>
        /// Broadcast to All Clients
        /// </summary>
        public async void SendRpc(byte[] data)
        {
            HashSet<IPEndPoint> tempHash = new HashSet<IPEndPoint>(m_Clients);
            
            foreach (var client in tempHash)
            {
                if (client == null)
                {
                    GameManager.Singleton.ShowWinPanel("Game Over");
                    MessageManager.Singleton.ExitGameMsgRpcMsg(true);
                }
                
                await m_UdpListener.SendAsync(data, data.Length, client);
            }
        }

        public void Close()
        {
            m_UdpListener.Close();//Stop Listening to Client Connections
            m_Clients.Clear();
            
            m_UdpListener.Dispose();
        }
    }
}