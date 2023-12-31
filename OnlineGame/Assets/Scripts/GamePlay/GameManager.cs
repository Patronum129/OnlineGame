using System;
using Model;
using Unity.Mathematics;
using UnityEngine;

namespace GamePlay
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField,Tooltip("开始的坐标")]
        private Transform[] m_StartPos;

        [SerializeField,Tooltip("玩家预制体")]
        private GameObject[] m_PlayerGo;
        
        private void Start()
        {
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
    }
}