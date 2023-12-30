using System;
using System.Text;
using Client;
using UnityEngine;
using Utilitys;

namespace Helper
{
    public class MessageManager : BaseSingleton<MessageManager>
    {
        private byte[] data = new byte[4096];

        private int msgLength = 0;
        
        public void CopyToData(byte[] buffer, int length)
        {
            Array.Copy(buffer,0,data,msgLength,length);

            msgLength += length;
            
            Handle();
        }
        
        public void Handle()
        {
            //包体大小（4） 协议ID（4） 包体（byte[])
            if(msgLength >= 8)
            {
                byte[] _size=new byte[msgLength];

                Array.Copy(data, 0, _size, 0, 4);

                int size = BitConverter.ToInt32(_size, 0);

                //本次要拿的长度
                var _length = 8 + size;

                if(msgLength >= _length)
                {
                    //拿出ID
                    byte[] _id = new byte[4];
                    Array.Copy(data,4, _id, 0, 4);
                    int id = BitConverter.ToInt32 (_id, 0);

                    //包体
                    byte[] body = new byte[size];

                    Array.Copy(data,8,body, 0, size);

                    if(msgLength>_length)
                    {
                        for(int i=0;i<msgLength-_length; i++)
                        {
                            data[i] = _id[_length+i];
                        }
                    }

                    msgLength -= _length;
                    Debug.Log($"收到服务端请求：{id}");

                    switch(id)
                    {
                        
                    }
                }
            }
        }
        
        public void SendToServer(int id , string str)
        {
            //转换成byte[]
            var body = Encoding.UTF8.GetBytes(str);

            //包体大小 4 消息ID 4 包体内容
            byte[] send_buff = new byte[body.Length + 8];

            int size = body.Length;
            var _size = BitConverter.GetBytes(size);
            var _id=BitConverter.GetBytes(id);

            Array.Copy(_size, 0, send_buff, 0, 4);
            Array.Copy(_id, 0, send_buff, 4, 4);
            Array.Copy(body,0, send_buff, 8, body.Length);

            ClientManager.Singleton.Send(send_buff);
        }
    }
}