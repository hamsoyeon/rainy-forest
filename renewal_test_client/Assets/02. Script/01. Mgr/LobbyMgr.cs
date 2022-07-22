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
        #region protocol
        public enum SUB_PROTOCOL
        {
            LOBBY,
            CHAT,
            ROOM,

            LOBBY_RESULTL,
            CHAT_RESULT,
            ROOM_RESULT,
        }
        public enum DETAIL_PROTOCOL
        {
            // LOBBY
            MULTI = 1,
            SINGLE = 2,
            ENTER_LOBBY = 4,
            EXIT_LOBBY = 8,

            // CHAT
            ALL_MSG = 1, // 전체 메시지
            NOTICE_MSG = 2, // 공지 메시지

            // ROOM
            ALL_ROOM = 1,
            PAGE_ROOM = 2,
            MAKE_ROOM = 4,
            ROOMLIST_UPDATE = 8,

            ENTER_ROOM = 16,
        }

        // 1,2,4,8,16,32,64,128
        public enum SERVER_DETAIL_PROTOCOL
        {
            // ROOM_RESULT
            ROOMLIST_UPDATE_SUCCESS = 1,
            ROOMLIST_UPDATE_FAIL = 2,
            MAKE_ROOM_SUCCESS = 4,
            MAKE_ROOM_FAIL = 8,

            OVER_PLAYERS = 16,   // 방 인원초과
            ENTER_ROOM_SUCCESS = 32, // 방 들어가기 성공

            // CHAT_RESULT
            ALL_MSG_SUCCESS = 1,
            ALL_MSG_FAIL = 2,
        };
        #endregion

        public enum INPUT_FIELD_TYPE
        {
            CHAT_INPUT_FILED,
        }

        public enum TEXT_TYPE
        {
            CHAT_FILED,
        }

        public enum BTN_TYPE
        {
            ROOM_LEFT,
            ROOM_RIGHT,
            ENTER_MAIN_MENU,
            MAKE_ROOM,
            CHAT,
        }

        [System.Serializable]
        public class Input_Field_Dic : test_client_unity.SerializableDictionary<INPUT_FIELD_TYPE, InputField> { }
        public Input_Field_Dic m_inputfieldDic = new Input_Field_Dic();

        [System.Serializable]
        public class Field_Dic : test_client_unity.SerializableDictionary<TEXT_TYPE, Text> { }
        public Field_Dic m_textDic = new Field_Dic();

        [System.Serializable]
        public class Button_Dic : test_client_unity.SerializableDictionary<BTN_TYPE, Button> { }
        public Button_Dic m_btnDic = new Button_Dic();

        private void Start()
        {
            // 버튼 이벤트
            m_btnDic.TryGetValue(BTN_TYPE.ROOM_LEFT, out Button left_btn);
            left_btn.onClick.AddListener(RoomMgr.Instance.Click_LeftBtn);

            m_btnDic.TryGetValue(BTN_TYPE.ROOM_RIGHT, out Button right_btn);
            right_btn.onClick.AddListener(RoomMgr.Instance.Click_RightBtn);

            m_btnDic.TryGetValue(BTN_TYPE.ENTER_MAIN_MENU, out Button enter_main_menu);

            m_btnDic.TryGetValue(BTN_TYPE.MAKE_ROOM, out Button make_room);
            make_room.onClick.AddListener(Click_MakeRoomBtn);

            m_btnDic.TryGetValue(BTN_TYPE.CHAT, out Button chat);
            chat.onClick.AddListener(Click_SendChatMsg);

            // 인풋필드
        }

        public void Click_MakeRoomBtn()
        {
            t_Eve eve = new t_Eve();

            uint protocol = 0;
            ProtocolMgr.Instance.AddSubProtocol(ref protocol, (uint)LobbyMgr.SUB_PROTOCOL.ROOM);
            ProtocolMgr.Instance.AddDetailProtocol(ref protocol, (uint)LobbyMgr.DETAIL_PROTOCOL.MAKE_ROOM);

            eve.buf_size = NetMgr.Instance.m_netWork.PackPacket(
                ref eve.buf,
                (int)protocol,
                null,
                0);

            NetMgr.Instance.m_sendQue.Enqueue(eve);

            Debug.Log("방 만들기");

            // 이때 방의 이름을 같이 보내주면 될듯.. 흐음..
        }

        public void EnterMainMenu()
        {
            WindowMgr.Instance.ChangeState(WindowMgr.WINDOW_TYPE.LOBBY, WindowMgr.WINDOW_TYPE.MAIN_MENU);

            NetMgr.Instance.m_curState = NetMgr.Instance.m_loginState;

            // 다시 메인메뉴로 간다는 정보를 서버에 보낸다.

            //t_Eve eve = new t_Eve();

            //uint protocol = 0;
            //ProtocolMgr.Instance.AddSubProtocol(ref protocol, (uint)LobbyMgr.SUB_PROTOCOL.LOBBY);
            //ProtocolMgr.Instance.AddDetailProtocol(ref protocol, (uint)LobbyMgr.DETAIL_PROTOCOL.);

            //eve.buf_size = Packpacket(
            //    ref eve.buf,
            //    (int)protocol,
            //    chat_input_filed.text
            //    );

            //NetMgr.Instance.m_sendQue.Enqueue(eve);
        }

        //public GameObject m_field;
        int m_field_cnt = 0;

        public void UpdateField(string _msg) // 채팅창에 문자를 추가한다.
        {
            m_textDic.TryGetValue(TEXT_TYPE.CHAT_FILED, out Text chat_filed);

            // 12줄이 되었을때, 높이를 더해준다.
            chat_filed.GetComponent<Text>().text += _msg + "\r\n";
            m_field_cnt++;
            if (m_field_cnt >= 12)
            {
                chat_filed.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(
                    RectTransform.Axis.Vertical,
                    chat_filed.GetComponent<RectTransform>().rect.height + 20);
            }
        }

        public void Click_SendChatMsg() // 채팅메시지 보내기 버튼
        {
            m_inputfieldDic.TryGetValue(INPUT_FIELD_TYPE.CHAT_INPUT_FILED, out InputField chat_input_filed);

            // 아무것도 입력을 안한 경우
            if (chat_input_filed.text == "")
            {
                return;
            }

            t_Eve eve = new t_Eve();

            uint protocol = 0;
            ProtocolMgr.Instance.AddSubProtocol(ref protocol, (uint)LobbyMgr.SUB_PROTOCOL.CHAT);
            ProtocolMgr.Instance.AddDetailProtocol(ref protocol, (uint)LobbyMgr.DETAIL_PROTOCOL.ALL_MSG);

            eve.buf_size = Packpacket(
                ref eve.buf,
                (int)protocol,
                chat_input_filed.text
                );

            NetMgr.Instance.m_sendQue.Enqueue(eve);

            Debug.Log("채팅 보내기");
        }

        #region packing, unpacking
        public int Packpacket(ref Byte[] _buf, int _protocol, string _msg)
        {
            _buf = new Byte[4096];

            Byte[] data_buf = new Byte[4096];

            int msg_size = _msg.Length * 2;
            int len = 0;

            BitConverter.GetBytes(msg_size).CopyTo(data_buf, len);
            len = len + sizeof(int);

            Encoding.Unicode.GetBytes(_msg).CopyTo(data_buf, len);
            len = len + msg_size;

            return NetMgr.Instance.m_netWork.PackPacket(ref _buf, _protocol, data_buf, len);
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
        #endregion

    }
}


