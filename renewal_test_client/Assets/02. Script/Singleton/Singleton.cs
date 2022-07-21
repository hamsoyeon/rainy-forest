using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace test_client_unity
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        // Destroy 여부 확인용
        private static bool _ShuttingDown = false;
        private static object _Lock = new object();
        private static T m_instance;

        public static T Instance
        {
            get
            {
                // 게임 종료시 obj보다 싱글톤의 onDestroy가 먼저 실행 될 수도 있다.
                // 해당 싱글톤을 gameobj.Ondestroy()에서는 사용하지 않거나 사용하면 null 체크를 해주자.
                if (_ShuttingDown)
                {                   
                    Debug.Log("[Singleton] Instance '" + typeof(T)
                        + "' already destroyed. Returning null.");
                    return null;
                }

                lock (_Lock) // Thread Safe
                {
                    if (m_instance == null)
                    {
                        // 인스턴트 존재 여부 확인
                        m_instance = (T)FindObjectOfType(typeof(T));

                        // 아직 생성되지 않았다면 인스턴스 생성
                        if (m_instance == null)
                        {
                            // 새로운 게임오브젝트를 만들어서 싱글톤 Attach
                            var singletonObject = new GameObject();
                            m_instance = singletonObject.AddComponent<T>();
                            singletonObject.name = typeof(T).ToString() + "(Singleton)";

                            // Make instance persistent.
                            DontDestroyOnLoad(singletonObject);
                        }
                    }
                    return m_instance;
                }
            }
        }

        private void OnApplicationQuit()
        {
            _ShuttingDown = true;
        }

        private void OnDestroy()
        {
            _ShuttingDown = true;
        }
    }
}

