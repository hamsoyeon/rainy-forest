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

            switch (_protocol) // 프로토콜에 따라서 언패킹
            {
                case (int)LoginMgr.SUB_PROTOCOL.LOGIN_RESULT:
                    PacketMgr.Instance.Unpackpacket(_buf, ref result, ref msg);
                    Debug.Log(msg);

                    teve.eve = (int)EVENT.LOGIN_SUCESS;

                    // 여기선 null을 던져서 안된다.. 이유가 뭘까..
                    // 로그인 성공시 메뉴로 들어간다.
                    if (result == 1)
                    {
                        NetMgr.Instance.GetQue.Enqueue(teve);
                        //LoginMgr.Instance.EnterMenu(); // 큐를 만들어
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
                case (int)LoginMgr.SUB_PROTOCOL.LOBBY_RESULT: // 로비에 입장한다는 패킷을 받고...
                    PacketMgr.Instance.Unpackpacket(_buf, ref msg);

                    // 로비에 입장한다는 이벤트를 켜준다.
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