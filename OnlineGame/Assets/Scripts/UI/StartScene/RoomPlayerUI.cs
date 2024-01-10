using System;
using Helper;
using Model;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.StartScene
{
    public class RoomPlayerUI : MonoBehaviour
    {
        [HideInInspector] public string MyName;

        [SerializeField] private Button m_ReadyBtn;
        [SerializeField] private TMP_Text m_NameText;

        private void Start()
        {
            m_ReadyBtn.onClick.AddListener(OnclickReadyBtn);
        }

        public void Init(string name)
        {
            MyName = name;

            m_NameText.text = name;

            if (GameModel.MyName != MyName)
            {
                m_ReadyBtn.interactable = false;
            }
        }

        private void OnclickReadyBtn()
        {
            if (GameModel.IsServer)
            {
                Ready();
                
                MessageManager.Singleton.SendReadyMsg(GameModel.MyName,true);
            }
            else
            {
                Ready();
                MessageManager.Singleton.SendReadyMsg(GameModel.MyName);
            }
        }

        public void Ready()
        {
            m_ReadyBtn.interactable = false;

            if (!GetReadyState())
            {
                m_NameText.text += "(Ready)";
            }
        }

        public bool GetReadyState()
        {
            return m_NameText.text.Contains("(Ready)");
        }

        public void SetNon()
        {
            this.transform.SetParent(null);
            
            this.transform.position = Vector3.zero;
        }
    }
}