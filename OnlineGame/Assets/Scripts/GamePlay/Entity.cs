using System;
using System.Collections.Generic;
using Model;
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
        
        private void Awake()
        {
            m_Animator = GetComponent<Animator>();
            m_Rigidbody = GetComponent<Rigidbody2D>();

            m_SwordAnimator = transform.Find("sword_attack_side-overlay").GetComponent<Animator>();
            m_WeaponAnimator = transform.Find("weapon_sword_side").GetComponent<Animator>();
            
            m_WeaponCollider = transform.Find("weapon_sword_side").GetComponent<BoxCollider2D>();
        }

        public void Init(int i)
        {
            MyName = GameModel.PlayerList[i];

            IsLocalPlayer = MyName == GameModel.MyName;
            
            GetComponent<NetTransform>().Init();
        }

        private void Update()
        {
            if (!IsLocalPlayer) return;

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
            
        }
    }
}