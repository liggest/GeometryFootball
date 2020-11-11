using UnityEngine;

namespace BallScripts.Utils {
    /// <summary>
    /// Inherit from this base class to create a singleton.
    /// e.g. public class MyClassName : Singleton<MyClassName> {}
    /// </summary>
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        // Check to see if we're about to be destroyed.
        private static bool m_ShuttingDown = false;
        private static object m_Lock = new object();
        private static T m_Instance;

        public static bool HasNoInstance { get => m_Instance == null; }

        /// <summary>
        /// 手动创建实例。最好不要用这个方法，直接.instance。
        /// </summary>
        /// <param name="noDestroyOnLoad">单例是否不因转换场景而摧毁</param>
        public static void InitInstance(bool noDestroyOnLoad = true)
        {
            if (m_Instance == null)
            {
                // Need to create a new GameObject to attach the singleton to.
                var singletonObject = new GameObject();
                m_Instance = singletonObject.AddComponent<T>();
                singletonObject.name = typeof(T).ToString() + "[单例]";

                if (noDestroyOnLoad)
                {
                    // Make instance persistent.
                    DontDestroyOnLoad(singletonObject); 
                }
            }
        }

        public static void Refresh()
        {
            if (m_ShuttingDown)
            {
                m_Instance = null;
                m_ShuttingDown = false;
                return;
            }
            Debug.Log($"[单例] {typeof(T)}实例尚未被销毁");
        }

        /// <summary>
        /// Access singleton instance through this propriety.
        /// </summary>
        public static T instance
        {
            get
            {
                if (m_ShuttingDown)
                {
                    Debug.LogWarning($"[单例] {typeof(T)}实例已被销毁，无法使用");
                    return null;
                }

                lock (m_Lock) //用static的空对象确保线程安全
                {
                    if (m_Instance == null)
                    {
                        // Search for existing instance.
                        m_Instance = (T)FindObjectOfType(typeof(T)); //只在场景中作为单例

                        InitInstance(); //如果场景中没有单例，就新建一个，并在全局作为单例
                    }

                    return m_Instance;
                }
            }
        }


        private void OnApplicationQuit()
        {
            m_ShuttingDown = true;
        }


        private void OnDestroy()
        {
            m_ShuttingDown = true;
        }
    }
}

