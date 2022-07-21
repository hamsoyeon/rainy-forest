using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace test_client_unity
{
    public class LobbyMgr : Singleton<LobbyMgr>
    {
        public enum BUTTON_EVENT
        {
            LEFT,
            RIGHT,
        }

        [System.Serializable]
        public class SerializeStateDic : test_client_unity.SerializableDictionary<BUTTON_EVENT, Button> { }
        public SerializeStateDic m_btnDic = new SerializeStateDic();
        private void Start()
        {
            // 버튼 이벤트 추기
            m_btnDic.TryGetValue(BUTTON_EVENT.LEFT, out Button left_btn);
            left_btn.onClick.AddListener(RoomMgr.Instance.OnLeftBtn);
            m_btnDic.TryGetValue(BUTTON_EVENT.RIGHT, out Button right_btn);
            right_btn.onClick.AddListener(RoomMgr.Instance.OnRightBtn);
        }

        public enum SUB_PROTOCOL
        {
            NONE = -1,
            LOBBY_RESULT,
            CREATE_ROOM,
            CREATE_ROOM_RESULT,
            CHAT_SEND,
            CHAT_RECV,
            ROOMLIST_UPDATE, // 클라가 방 리스트 보내줘하면 서버가 보내줌
            ROOMLIST_RESULT, // 클라한테 방리스트를 보낼게
            MAX,
        }

        public enum DETAIL_PROTOCOL
        {
            NONE = -1,
            MULTI = 1,
            SINGLE,
            NOTICE_MSG,
            ALL_MSG,         // 공지 메시지
            ALL_ROOM,
            PAGE_ROOM,
            MAX		        // 전체 메시지
        }

        public void EnterMainMenu()
        {
            StageMgr.Instance.ChangeState(StageMgr.STAGE.LOBBY, StageMgr.STAGE.MAIN_MENU);

            NetMgr.Instance.Cur_State = StateMgr.Instance.GetLogin;

            // 다시 메인메뉴로 간다는 정보를 서버에 보낸다.

        }

        public InputField m_input_text;
        public GameObject m_field;
        int m_field_cnt = 0;

        public void UpdateField(string _msg) // 채팅창에 문자를 추가한다.
        {
            // 12줄이 되었을때, 높이를 더해준다.
            m_field.GetComponent<Text>().text += _msg + "\r\n";
            m_field_cnt++;
            if (m_field_cnt >= 12)
            {               
                m_field.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(
                    RectTransform.Axis.Vertical, 
                    m_field.GetComponent<RectTransform>().rect.height + 20);
            }
        }

        public void OnSendChatMsg() // 채팅메시지 보내기 버튼
        {
            // 아무것도 입력을 안한 경우
            if (m_input_text.text == "")
            {
                return;
            }

            uint protocol = 0;
            ProtocolMgr.Instance.AddSubProtocol(ref protocol, (uint)LobbyMgr.SUB_PROTOCOL.CHAT_SEND);
            ProtocolMgr.Instance.AddDetailProtocol(ref protocol, (uint)LobbyMgr.DETAIL_PROTOCOL.ALL_MSG);

            Byte[] buf = new Byte[4096]; // 방을 달라는 패킷을 보낸다.
            int size = Packpacket(
                ref buf,
                (int)protocol,
                m_input_text.text
                );

            NetMgr.Instance.Getnetwork.Send(buf, size);

            Debug.Log("채팅 보내기");
        }

        public int Packpacket(ref Byte[] _buf, int _protocol, string _msg)
        {
            int len = 0;
            // 프로토콜 / 전체 데이터 사이즈 / msg 데이터 사이즈 / msg 데이터
            int msg_size = _msg.Length * 2;
            int size = sizeof(int) + sizeof(int) + sizeof(int) + msg_size;

            Byte[] tmp_byte = BitConverter.GetBytes(size);
            tmp_byte.CopyTo(_buf, len);
            len = len + sizeof(int);

            tmp_byte = BitConverter.GetBytes(_protocol);
            tmp_byte.CopyTo(_buf, len);
            len = len + sizeof(int);

            tmp_byte = BitConverter.GetBytes(size);
            tmp_byte.CopyTo(_buf, len);
            len = len + sizeof(int);

            tmp_byte = BitConverter.GetBytes(msg_size);
            tmp_byte.CopyTo(_buf, len);
            len = len + sizeof(int);

            tmp_byte = Encoding.Unicode.GetBytes(_msg);
            tmp_byte.CopyTo(_buf, len);
            len = len + msg_size;

            return len;
        }
        public void Unpackpacket(Byte[] _buf, ref string _msg)
        {
            int len = sizeof(int) + sizeof(int) + sizeof(int); // 전체 데이터 사이즈 / 패킷넘버 / 데이터 사이즈
            Byte[] size = new byte[4];

            Array.Copy(_buf, len, size, 0, sizeof(int));
            len = len + sizeof(int);

            _msg = Encoding.Unicode.GetString(_buf, len, BitConverter.ToInt32(size));
        }
        public void Unpackpacket(Byte[] _buf, ref string _roomName, ref int _roomNum, ref int _roomCnt)
        {
            int len = sizeof(int) + sizeof(int) + sizeof(int);
            Byte[] roomName_size = new Byte[4];
            Byte[] roomNum = new Byte[4];
            Byte[] roomCnt = new Byte[4];

            Array.Copy(_buf, len, roomName_size, 0, sizeof(int));
            len = len + sizeof(int);

            _roomName = Encoding.Unicode.GetString(_buf, len, BitConverter.ToInt32(roomName_size));
            len = len + BitConverter.ToInt32(roomName_size);

            Array.Copy(_buf, len, roomNum, 0, sizeof(int));
            _roomNum = BitConverter.ToInt32(roomNum);
            len = len + sizeof(int);

            Array.Copy(_buf, len, roomCnt, 0, sizeof(int));
            _roomCnt = BitConverter.ToInt32(roomCnt);
        }
    }
}


