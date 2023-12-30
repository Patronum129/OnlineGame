using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Helper;
using UnityEngine;

namespace Client
{
    public class UDPClient
    {
        private UdpClient m_UDPClient;
        
        public string Ip;
        private int m_Port = 8899;

        public void Start()
        {
            m_UDPClient = new UdpClient();
            Receive();
        }

        public async void Receive()
        {
            try
            {
                var result = await m_UDPClient.ReceiveAsync();
                
                byte[] buff = result.Buffer;
                int length = buff.Length;

                if (length > 0)
                {
                    string str = Encoding.UTF8.GetString(buff, 0, buff.Length);

                    MessageManager.Singleton.CopyToData(buff, length);
                }
                else
                {
                    Debug.LogError($"Receive error: 数据为空");
                }

                Receive();
            }
            catch (Exception e)
            {
                Debug.LogError($"Receive error:{e.Message}");
                m_UDPClient.Close();
            }
        }
        
        public async void Send(byte[] data)
        {
            try
            {
                await m_UDPClient.SendAsync(data, data.Length, Ip,m_Port);
            }
            catch (Exception e)
            {
                Debug.LogError($"Send error:{e.Message}");
                
                m_UDPClient.Close();
            }
        }
    }
}