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

            NetMgr.t_Eve teve = new NetMgr.t_Eve();
            teve.buf = new Byte[4096];
            Array.Copy(_buf, teve.buf, _buf.Length);

            switch (_protocol) // �������ݿ� ���� ����ŷ
            {
                case (int)LoginMgr.SUB_PROTOCOL.LOGIN_RESULT:
                    PacketMgr.Instance.Unpackpacket(_buf, ref result, ref msg);
                    Debug.Log(msg);

                    teve.eve = (int)EVENT.LOGIN_SUCESS;

                    // ���⼱ null�� ������ �ȵȴ�.. ������ ����..
                    // �α��� ������ �޴��� ����.
                    if (result == 1)
                    {
                        NetMgr.Instance.GetQue.Enqueue(teve);
                        //LoginMgr.Instance.EnterMenu(); // ť�� �����
                    }

                    break;
                case (int)LoginMgr.SUB_PROTOCOL.JOIN_RESULT:
                    PacketMgr.Instance.Unpackpacket(_buf, ref result, ref msg);
                    Debug.Log(msg);
                    break;
                case (int)LoginMgr.SUB_PROTOCOL.LOGOUT_RESULT:
                    PacketMgr.Instance.Unpackpacket(_buf, ref msg);
                    Debug.Log(msg);
                    break;
                case (int)LoginMgr.SUB_PROTOCOL.LOBBY_RESULT: // �κ� �����Ѵٴ� ��Ŷ�� �ް�...
                    PacketMgr.Instance.Unpackpacket(_buf, ref msg);

                    // �κ� �����Ѵٴ� �̺�Ʈ�� ���ش�.
                    teve.eve = (int)EVENT.ENTER_LOBBY;
                    NetMgr.Instance.GetQue.Enqueue(teve);

                    Debug.Log(msg);
                    break;
            }
        }

        public override void RecvEvent(NetMgr.t_Eve _teve)
        {
            switch (_teve.eve)
            {
                case (int)EVENT.LOGIN_SUCESS:
                    LoginMgr.Instance.EnterMenu();
                    break;
                case (int)EVENT.ENTER_LOBBY:
                    NetMgr.Instance.Cur_State = StateMgr.Instance.GetLobby;
                    LoginMgr.Instance.EnterLobby();
                    break;
            }
        }
    }
}