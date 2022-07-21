using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace test_client_unity
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        // Destroy ���� Ȯ�ο�
        private static bool _ShuttingDown = false;
        private static object _Lock = new object();
        private static T m_instance;

        public static T Instance
        {
            get
            {
                // ���� ����� obj���� �̱����� onDestroy�� ���� ���� �� ���� �ִ�.
                // �ش� �̱����� gameobj.Ondestroy()������ ������� �ʰų� ����ϸ� null üũ�� ������.
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
                        // �ν���Ʈ ���� ���� Ȯ��
                        m_instance = (T)FindObjectOfType(typeof(T));

                        // ���� �������� �ʾҴٸ� �ν��Ͻ� ����
                        if (m_instance == null)
                        {
                            // ���ο� ���ӿ�����Ʈ�� ���� �̱��� Attach
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

