using System;
using System.Collections;
using System.Collections.Generic;
using Helper;
using Model;
using Net.Actions;
using UI;
using UnityEngine;
using Utilitys;
using Random = UnityEngine.Random;

namespace GamePlay
{
    public class GameManager : BaseSingleton<GameManager>
    {
        [SerializeField,Tooltip("Start Position")]
        private Transform[] m_StartPos;

        [SerializeField,Tooltip("Player Prefab")]
        private GameObject[] m_PlayerGo;

        [SerializeField]
        private List<Transform> m_BombPos;
        
        [HideInInspector] public int DieSum;

        [SerializeField] private GameObject WinPanel;

        [SerializeField] private GameObject Bomb;

        private float m_Timer;
        
        private void Start()
        {
            NetActions.WinHandle += ShowWinPanel;
            NetActions.CreateBombHandle += CreateBombHandle;
            
            InitGame();
        }

        private void InitGame()
        {
            for (int i = 0; i < GameModel.PlayerList.Count; i++)
            {
                var go = GameObject.Instantiate(m_PlayerGo[i],m_StartPos[i].position,Quaternion.identity);
                
                go.GetComponent<Entity>().Init(i);
            }
        }

        private void Update()
        {
            if (!GameModel.IsServer) return;
            
            m_Timer += Time.deltaTime;

            if (m_Timer >= 3.3f)
            {
                m_Timer = 0;
                CreateAllBomb();
            }
        }

        public void CheckWin()
        {
            if (!GameModel.IsServer) return;

            StartCoroutine(DelayCheckWin());
        }

        private IEnumerator DelayCheckWin()
        {
            yield return new WaitForSeconds(0.1f);
            
            if (GameModel.PlayerList.Count - 1 == DieSum)
            {
                var entity = FindObjectOfType<Entity>();
                
                //RPC
                MessageManager.Singleton.SendWinMsg(entity.MyName,true);
                
                ShowWinPanel(entity.MyName);
            }
            
            if (GameModel.PlayerList.Count == DieSum)
            {
                var entity = FindObjectOfType<Entity>();
                
                //RPC
                MessageManager.Singleton.SendWinMsg("A tie",true);
                
                ShowWinPanel("A tie");
            }
        }
        
        public void ShowWinPanel(string _name)
        {
            StartCoroutine(DelayWin(_name));
        }

        private IEnumerator DelayWin(string _name)
        {
            yield return new WaitForSeconds(1.5f);
            
            WinPanel.SetActive(true);
            
            WinPanel.GetComponent<WinPanel>().Init(_name);
        }

        private void CreateAllBomb()
        {
            List<int> indexs = new List<int>();

            for (int i = 0; i < 20; i++)
            {
                int range = Random.Range(0, m_BombPos.Count);

                while (indexs.Contains(range))
                {
                    range = Random.Range(0, m_BombPos.Count);
                }
                
                indexs.Add(range);
            }

            MessageManager.Singleton.SendBombMsg(indexs,true);
            
            CreateBombHandle(indexs);
        }

        private void CreateBombHandle(List<int> list)
        {
            int sum = 0;
            foreach (var item in list)
            {
                var go = GameObject.Instantiate(Bomb, m_BombPos[item].position, Quaternion.identity);

                go.GetComponent<Bomb>().ID = sum;

                sum++;
            }
        }
    }
}