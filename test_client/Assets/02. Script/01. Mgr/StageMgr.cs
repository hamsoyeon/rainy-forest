using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace test_client_unity
{
    public class StageMgr : Singleton<StageMgr>
    {
        public enum STAGE
        {
            LOGIN,
            JOIN,
            MENU,
            MAIN_MENU,
            LOBBY,
        };

        [System.Serializable]
        public class SerializeStateDic : test_client_unity.SerializableDictionary<STAGE, GameObject>
        {

        }

        public SerializeStateDic m_state_dic = new SerializeStateDic();

        public void ChangeState(STAGE _pre_stage, STAGE _cur_stage) // state 전환
        {
            m_state_dic.TryGetValue(_pre_stage, out GameObject pre_obj);
            pre_obj.SetActive(false);
            m_state_dic.TryGetValue(_cur_stage, out GameObject cur_obj);
            cur_obj.SetActive(true);
        }

        public void OnState(STAGE _stage) // state 켜주기
        {
            m_state_dic.TryGetValue(_stage, out GameObject cur_obj);
            cur_obj.SetActive(true);
        }

        public void OffState(STAGE _stage) // state 꺼주기
        {
            m_state_dic.TryGetValue(_stage, out GameObject cur_obj);
            cur_obj.SetActive(false);
        }
    }
}

