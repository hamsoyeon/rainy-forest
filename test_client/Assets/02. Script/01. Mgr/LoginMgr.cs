using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

// Send하는 부분은 여따가 놓기!!!

namespace test_client_unity
{
    public class LoginMgr : Singleton<LoginMgr>
    {
        public enum SUB_PROTOCOL
        {
            NONE,
            LOGIN_INFO,
            LOGIN_RESULT,
            JOIN_INFO,
            JOIN_RESULT,
            LOGOUT_INFO,
            LOGOUT_RESULT,
            LOBBY_ENTER,
            LOBBY_RESULT,
            LOGIN = 1,
            JOIN,
            LOGOUT,
            LOBBY,
            MAX
        };

        public enum FIELD_TYPE
        {
            LOGIN_ID,
            LOGIN_PW,

            JOIN_ID,
            JOIN_PW,
            JOIN_NICK,
        }

        public enum BTN_TYPE
        {

        }

        [System.Serializable]
        public class Input_Field_Dic : test_client_unity.SerializableDictionary<FIELD_TYPE, InputField> { }
        public class Button_Dic : test_client_unity.SerializableDictionary<BTN_TYPE, Button> { }

        public Input_Field_Dic m_fieldDic = new Input_Field_Dic();

        public Button_Dic m_btnDic = new Button_Dic();

        void Start()
        {

        }

        // 로그인 버튼을 눌렀을 때 호출되는 함수
        public void OnLoginBtn()
        {
            m_fieldDic.TryGetValue(FIELD_TYPE.LOGIN_ID, out InputField login_id);
            m_fieldDic.TryGetValue(FIELD_TYPE.LOGIN_PW, out InputField login_pw);

            if (login_id.text == "" &&
                login_pw.text == "")
            {
                return;
            }
            else
            {
                // 필드에 값을 입력하였다면 서버로 로그인 정보를 전송한다.

                uint protocol = 0;
                ProtocolMgr.Instance.AddSubProtocol(ref protocol, (uint)LoginMgr.SUB_PROTOCOL.LOGIN_INFO);

                Byte[] buf = new Byte[4096];
                int size = Packpacket(
                    ref buf,
                    (int)protocol,
                    login_id.text,
                    login_pw.text);

                NetMgr.Instance.m_netWork.Send(buf, size);

                Debug.Log("로그인 시도");

            }
        }

        // 가입 버튼을 눌렀을 때 호출되는 함수
        public void OnJoinBtn()
        {
            m_fieldDic.TryGetValue(FIELD_TYPE.JOIN_ID, out InputField join_id);
            m_fieldDic.TryGetValue(FIELD_TYPE.JOIN_PW, out InputField join_pw);
            m_fieldDic.TryGetValue(FIELD_TYPE.JOIN_NICK, out InputField join_nick);

            if (join_id.text == "" ||
                join_pw.text == "" ||
                join_nick.text == "")
            {
                return;
            }
            else
            {
                uint protocol = 0;
                ProtocolMgr.Instance.AddSubProtocol(ref protocol, (uint)LoginMgr.SUB_PROTOCOL.JOIN_INFO);

                Byte[] buf = new Byte[4096];
                int size = Packpacket(
                    ref buf,
                    (int)protocol,
                    join_id.text,
                    join_pw.text,
                    join_nick.text);

                NetMgr.Instance.m_netWork.Send(buf, size);

                Debug.Log("가입 시도");
            }
        }

        public void OnEnterLobbyBtn() // multi SEND!!!
        {
            StageMgr.Instance.OffState(StageMgr.STAGE.MENU);

            uint protocol = 0;
            ProtocolMgr.Instance.AddSubProtocol(ref protocol, (uint)LoginMgr.SUB_PROTOCOL.LOBBY_ENTER);

            Byte[] buf = new Byte[4096];
            int size = Packpacket(
                ref buf,
                (int)protocol);

            NetMgr.Instance.m_netWork.Send(buf, size);

            Debug.Log("Enter 로비");
        }

        /// stage 전환
        /// <summary>
        /// 메뉴 입장 버튼 로그인 성공시 호출된다.
        /// </summary>
        public void EnterMenu()
        {
            StageMgr.Instance.ChangeState(StageMgr.STAGE.LOGIN, StageMgr.STAGE.MENU);

            Debug.Log("Enter 메뉴화면");
        }

