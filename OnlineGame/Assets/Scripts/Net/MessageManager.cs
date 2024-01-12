using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Client;
using Model;
using Net.Actions;
using Net.Datas;
using Newtonsoft.Json;
using Server;
using UnityEngine;
using Utilitys;

namespace Helper
{
    public class MessageManager : BaseSingleton<MessageManager>
    {
        private byte[] data = new byte[512];

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
            //Package Size (4), Protocol ID (4), Package Body (byte[])
            if(msgLength >= 8)
            {
                byte[] _size=new byte[msgLength];

                Array.Copy(data, 0, _size, 0, 4);

                int size = BitConverter.ToInt32(_size, 0);

                //Length to Retrieve This Time
                var _length = 8 + size;

                if(msgLength >= _length)
                {
                    //Extract the ID
                    byte[] _id = new byte[4];
                    Array.Copy(data,4, _id, 0, 4);
                    int id = BitConverter.ToInt32 (_id, 0);

                    //Package Body
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
                            DieMsgHandle(body);
                            return;
                        case 1009:
                            WinMsgHandle(body);
                            return;
                        case 1010:
                            BombMsgHandle(body);
                            return;
                        case 1011:
                            CreateBombMsgHandle(body);
                            return;
                        case 1012:
                            ExitGameMsgHandle();
                            return;
                        case 1013:
                            ExitLobbyMsgHandle(body);
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

            var msg = JsonHelper.ToObject<SyncAnimatorMsg>(str);
            
            NetActions.SyncAnimatorHandle?.Invoke(msg);

            if (GameModel.IsServer)
            {
                SendSyncAnimatorMsg(msg, true);
            }
        }

        #endregion
        
        #region DieMsg
        
        public void SendDieMsg(string str, bool isServer = false, bool isRpc = true, IPEndPoint ipEndPoint = null)
        {
            Send(1008,str,isServer,isRpc,ipEndPoint);
        }
        
        private void DieMsgHandle(byte[] obj)
        {
            var str = Encoding.UTF8.GetString(obj);

            NetActions.DieHandle?.Invoke(str);

            if (GameModel.IsServer)
            {
                SendDieMsg(str, true);
            }
        }

        #endregion
        
        #region WinMsg
        
        public void SendWinMsg(string str, bool isServer = false, bool isRpc = true, IPEndPoint ipEndPoint = null)
        {
            Send(1009,str,isServer,isRpc,ipEndPoint);
        }
        
        private void WinMsgHandle(byte[] obj)
        {
            var str = Encoding.UTF8.GetString(obj);
            
            NetActions.WinHandle?.Invoke(str);
        }

        #endregion
        
        #region BombMsg
        
        public void SendBombMsg(int id, bool isServer = false, bool isRpc = true, IPEndPoint ipEndPoint = null)
        {
            var str = id.ToString();
            Send(1010,str,isServer,isRpc,ipEndPoint);
        }
        
        private void BombMsgHandle(byte[] obj)
        {
            var str = Encoding.UTF8.GetString(obj);

            int id = Int32.Parse(str);
            
            Debug.Log(id);
            
            NetActions.BombHandle?.Invoke(id);
        }

        #endregion
        
        #region CreateBombMsg
        
        public void SendBombMsg(List<int> list, bool isServer = false, bool isRpc = true, IPEndPoint ipEndPoint = null)
        {
            var str = JsonHelper.ToJson(list);
            Send(1011,str,isServer,isRpc,ipEndPoint);
        }
        
        private void CreateBombMsgHandle(byte[] obj)
        {
            var str = Encoding.UTF8.GetString(obj);

            List<int> list = JsonHelper.ToObject<List<int>>(str);
            
            NetActions.CreateBombHandle?.Invoke(list);
        }

        #endregion
        
        #region ExitGameMsg

        public void ExitGameMsgRpcMsg(bool isServer = false, bool isRpc = true, IPEndPoint ipEndPoint = null)
        {
            Send(1012,"ExitGame",isServer,isRpc,ipEndPoint);
        }
        
        private void ExitGameMsgHandle()
        {
            NetActions.ExitGameHandle?.Invoke();
        }

        #endregion
        
        #region JoinGameMsg
        
        public void SendExitLobbyMsg(string _name, bool isServer = false, bool isRpc = true, IPEndPoint ipEndPoint = null)
        {
            Send(1013,_name,isServer,isRpc,ipEndPoint);
        }
        
        private void ExitLobbyMsgHandle(byte[] obj)
        {
            var str = Encoding.UTF8.GetString(obj);

            NetActions.ExitLobbyHandle?.Invoke(str);
        }
        
        #endregion
        
        private void Send(int id, string str, bool isServer, bool isRpc, IPEndPoint ipEndPoint)
        {
            //Convert to byte[]
            var body = Encoding.UTF8.GetBytes(str);

            //Package Body Size 4, Message ID 4, Package Body Content
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
                try
                {
                    ClientManager.Singleton.Send(send_buff);
                }
                catch{}
            }
        }
    }
}