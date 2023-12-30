using UnityEngine;

namespace Utilitys
{
    public class BaseSingleton<T> : MonoBehaviour where T : BaseSingleton<T>
    {
        private static T _singleton;

        public static T Singleton
        {
            get { return _singleton; }
        }

        public static bool IsInitialized
        {
            get { return _singleton != null; }
        }
        
        protected virtual void Awake()
        {
            if (_singleton != null)
            {
                Destroy(gameObject);
            }
            else
            {
                _singleton = (T)this;
            }
        }

        protected void OnDestroy()
        {
            if (_singleton == this)
            {
                _singleton = null;
            }
        }
    }
}