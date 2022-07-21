using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace test_client_unity
{
    // �ν����Ϳ� ��ųʸ��� ���� �ϴ� �۾�
    [System.Serializable]
    public class SerializableDictionary<TKey, TValue> :
        Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField]
        private List<TKey> g_InspectorKeys;
        [SerializeField]
        private List<TValue> g_InspectorValues;

        public SerializableDictionary()
        {
            g_InspectorKeys = new List<TKey>();
            g_InspectorValues = new List<TValue>();
            SyncInspectorFromDictionary();
        }
        public new void Add(TKey _key, TValue _value)
        {
            base.Add(_key, _value);
            SyncInspectorFromDictionary();
        }
        public new void Remove(TKey _key)
        {
            base.Remove(_key);
            SyncInspectorFromDictionary();
        }
        public void OnBeforeSerialize()
        {

        }
        /// <summary>
        /// �ν����͸� ��ųʸ��� �ʱ�ȭ
        /// </summary>
        public void SyncInspectorFromDictionary()
        {
            g_InspectorKeys.Clear();
            g_InspectorValues.Clear();
            foreach (KeyValuePair<TKey, TValue> pair in this)
            {
                g_InspectorKeys.Add(pair.Key);
                g_InspectorValues.Add(pair.Value);
            }
        }
        ///
        /// ��ųʸ��� �ν����ͷ� �ʱ�ȭ
        ///
        public void SyncDictionaryFromInspector()
        {
            foreach (var key in Keys.ToList())
            {
                base.Remove(key);
            }
            for (int i = 0; i < g_InspectorKeys.Count; i++)
            {
                // �ߺ�Ű�� �ִٸ� ������ ����Ѵ�.
                if (this.ContainsKey(g_InspectorKeys[i]))
                {
                    Debug.LogError("�ߺ�Ű �߻�.");
                    break;
                }
                base.Add(g_InspectorKeys[i], g_InspectorValues[i]);
            }
        }
        public void OnAfterDeserialize()
        {
            Debug.Log(this + string.Format("�ν����� Ű �� : {0} �� �� : {1}",
                g_InspectorKeys.Count, g_InspectorValues.Count));

            // �ν������� key value�� keyvaluepair ���¸� �� ���
            if (g_InspectorKeys.Count == g_InspectorValues.Count)
            {
                SyncDictionaryFromInspector();
            }
        }
    }
}
