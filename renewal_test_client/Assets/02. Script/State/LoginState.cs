using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace test_client_unity
{
    public class LoginState : State
    {
        public enum EVENT
        {
            LOGIN_SUCESS,
            ENTER_LOBBY,
        }

        public override void Recv(Byte[] _buf, uint _protocol)
        {
            int result = new int();
            string msg = null;

            t_Eve teve = new t_Eve();
            teve.buf = new Byte[4096];
            Array.Copy(_buf, teve.buf, _buf.Length);

            switch (_protocol) // �������ݿ� ���� ����ŷ
            {
                case (int)LoginMgr.SUB_PROTOCOL.LOGIN_RESULT:
                    LoginMgr.Instance.Unpackpacket(_buf, ref result, ref msg);
                    Debug.Log(msg);

                    teve.eve = (int)EVENT.LOGIN_SUCESS;

                    // ���⼱ null�� ������ �ȵȴ�.. ������ ����..
                    // �α��� ������ �޴��� ����.
                    if (result == 1)
                    {
                        NetMgr.Instance.m_recvQue.Enqueue(teve);
                        //LoginMgr.Instance.EnterMenu(); // ť�� �����
                    }

                    break;
                case (int)LoginMgr.SUB_PROTOCOL.JOIN_RESULT:
                    LoginMgr.Instance.Unpackpacket(_buf, ref result, ref msg);
                    Debug.Log(msg);
                    break;
                case (int)LoginMgr.SUB_PROTOCOL.LOGOUT_RESULT:
                    LoginMgr.Instance.Unpackpacket(_buf, ref msg);
                    Debug.Log(msg);
                    break;
                case (int)LoginMgr.SUB_PROTOCOL.LOBBY_RESULT: // �κ� �����Ѵٴ� ��Ŷ�� �ް�...
                    LoginMgr.Instance.Unpackpacket(_buf, ref msg);

                    // �κ� �����Ѵٴ� �̺�Ʈ�� ���ش�.
                    teve.eve = (int)EVENT.ENTER_LOBBY;
                    NetMgr.Instance.m_recvQue.Enqueue(teve);

                    Debug.Log(msg);
                    break;
            }
        }

        public override void RecvEvent(t_Eve _teve)
        {
            switch (_teve.eve)
            {
                case (int)EVENT.LOGIN_SUCESS:
                    LoginMgr.Instance.EnterMenu();
                    break;
                case (int)EVENT.ENTER_LOBBY:
                    NetMgr.Instance.m_curState = NetMgr.Instance.m_lobbyState;
                    LoginMgr.Instance.EnterLobby();
                    break;
            }
        }

        public override void Send(Byte[] _buf, uint _protocol)
        {
            switch (_protocol)
            {

            }
        }

        public override void SendEvent(t_Eve _eve)
        {
            // �̺�Ʈ �߻��� send
            switch (_eve.eve)
            {

            }
        }
    }
}