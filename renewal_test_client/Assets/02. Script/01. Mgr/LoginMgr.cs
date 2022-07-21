using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

// Send하는 부분은 여따가 놓기!!!

namespace test_client_unity
{
    public class LoginMgr : Singleton<LoginMgr> // login, join mgr
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
            ENTER_LOGIN,
            ENTER_JOIN,
            JOIN,
            LOGIN
        }

        [System.Serializable]
        public class Input_Field_Dic : test_client_unity.SerializableDictionary<FIELD_TYPE, InputField> { }

        [System.Serializable]
        public class Button_Dic : test_client_unity.SerializableDictionary<BTN_TYPE, Button> { }

        public Input_Field_Dic m_fieldDic = new Input_Field_Dic();

        public Button_Dic m_btnDic = new Button_Dic();

        void Start()
        {
            // 이벤트 등록
            m_btnDic.TryGetValue(BTN_TYPE.ENTER_JOIN, out Button enter_join);
            enter_join.onClick.AddListener(Click_EnterJoinBtn);
            m_btnDic.TryGetValue(BTN_TYPE.ENTER_LOGIN, out Button enter_login);
            enter_login.onClick.AddListener(Click_EnterLoginBtn);
            m_btnDic.TryGetValue(BTN_TYPE.LOGIN, out Button login);
            login.onClick.AddListener(Click_LoginBtn);
            m_btnDic.TryGetValue(BTN_TYPE.JOIN, out Button join);
            join.onClick.AddListener(Click_JoinBtn);
        }

        // 가입화면 들어가기
        public void Click_EnterJoinBtn()
        {
            // TryGetValue
            // - Key값에 해당하는 value를 가져온다.
            WindowMgr.Instance.ChangeState(WindowMgr.WINDOW_TYPE.LOGIN, WindowMgr.WINDOW_TYPE.JOIN);

            Debug.Log("가입화면 들어가기");
        }
        // 로그인화면 들어가기
        public void Click_EnterLoginBtn()
        {
            WindowMgr.Instance.ChangeState(WindowMgr.WINDOW_TYPE.JOIN, WindowMgr.WINDOW_TYPE.LOGIN);

            Debug.Log("로그인 버튼 클릭");
        }

        // 로그인 버튼을 눌렀을 때 호출되는 함수
        public void Click_LoginBtn()
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
                t_Eve eve = new t_Eve();

                uint protocol = 0;
                ProtocolMgr.Instance.AddSubProtocol(ref protocol, (uint)LoginMgr.SUB_PROTOCOL.LOGIN_INFO);

                eve.buf_size = Packpacket(
                    ref eve.buf,
                    (int)protocol,
                    login_id.text,
                    login_pw.text);

                Debug.Log(login_id.text + ", " + login_pw.text + " 로그인 시도");

                NetMgr.Instance.m_sendQue.Enqueue(eve);
            }
        }
        // 가입 버튼을 눌렀을 때 호출되는 함수
        public void Click_JoinBtn()
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
            WindowMgr.Instance.OffState(WindowMgr.WINDOW_TYPE.MENU);

            t_Eve eve = new t_Eve();

            uint protocol = 0;
            ProtocolMgr.Instance.AddSubProtocol(ref protocol, (uint)LoginMgr.SUB_PROTOCOL.LOBBY_ENTER);

            eve.buf_size = NetMgr.Instance.m_netWork.PackPacket(
                ref eve.buf,
                (int)protocol,
                null,
                0);

            NetMgr.Instance.m_sendQue.Enqueue(eve);

            Debug.Log("Enter 로비");
        }

        /// stage 전환
        /// <summary>
        /// 메뉴 입장 버튼 로그인 성공시 호출된다.
        /// </summary>
        public void EnterMenu()
        {
            WindowMgr.Instance.ChangeState(WindowMgr.WINDOW_TYPE.LOGIN, WindowMgr.WINDOW_TYPE.MENU);

            Debug.Log("Enter 메뉴화면");
        }
        public void EnterLobby() // 멀티선택시 로비로 입장.
        {
            WindowMgr.Instance.ChangeState(WindowMgr.WINDOW_TYPE.MAIN_MENU, WindowMgr.WINDOW_TYPE.LOBBY);
            
            t_Eve eve = new t_Eve();

            uint protocol = 0;
            ProtocolMgr.Instance.AddSubProtocol(ref protocol, (uint)LobbyMgr.SUB_PROTOCOL.ROOMLIST_UPDATE);
            ProtocolMgr.Instance.AddDetailProtocol(ref protocol, (uint)LobbyMgr.DETAIL_PROTOCOL.MULTI);

            eve.buf_size = NetMgr.Instance.m_netWork.PackPacket(
                ref eve.buf,
                (int)protocol,
                null,
                0);

            NetMgr.Instance.m_sendQue.Enqueue(eve);

            Debug.Log("로비입장");
        }
        public void OnLogoutBtn() // 로그아웃
        {
            WindowMgr.Instance.ChangeState(WindowMgr.WINDOW_TYPE.MENU, WindowMgr.WINDOW_TYPE.LOGIN);

            t_Eve eve = new t_Eve();

            eve.buf_size = NetMgr.Instance.m_netWork.PackPacket(
                ref eve.buf,
                (int)LoginMgr.SUB_PROTOCOL.LOGOUT_INFO,
                null,
                0);

            NetMgr.Instance.m_sendQue.Enqueue(eve);

            Debug.Log("로그아웃");
        }

        #region packing, unpacking

        public int Packpacket(ref Byte[] _buf, int _protocol, string _id, string _pw)
        {
            _buf = new Byte[4096];

            Byte[] data_buf = new Byte[4096];

            int id_size = _id.Length * 2;
            int pw_size = _pw.Length * 2;

            // 전체 데이터 사이즈 / 프로토콜 / 전체 데이터 사이즈 / ID 사이즈 / ID / PW 사이즈 / PW
            int len = 0;

            BitConverter.GetBytes(id_size).CopyTo(data_buf, len);
            len = len + sizeof(int);

            Encoding.Unicode.GetBytes(_id).CopyTo(data_buf, len);
            len = len + id_size;

            BitConverter.GetBytes(pw_size).CopyTo(data_buf, len);
            len = len + sizeof(int);

            Encoding.Unicode.GetBytes(_pw).CopyTo(data_buf, len);
            len = len + pw_size;

            return NetMgr.Instance.m_netWork.PackPacket(ref _buf, _protocol, data_buf, len);
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

        public void Unpackpacket(Byte[] _buf, ref int _result, ref string _msg)
        {
            int len = sizeof(int) + sizeof(int) + sizeof(int);
            Byte[] msg_size = new Byte[4];
            Byte[] result = new Byte[4];

            Array.Copy(_buf, len, result, 0, sizeof(int));
            _result = BitConverter.ToInt32(result);
            len = len + sizeof(int);

            Array.Copy(_buf, len, msg_size, 0, sizeof(int));
            len = len + sizeof(int);

            _msg = Encoding.Unicode.GetString(_buf, len, BitConverter.ToInt32(msg_size));
        }

        public void Unpackpacket(Byte[] _buf, ref string _msg)
        {
            // 패킷넘버 / 프로토콜 / 데이터 사이즈
            int len = sizeof(int) + sizeof(int) + sizeof(int);
            Byte[] msg_size = new Byte[4];

            Array.Copy(_buf, len, msg_size, 0, sizeof(int));
            len = len + sizeof(int);

            _msg = Encoding.Unicode.GetString(_buf, len, BitConverter.ToInt32(msg_size));
        }

        #endregion
    }
}

