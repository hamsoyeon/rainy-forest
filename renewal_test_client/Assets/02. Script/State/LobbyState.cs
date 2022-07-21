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
        }

        public override void Recv(Byte[] _buf, uint _protocol)
        {
            int result = new int();
            string msg = null;

            t_Eve teve = new t_Eve();
            teve.buf = new Byte[4096];
            Array.Copy(_buf, teve.buf, _buf.Length);

            switch (_protocol) // 프로토콜에 따라서 언패킹
            {
                case (int)LobbyMgr.SUB_PROTOCOL.ROOMLIST_RESULT:
                    {
                        teve.eve = (int)RECV_EVENT.CREATE_ROOM;
                        NetMgr.Instance.m_recvQue.Enqueue(teve);
                    }
                    break;
                case (int)LobbyMgr.SUB_PROTOCOL.CHAT_RECV: // 메시지를 받으면 채팅창에 올라간다.
                    {
                        teve.eve = (int)RECV_EVENT.CHAT_FILED;
                        NetMgr.Instance.m_recvQue.Enqueue(teve);
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
                        //RoomMgr.Instance.CreateRoom(roomName, roomNum, roomCnt);
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
            // 이벤트 발생시 send
            switch (_eve.eve)
            {

            }
        }
    }
}