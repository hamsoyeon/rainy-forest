using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace test_client_unity
{
    public class LobbyState : State
    {
        public enum RECV_EVENT
        {
            CREATE_ROOM,
            CHAT_FILED,
            CHANGE_STAGE,
        }

        public override void Recv(Byte[] _buf, Byte[] _protocol)
        {
            int result = new int();
            string msg = null;

            t_Eve teve = new t_Eve();
            teve.buf = new Byte[4096];
            teve.state_type = (int)STATE_TYPE.LOBBY_STATE;

            Array.Copy(_buf, teve.buf, _buf.Length);

            int sub_protocol = NetMgr.Instance.m_netWork.GetSubProtocol(_protocol);
            int detail_prototocol = NetMgr.Instance.m_netWork.GetDetailProtocol(_protocol);

            switch (sub_protocol) // 프로토콜에 따라서 언패킹
            {
                case (int)LobbyMgr.SUB_PROTOCOL.CHAT_RESULT:
                    {
                        switch (detail_prototocol)
                        {
                            case (int)LobbyMgr.SERVER_DETAIL_PROTOCOL.ALL_MSG_SUCCESS:
                                teve.eve = (int)RECV_EVENT.CHAT_FILED;
                                NetMgr.Instance.m_recvQue.Enqueue(teve);
                                break;
                            case (int)LobbyMgr.SERVER_DETAIL_PROTOCOL.ALL_MSG_FAIL:
                                break;
                        }
                    }
                    break;
                case (int)LobbyMgr.SUB_PROTOCOL.ROOM_RESULT: // 메시지를 받으면 채팅창에 올라간다.
                    {
                        switch (detail_prototocol)
                        {
                            case (int)LobbyMgr.SERVER_DETAIL_PROTOCOL.ROOMLIST_UPDATE_SUCCESS:
                                teve.eve = (int)RECV_EVENT.CREATE_ROOM;
                                NetMgr.Instance.m_recvQue.Enqueue(teve);
                                break;
                            case (int)LobbyMgr.SERVER_DETAIL_PROTOCOL.ROOMLIST_UPDATE_FAIL:
                                break;
                            case (int)LobbyMgr.SERVER_DETAIL_PROTOCOL.MAKE_ROOM_SUCCESS:
                                /// 룸생성에 성공하면 플레이어는 stage로 이동한다.
                                teve.eve = (int)RECV_EVENT.CHANGE_STAGE;
                                NetMgr.Instance.m_recvQue.Enqueue(teve);
                                break;
                            case (int)LobbyMgr.SERVER_DETAIL_PROTOCOL.MAKE_ROOM_FAIL:
                                break;
                            case (int)LobbyMgr.SERVER_DETAIL_PROTOCOL.OVER_PLAYERS: // 인원초과
                                break;
                            case (int)LobbyMgr.SERVER_DETAIL_PROTOCOL.ENTER_ROOM_SUCCESS: // 방들어가기 성공
                                teve.eve = (int)RECV_EVENT.CHANGE_STAGE;
                                NetMgr.Instance.m_recvQue.Enqueue(teve);
                                break;
                        }
                    }
                    break;
            }
        }

        public override void RecvEvent(t_Eve _eve)
        {
            switch (_eve.eve)
            {
                case (int)RECV_EVENT.CREATE_ROOM:
                    {
                        // 방번호와 이름, 인원수
                        string roomName = null; int roomNum = new int(); int roomCnt = new int();
                        LobbyMgr.Instance.Unpackpacket(_eve.buf, ref roomName, ref roomNum, ref roomCnt);
                        RoomMgr.Instance.CreateRoom(roomName, roomNum, roomCnt);
                    }
                    break;
                case (int)RECV_EVENT.CHAT_FILED:
                    {
                        string msg = null;
                        LobbyMgr.Instance.Unpackpacket(_eve.buf, ref msg);
                        LobbyMgr.Instance.UpdateField(msg);
                        Debug.Log(msg);
                    }
                    break;
                case (int)RECV_EVENT.CHANGE_STAGE:
                    {
                        WindowMgr.Instance.ChangeState(WindowMgr.WINDOW_TYPE.LOBBY, WindowMgr.WINDOW_TYPE.STAGE);
                        NetMgr.Instance.m_curState = NetMgr.Instance.m_stageState;
                        Debug.Log("Stage로 이동...");
                    }
                    break;
            }
        }

        public override void Send(Byte[] _buf, int _protocol)
        {
            switch (_protocol)
            {

            }
        }

        public override void SendEvent(t_Eve _eve)
        {
            // 이벤트 발생시 send
            switch (_eve.eve)
            {

            }
        }
    }
}