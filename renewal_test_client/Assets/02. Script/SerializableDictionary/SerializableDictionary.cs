using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace test_client_unity
{
    // 인스펙터에 딕셔너리를 띄우게 하는 작업
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
        /// 인스펙터를 딕셔너리로 초기화
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
        /// 딕셔너리를 인스펙터로 초기화
        ///
        public void SyncDictionaryFromInspector()
        {
            foreach (var key in Keys.ToList())
            {
                base.Remove(key);
            }
            for (int i = 0; i < g_InspectorKeys.Count; i++)
            {
                // 중복키가 있다면 에러를 출력한다.
                if (this.ContainsKey(g_InspectorKeys[i]))
                {
                    Debug.LogError("중복키 발생.");
                    break;
                }
                base.Add(g_InspectorKeys[i], g_InspectorValues[i]);
            }
        }
        public void OnAfterDeserialize()
        {
            Debug.Log(this + string.Format("인스펙터 키 수 : {0} 값 수 : {1}",
                g_InspectorKeys.Count, g_InspectorValues.Count));

            // 인스펙터의 key value가 keyvaluepair 형태를 띌 경우
            if (g_InspectorKeys.Count == g_InspectorValues.Count)
            {
                SyncDictionaryFromInspector();
            }
        }
    }
}
