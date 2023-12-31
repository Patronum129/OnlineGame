using System.Collections;
using Helper;
using Model;
using Net.Actions;
using UI;
using UnityEngine;
using Utilitys;

namespace GamePlay
{
    public class GameManager : BaseSingleton<GameManager>
    {
        [SerializeField,Tooltip("开始的坐标")]
        private Transform[] m_StartPos;

        [SerializeField,Tooltip("玩家预制体")]
        private GameObject[] m_PlayerGo;

        [HideInInspector] public int DieSum;

        [SerializeField] private GameObject WinPanel;
        
        private void Start()
        {
            NetActions.WinHandle += ShowWinPanel;
            
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

        public void CheckWin()
        {
            if (!GameModel.IsServer) return;

            if (GameModel.PlayerList.Count - 1 == DieSum)
            {
                var entity = FindObjectOfType<Entity>();
                
                //RPC
                MessageManager.Singleton.SendWinMsg(entity.MyName,true);
                
                ShowWinPanel(entity.MyName);
            }
        }

        private void ShowWinPanel(string _name)
        {
            StartCoroutine(DelayWin(_name));
        }

        private IEnumerator DelayWin(string _name)
        {
            yield return new WaitForSeconds(3f);
            
            WinPanel.SetActive(true);
            
            WinPanel.GetComponent<WinPanel>().Init(_name);
        }
    }
}