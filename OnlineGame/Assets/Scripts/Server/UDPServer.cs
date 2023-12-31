using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using GamePlay;
using Helper;
using UnityEngine;

namespace Server
{
    public class UDPServer
    {
        private UdpClient m_UdpListener;
        
        private int m_Port = 8899;

        private HashSet<IPEndPoint> m_Clients;

        /// <summary>
        /// 启动
        /// </summary>
        public void Start()
        {
            try
            {
                m_Clients = new HashSet<IPEndPoint>();
                
                //创建监听器
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
        /// 监听
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
                    Debug.Log("客户端已连接：" + client);
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
                
                m_UdpListener.Close();//停止监听客户端的连接
            }
        }

        /// <summary>
        /// 发送到指定客户端
        /// </summary>
        public async void SendTarget(byte[] data, IPEndPoint remote)
        {
            await m_UdpListener.SendAsync(data, data.Length, remote);
        }

        /// <summary>
        /// 广播给所有客户端
        /// </summary>
        public async void SendRpc(byte[] data)
        {
            foreach (var client in m_Clients)
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
            m_UdpListener.Close();//停止监听客户端的连接
            m_Clients.Clear();
        }
    }
}