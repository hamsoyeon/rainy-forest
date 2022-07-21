using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace test_client_unity
{
    public class WindowMgr : Singleton<WindowMgr>
    {
        public enum WINDOW_TYPE
        {
            LOGIN,
            JOIN,
            MENU,
            MAIN_MENU,
            LOBBY,
        };

        [System.Serializable]
        public class SerializeStateDic : test_client_unity.SerializableDictionary<WINDOW_TYPE, GameObject>
        {

        }

        public SerializeStateDic m_state_dic = new SerializeStateDic();

        public void ChangeState(WINDOW_TYPE _pre_stage, WINDOW_TYPE _cur_stage) // state 전환
        {
            m_state_dic.TryGetValue(_pre_stage, out GameObject pre_obj);
            pre_obj.SetActive(false);
            m_state_dic.TryGetValue(_cur_stage, out GameObject cur_obj);
            cur_obj.SetActive(true);
        }

        public void OnState(WINDOW_TYPE _stage) // state 켜주기
        {
            m_state_dic.TryGetValue(_stage, out GameObject cur_obj);
            cur_obj.SetActive(true);
        }

        public void OffState(WINDOW_TYPE _stage) // state 꺼주기
        {
            m_state_dic.TryGetValue(_stage, out GameObject cur_obj);
            cur_obj.SetActive(false);
        }
    }
}