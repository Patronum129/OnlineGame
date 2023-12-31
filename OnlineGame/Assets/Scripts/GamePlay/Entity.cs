using System;
using Model;
using UnityEngine;

namespace GamePlay
{
    public class Entity : MonoBehaviour
    {
        [HideInInspector] public string MyName;
        
        private bool m_IsLocalPlayer;
        private Animator m_Animator;
        private Rigidbody2D m_Rigidbody;

        private void Awake()
        {
            m_Animator = GetComponent<Animator>();
            m_Rigidbody = GetComponent<Rigidbody2D>();
        }

        public void Init(int i)
        {
            MyName = GameModel.PlayerList[i];

            m_IsLocalPlayer = MyName == GameModel.MyName;
        }
    }
}