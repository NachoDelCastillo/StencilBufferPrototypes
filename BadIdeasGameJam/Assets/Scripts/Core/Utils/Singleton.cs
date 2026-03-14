using UnityEngine;

namespace Core.Singleton
{
    public class Singleton<T> : MonoBehaviour where T : Component
    {
        private static T __instance;

        public static T _instance
        {
            get { return __instance; }
            set
            {
                __instance = value;
            }
        }

        //private static T _instance;
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindFirstObjectByType<T>();
                    //if (_instance == null)
                    //{
                    //    GameObject obj = new GameObject
                    //    {
                    //        name = typeof(T).Name
                    //    };
                    //    _instance = obj.AddComponent<T>();
                    //}
                }
                return _instance;
            }
        }

        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
            }
            else if (_instance != this as T)
            {
                Destroy(gameObject);
            }
        }

        /* In order to use Awake, do it the next way
        * protected override void Awake()
        * {
        *     base.Awake();
        *     //Your code goes here
        * }
        * */
    }
}