using System;
using System.Collections.Generic;
using Client;
using Helper;
using Model;
using Net.Actions;
using Net.Datas;
using TMPro;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UI;
using Utilitys;

namespace UI.StartScene
{
    public class RoomPanel : BaseSingleton<RoomPanel>
    {
        [SerializeField] private TMP_Text m_Title;

        [SerializeField] private Transform MsgParent;

        [SerializeField] private Button m_SendBtn;
        [SerializeField] private TMP_InputField m_InputMsg;

        [SerializeField] private Transform PlayerListParent;
        
        private GameObject m_MsgGo;
        private GameObject m_PlayerUI;

        private List<RoomPlayerUI> m_PlayerUIList;

        protected override void Awake()
        {
            base.Awake();
            
            m_MsgGo = Resources.Load<GameObject>("Prefabs/ChatMsg");
            m_PlayerUI = Resources.Load<GameObject>("Prefabs/RoomPlayerUI");

            m_PlayerUIList = new List<RoomPlayerUI>();
            
            m_SendBtn.onClick.AddListener(SendMsg);
            
            NetActions.ChatHandle += AddMsg;
            NetActions.ReadyHandle += Ready;
            NetActions.RoomPlayerUIHandle += HandleJoinLobby;
            NetActions.JoinHandle += ServerJoin;
        }

        private void OnEnable()
        {
            ShowIpTitel();

            SendJoinLobby();
        }

        private void ShowIpTitel()
        {
            if (GameModel.IsServer)
            {
                m_Title.text = "UDP Server:" + NetTool.GetIPAddress().ToString();
            }
            else
            {
                m_Title.text = "UDP Client:" + ClientManager.Singleton.GetIp();
            }
        }

        private void AddMsg(ChatMsg chatMsg)
        {
            var go = GameObject.Instantiate(m_MsgGo, MsgParent);
            go.GetComponent<TMP_Text>().text = $"{chatMsg.playerName}:{chatMsg.msg}";
        }

        private void SendMsg()
        {
            if (GameModel.IsServer)
            {
                var chat = new ChatMsg();
                chat.playerName = GameModel.MyName;
                chat.msg = m_InputMsg.text;
                
                AddMsg(chat);
                
                MessageManager.Singleton.SendChatMsg(GameModel.MyName,m_InputMsg.text,true);
            }
            else
            {
                MessageManager.Singleton.SendChatMsg(GameModel.MyName,m_InputMsg.text);
            }
        }

        private void SendJoinLobby()
        {
            if (GameModel.IsServer)
            {
                AddPlayerUI(GameModel.MyName);
            }
            else
            {
                MessageManager.Singleton.SendJoinMsg(GameModel.MyName);
            }
        }

        private void HandleJoinLobby(List<string> names)
        {
            foreach (var item in m_PlayerUIList)
            {
                GameObject.Destroy(item);
            }
            
            m_PlayerUIList.Clear();

            foreach (var name in names)
            {
                AddPlayerUI(name);
            }
        }
        
        private void AddPlayerUI(string name)
        {
            var go = GameObject.Instantiate(m_PlayerUI, PlayerListParent);

            var playerUI = go.GetComponent<RoomPlayerUI>();
            
            m_PlayerUIList.Add(playerUI);
            
            playerUI.Init(name);
        }

        private void ServerJoin(string name)
        {
            AddPlayerUI(name);
            
            List<string> names = new List<string>();

            foreach (var item in m_PlayerUIList)
            {
                names.Add(item.MyName);
            }

            MessageManager.Singleton.SendJoinRpcMsg(names);
        }
        
        private void Ready(string _name)
        {
            Debug.Log("m_PlayerUIList:" + m_PlayerUIList.Count);
            
            foreach (var item in m_PlayerUIList)
            {
                if (item.MyName == _name)
                {
                    item.Ready();
                    return;
                }
            }
        }
    }
}