        public void EnterLobby() // 멀티선택시 로비로 입장.
        {
            StageMgr.Instance.ChangeState(StageMgr.STAGE.MAIN_MENU, StageMgr.STAGE.LOBBY);

            uint protocol = 0;
            ProtocolMgr.Instance.AddSubProtocol(ref protocol, (uint)LobbyMgr.SUB_PROTOCOL.ROOMLIST_UPDATE);
            ProtocolMgr.Instance.AddDetailProtocol(ref protocol, (uint)LobbyMgr.DETAIL_PROTOCOL.MULTI);

            Byte[] buf = new Byte[4096]; // 방을 달라는 패킷을 보낸다.
            int size = Packpacket(
                ref buf,
                (int)protocol);

            NetMgr.Instance.m_netWork.Send(buf, size);
        }

        public void OnLogoutBtn() // 로그아웃
        {
            StageMgr.Instance.ChangeState(StageMgr.STAGE.MENU, StageMgr.STAGE.LOGIN);

            Byte[] buf = new Byte[4096];
            int size = Packpacket(
                ref buf,
                (int)LoginMgr.SUB_PROTOCOL.LOGOUT_INFO);

            NetMgr.Instance.m_netWork.Send(buf, size);

            Debug.Log("로그아웃");
        }

        #region packing, unpacking

        public int Packpacket(ref Byte[] _buf, int _protocol)
        {
            // 전체 데이터 사이즈 / 프로토콜 / 전체 데이터 사이즈
            int size = sizeof(int) + sizeof(int);
            int len = 0;

            BitConverter.GetBytes(size).CopyTo(_buf, len);
            len = len + sizeof(int);

            BitConverter.GetBytes(_protocol).CopyTo(_buf, len);
            len = len + sizeof(int);

            BitConverter.GetBytes(size).CopyTo(_buf, len);
            len = len + sizeof(int);

            return len;
        }

        public int Packpacket(ref Byte[] _buf, int _protocol, string _id, string _pw)
        {
            int id_size = _id.Length * 2;
            int pw_size = _pw.Length * 2;

            // 전체 데이터 사이즈 / 프로토콜 / 전체 데이터 사이즈 / ID 사이즈 / ID / PW 사이즈 / PW
            int size = sizeof(int) + sizeof(int) + id_size + sizeof(int) + pw_size + sizeof(int); // 총사이즈
            int len = 0;

            // 전체 데이터 사이즈
            BitConverter.GetBytes(size).CopyTo(_buf, len);
            len = len + sizeof(int);

            BitConverter.GetBytes(_protocol).CopyTo(_buf, len);
            len = len + sizeof(int);

            // 전체 데이터 사이즈 +
            BitConverter.GetBytes(size).CopyTo(_buf, len);
            len = len + sizeof(int);

            BitConverter.GetBytes(id_size).CopyTo(_buf, len);
            len = len + sizeof(int);

            Encoding.Unicode.GetBytes(_id).CopyTo(_buf, len);
            len = len + id_size;

            BitConverter.GetBytes(pw_size).CopyTo(_buf, len);
            len = len + sizeof(int);

            Encoding.Unicode.GetBytes(_pw).CopyTo(_buf, len);
            len = len + pw_size;

            return len;
        }

        public int Packpacket(ref Byte[] _buf, int _protocol, string _id, string _pw, string _nick)
        {
            int id_size = _id.Length * 2;
            int pw_size = _pw.Length * 2;
            int nick_size = _nick.Length * 2;

            // 전체 데이터 사이즈 / 프로토콜 / 전체 데이터 사이즈 / ID 사이즈 / ID / PW 사이즈 / PW / NICK 사이즈 / NICK
            int size = sizeof(int) + sizeof(int) + id_size + sizeof(int) + pw_size + sizeof(int) + nick_size + sizeof(int); // 총사이즈
            int len = 0;

            // 전체 데이터 사이즈
            BitConverter.GetBytes(size).CopyTo(_buf, len);
            len = len + sizeof(int);

            BitConverter.GetBytes(_protocol).CopyTo(_buf, len);
            len = len + sizeof(int);

            // 전체 데이터 사이즈 +
            BitConverter.GetBytes(size).CopyTo(_buf, len);
            len = len + sizeof(int);

            BitConverter.GetBytes(id_size).CopyTo(_buf, len);
            len = len + sizeof(int);

            Encoding.Unicode.GetBytes(_id).CopyTo(_buf, len);
            len = len + id_size;

            BitConverter.GetBytes(pw_size).CopyTo(_buf, len);
            len = len + sizeof(int);

            Encoding.Unicode.GetBytes(_pw).CopyTo(_buf, len);
            len = len + pw_size;

            BitConverter.GetBytes(nick_size).CopyTo(_buf, len);
            len = len + sizeof(int);

            Encoding.Unicode.GetBytes(_nick).CopyTo(_buf, len);
            len = len + nick_size;

            return len;
        }

        #endregion

    }
}

