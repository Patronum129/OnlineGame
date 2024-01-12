using System;
using System.Collections;
using Helper;
using Model;
using Net.Actions;
using UnityEngine;

namespace GamePlay
{
    public class Bomb : MonoBehaviour
    {
        public int ID;
        
        private bool m_IsDestory;

        private bool m_IsBoom;
        private float timer;
        
        private void Start()
        {
            GetComponent<Animator>().SetTrigger("Bomb");
                
            GameObject.Destroy(transform.gameObject,2.3f);

            StartCoroutine(BoomMusic());
        }

        private IEnumerator BoomMusic()
        {
            yield return new WaitForSeconds(1.5f);

            var clip = GetComponentInChildren<AudioSource>().clip;
            
            GetComponentInChildren<AudioSource>().PlayOneShot(clip);
        }
        
        private void Update()
        {
            if (!GameModel.IsServer) return;
            
            if (!m_IsBoom)
            {
                timer += Time.deltaTime;

                if (timer > 1.5f)
                {
                    m_IsBoom = true;

                    var rays = Physics2D.CircleCastAll(this.transform.position, 3f, Vector2.zero);

                    foreach (var item in rays)
                    {
                        if (item.transform.GetComponent<Entity>())
                        {
                            item.transform.GetComponent<Entity>().Die();
                        }
                    }
                }
            }
        }
    }
}