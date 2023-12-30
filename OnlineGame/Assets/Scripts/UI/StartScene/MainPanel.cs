using System;
using System.Collections;
using Client;
using Model;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.StartScene
{
    public class MainPanel : MonoBehaviour
    {
        [SerializeField] private TMP_InputField m_InputName;
        [SerializeField] private TMP_InputField m_InputIP;

        [SerializeField] private Button m_HostBtn;
        [SerializeField] private Button m_JoinBtn;

        private void Start()
        {
            m_HostBtn.onClick.AddListener(HostGame);
            m_JoinBtn.onClick.AddListener(JoinGame);
        }

        private void HostGame()
        {
            if (m_InputName.text == "") return;

            GameModel.MyName = m_InputName.text;
            
            var go = Resources.Load<GameObject>("Prefabs/ServerManager");

            GameObject.Instantiate(go);

            GameModel.IsServer = true;
            
            StartCoroutine(ShowRoom());
        }

        private void JoinGame()
        {
            if (m_InputName.text == "") return;
            if (m_InputIP.text == "") return;
            
            GameModel.MyName = m_InputName.text;

            var go = Resources.Load<GameObject>("Prefabs/ClientManager");

            GameObject.Instantiate(go);
            
            GameModel.IsServer = false;
            
            ClientManager.Singleton.Init(m_InputIP.text);
            
            StartCoroutine(ShowRoom());
        }

        private IEnumerator ShowRoom()
        {
            yield return new WaitForSeconds(0.1f);
            
            gameObject.SetActive(false);
            AllPanel.Singleton.RoomPanel.SetActive(true);
        }
    }
}