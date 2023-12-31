using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Client;
using Model;
using Net.Actions;
using Net.Datas;
using Server;
using UnityEngine;
using Utilitys;

namespace Helper
{
    public class MessageManager : BaseSingleton<MessageManager>
    {
        private byte[] data = new byte[4096];

        private int msgLength = 0;

        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
        }

        public void CopyToData(byte[] buffer, int length)
        {
            Array.Copy(buffer,0,data,msgLength,length);

            msgLength += length;
            
            Handle();
        }
        
        private void Handle()
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
                    Debug.Log($"收到请求：{id}");

                    switch(id)
                    {
                        case 1001:
                            ChatMsgHandle(body);
                            return;
                        case 1002:
                            ReadyMsgHandle(body);
                            return;
                        case 1003:
                            JoinMsgHandle(body);
                            return;
                        case 1004:
                            JoinRpcMsgHandle(body);
                            return;
                        case 1005:
                            StartGameMsgHandle();
                            return;
                        case 1006:
                            SyncTransformMsgHandle(body);
                            return;
                        case 1007:
                            SyncAnimatorMsgHandle(body);
                            return;
                        case 1008:
                            return;
                        case 1009:
                            return;
                        
                    }
                }
            }
        }

        #region ChatMsg
        
        public void SendChatMsg(string name, string chat, bool isServer = false, bool isRpc = true, IPEndPoint ipEndPoint = null)
        {
            ChatMsg msg = new ChatMsg();

            msg.playerName = name;
            msg.msg = chat;

            var str = JsonHelper.ToJson(msg);
            
            Send(1001,str,isServer,isRpc,ipEndPoint);
        }
        
        private void ChatMsgHandle(byte[] obj)
        {
            var str = Encoding.UTF8.GetString(obj);
            
            ChatMsg msg = JsonHelper.ToObject<ChatMsg>(str);
            
            NetActions.ChatHandle?.Invoke(msg);

            if (GameModel.IsServer)
            {
                SendChatMsg(msg.playerName, msg.msg, true);
            }
        }

        #endregion

        #region ReadyMsg
        
        public void SendReadyMsg(string _name, bool isServer = false, bool isRpc = true, IPEndPoint ipEndPoint = null)
        {
            Send(1002,_name,isServer,isRpc,ipEndPoint);
        }
        
        private void ReadyMsgHandle(byte[] obj)
        {
            var str = Encoding.UTF8.GetString(obj);

            NetActions.ReadyHandle?.Invoke(str);
            
            if (GameModel.IsServer)
            {
                SendReadyMsg(str, true);
            }
        }

        #endregion
        
        #region JoinGameMsg
        
        public void SendJoinMsg(string name, bool isServer = false, bool isRpc = true, IPEndPoint ipEndPoint = null)
        {
            Send(1003,name,isServer,isRpc,ipEndPoint);
        }
        
        private void JoinMsgHandle(byte[] obj)
        {
            var str = Encoding.UTF8.GetString(obj);

            NetActions.JoinHandle?.Invoke(str);
        }

        public void SendJoinRpcMsg(List<string> names)
        {
            var str = JsonHelper.ToJson(names);
            Send(1004,str,true,true,null);
        }
        
        private void JoinRpcMsgHandle(byte[] obj)
        {
            var str = Encoding.UTF8.GetString(obj);

            var names = JsonHelper.ToObject<List<string>>(str);

            NetActions.RoomPlayerUIHandle?.Invoke(names);
        }
        #endregion

        #region StartGameMsg

        public void SendStartGameRpcMsg()
        {
            Send(1005,"StartGame",true,true,null);
        }
        
        private void StartGameMsgHandle()
        {
            NetActions.StartGameHandle?.Invoke();
        }

        #endregion
        
        #region SyncTransform
        
        public void SendSyncTransformMsg(SyncTransformMsg syncTransformMsg, bool isServer = false, bool isRpc = true, IPEndPoint ipEndPoint = null)
        {
            var str = JsonHelper.ToJson(syncTransformMsg);
            
            Send(1006,str,isServer,isRpc,ipEndPoint);
        }
        
        private void SyncTransformMsgHandle(byte[] obj)
        {
            var str = Encoding.UTF8.GetString(obj);
            
            SyncTransformMsg msg = JsonHelper.ToObject<SyncTransformMsg>(str);
            
            NetActions.SyncTransformHandle?.Invoke(msg);

            if (GameModel.IsServer)
            {
                SendSyncTransformMsg(msg, true);
            }
        }

        #endregion
        
        #region SyncAnimator
        
        public void SendSyncAnimatorMsg(SyncAnimatorMsg syncAnimatorMsg, bool isServer = false, bool isRpc = true, IPEndPoint ipEndPoint = null)
        {
            var str = JsonHelper.ToJson(syncAnimatorMsg);
            
            Send(1007,str,isServer,isRpc,ipEndPoint);
        }
        
        private void SyncAnimatorMsgHandle(byte[] obj)
        {
            var str = Encoding.UTF8.GetString(obj);
            
            SyncAnimatorMsg msg = JsonHelper.ToObject<SyncAnimatorMsg>(str);
            
            NetActions.SyncAnimatorHandle?.Invoke(msg);

            if (GameModel.IsServer)
            {
                SendSyncAnimatorMsg(msg, true);
            }
        }

        #endregion
        
        private void Send(int id, string str, bool isServer, bool isRpc, IPEndPoint ipEndPoint)
        {
            //转换成byte[]
            var body = Encoding.UTF8.GetBytes(str);

            //包体大小 4 消息ID 4 包体内容
            byte[] send_buff = new byte[body.Length + 8];

            int size = body.Length;
            var _size = BitConverter.GetBytes(size);
            var _id = BitConverter.GetBytes(id);

            Array.Copy(_size, 0, send_buff, 0, 4);
            Array.Copy(_id, 0, send_buff, 4, 4);
            Array.Copy(body, 0, send_buff, 8, body.Length);

            if (isServer)
            {
                if (isRpc)
                {
                    ServerManager.Singleton.SendRpc(send_buff);
                }
                else
                {
                    ServerManager.Singleton.SendTarget(send_buff,ipEndPoint);
                }
            }
            else
            {
                ClientManager.Singleton.Send(send_buff);
            }
        }
    }
}