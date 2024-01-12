using UnityEngine;
using Utilitys;

namespace GamePlay
{
    public class AudioManager : BaseSingleton<AudioManager>
    {
        public AudioClip AttackMusic;
        public AudioClip EndMusic;
        
        
        private AudioSource m_AudioSource;

        protected override void Awake()
        {
            base.Awake();
            m_AudioSource = GetComponent<AudioSource>();
        }

        public void PlayAttack()
        {
            m_AudioSource.PlayOneShot(AttackMusic);
        }
        
        public void PlayEnd()
        {
            m_AudioSource.PlayOneShot(EndMusic);
        }
    }
}