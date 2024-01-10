using System;
using System.Collections;
using System.Collections.Generic;
using Helper;
using Model;
using Net.Actions;
using Net.NetComponents;
using UnityEngine;

namespace GamePlay
{
    public class Entity : MonoBehaviour
    {
        [HideInInspector] public string MyName;
        
        [HideInInspector] public bool IsLocalPlayer;
        private Animator m_Animator;

        private Animator m_SwordAnimator;
        private Animator m_WeaponAnimator;
        
        private Rigidbody2D m_Rigidbody;

        private BoxCollider2D m_WeaponCollider;
        
        [HideInInspector] public Vector2 Dir;

        private NetAnimator m_NetAnimator;

        public bool IsDie;
        
        private void Awake()
        {
            m_Animator = GetComponent<Animator>();
            m_Rigidbody = GetComponent<Rigidbody2D>();

            m_SwordAnimator = transform.Find("sword_attack_side-overlay").GetComponent<Animator>();
            m_WeaponAnimator = transform.Find("weapon_sword_side").GetComponent<Animator>();
            
            m_WeaponCollider = transform.Find("weapon_sword_side").GetComponent<BoxCollider2D>();

            NetActions.DieHandle += DieHandle;
            NetActions.ExitGameHandle += ExitGameHandle;
        }

        public void Init(int i)
        {
            MyName = GameModel.PlayerList[i];

            IsLocalPlayer = MyName == GameModel.MyName;
            
            GetComponent<NetTransform>().Init();
            m_NetAnimator = GetComponent<NetAnimator>();
            m_NetAnimator.Init();
        }

        private void Update()
        {
            if (!IsLocalPlayer || IsDie) return;

            var tempDir = Vector2.zero;
            
            if (Input.GetKey(KeyCode.W))
            {
                tempDir += Vector2.up;
            }
            
            if (Input.GetKey(KeyCode.S))
            {
                tempDir += Vector2.down;
            }
            
            if (Input.GetKey(KeyCode.A))
            {
                tempDir += Vector2.left;
            }
            
            if (Input.GetKey(KeyCode.D))
            {
                tempDir += Vector2.right;
            }

            if (tempDir.x > 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }

            if (tempDir.x < 0)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }

            Dir = tempDir;
        }

        public void SetTrigger(string _name)
        {
            m_Animator.SetTrigger(_name);

            if (_name == "Attack")
            {
                m_WeaponAnimator.SetTrigger(_name);
                m_SwordAnimator.SetTrigger(_name);
            }

            m_NetAnimator.SyncAnimator(_name);
        }

        public void SetSpeed(int speed)
        {
            m_Rigidbody.velocity = speed * Dir;
        }

        public bool DetectorWeapon(out List<Entity> list)
        {
            list = new List<Entity>();

            var raycasts = Physics2D.BoxCastAll((Vector2)m_WeaponCollider.transform.position + m_WeaponCollider.offset, m_WeaponCollider.size, 0,Vector2.zero);

            foreach (var item in raycasts)
            {
                var entity = item.transform.GetComponent<Entity>();

                if (entity != null)
                {
                    list.Add(entity);
                }
            }
            
            return list.Count > 0;
        }

        public void Die()
        {
            if (GameModel.IsServer)
            {
                MessageManager.Singleton.SendDieMsg(MyName,true);

                DieHandle(MyName);
            }
            else
            {
                MessageManager.Singleton.SendDieMsg(MyName);
            }
        }

        private void DieHandle(string _name)
        {
            if (_name == MyName)
            {
                if (IsDie) return;

                IsDie = true;
                
                GameManager.Singleton.DieSum++;
                
                GameManager.Singleton.CheckWin();

                StartCoroutine(SetScale());
            }
        }

        private IEnumerator SetScale()
        {
            for(int i=0;i<500;i++)
            {
                this.transform.localScale = Vector3.zero;
                yield return null;
            }
        }

        private void ExitGameHandle()
        {
            GameManager.Singleton.ShowWinPanel("Game Over");
        }

        private void OnDestroy()
        {
            NetActions.DieHandle -= DieHandle;
            NetActions.ExitGameHandle -= ExitGameHandle;
        }
    }
}