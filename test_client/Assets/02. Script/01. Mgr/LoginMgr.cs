using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

// Send�ϴ� �κ��� ������ ����!!!

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

        // �α��� ��ư�� ������ �� ȣ��Ǵ� �Լ�
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
                // �ʵ忡 ���� �Է��Ͽ��ٸ� ������ �α��� ������ �����Ѵ�.

                uint protocol = 0;
                ProtocolMgr.Instance.AddSubProtocol(ref protocol, (uint)LoginMgr.SUB_PROTOCOL.LOGIN_INFO);

                Byte[] buf = new Byte[4096];
                int size = Packpacket(
                    ref buf,
                    (int)protocol,
                    login_id.text,
                    login_pw.text);

                NetMgr.Instance.m_netWork.Send(buf, size);

                Debug.Log("�α��� �õ�");

            }
        }

        // ���� ��ư�� ������ �� ȣ��Ǵ� �Լ�
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

                Debug.Log("���� �õ�");
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

            Debug.Log("Enter �κ�");
        }

        /// stage ��ȯ
        /// <summary>
        /// �޴� ���� ��ư �α��� ������ ȣ��ȴ�.
        /// </summary>
        public void EnterMenu()
        {
            StageMgr.Instance.ChangeState(StageMgr.STAGE.LOGIN, StageMgr.STAGE.MENU);

            Debug.Log("Enter �޴�ȭ��");
        }

        public void EnterLobby() // ��Ƽ���ý� �κ�� ����.
        {
            StageMgr.Instance.ChangeState(StageMgr.STAGE.MAIN_MENU, StageMgr.STAGE.LOBBY);

            uint protocol = 0;
            ProtocolMgr.Instance.AddSubProtocol(ref protocol, (uint)LobbyMgr.SUB_PROTOCOL.ROOMLIST_UPDATE);
            ProtocolMgr.Instance.AddDetailProtocol(ref protocol, (uint)LobbyMgr.DETAIL_PROTOCOL.MULTI);

            Byte[] buf = new Byte[4096]; // ���� �޶�� ��Ŷ�� ������.
            int size = Packpacket(
                ref buf,
                (int)protocol);

            NetMgr.Instance.m_netWork.Send(buf, size);
        }

        public void OnLogoutBtn() // �α׾ƿ�
        {
            StageMgr.Instance.ChangeState(StageMgr.STAGE.MENU, StageMgr.STAGE.LOGIN);

            Byte[] buf = new Byte[4096];
            int size = Packpacket(
                ref buf,
                (int)LoginMgr.SUB_PROTOCOL.LOGOUT_INFO);

            NetMgr.Instance.m_netWork.Send(buf, size);

            Debug.Log("�α׾ƿ�");
        }

        #region packing, unpacking

        public int Packpacket(ref Byte[] _buf, int _protocol)
        {
            // ��ü ������ ������ / �������� / ��ü ������ ������
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

            // ��ü ������ ������ / �������� / ��ü ������ ������ / ID ������ / ID / PW ������ / PW
            int size = sizeof(int) + sizeof(int) + id_size + sizeof(int) + pw_size + sizeof(int); // �ѻ�����
            int len = 0;

            // ��ü ������ ������
            BitConverter.GetBytes(size).CopyTo(_buf, len);
            len = len + sizeof(int);

            BitConverter.GetBytes(_protocol).CopyTo(_buf, len);
            len = len + sizeof(int);

            // ��ü ������ ������ +
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

            // ��ü ������ ������ / �������� / ��ü ������ ������ / ID ������ / ID / PW ������ / PW / NICK ������ / NICK
            int size = sizeof(int) + sizeof(int) + id_size + sizeof(int) + pw_size + sizeof(int) + nick_size + sizeof(int); // �ѻ�����
            int len = 0;

            // ��ü ������ ������
            BitConverter.GetBytes(size).CopyTo(_buf, len);
            len = len + sizeof(int);

            BitConverter.GetBytes(_protocol).CopyTo(_buf, len);
            len = len + sizeof(int);

            // ��ü ������ ������ +
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

