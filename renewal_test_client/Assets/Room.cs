using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace test_client_unity
{
    // 방이름
    // 방번호
    // 방인원수

    public class Room : MonoBehaviour
    {
        public string m_roomName { get; set; }
        public int m_roomNum { get; set; }
        public int m_roomCnt { get; set; }

        public void SetRoom(string _roomName, int _roomNum, int _roomCnt = 0)
        {
            // 값세팅
            m_roomName = _roomName; m_roomNum = _roomNum; m_roomCnt = m_roomCnt; 

            Transform[] allchildren = this.GetComponentsInChildren<Transform>();

            // 현재 obj의 child를 탐색
            // - 버튼
            // - 방넘버
            // - 현재 방인원수
            foreach (Transform child in allchildren)
            {
                switch (child.gameObject.name)
                {
                    case "enter room button": // 방이름
                        child.GetComponent<Button>().onClick.AddListener(Click_EnterStageBtn);
                        child.GetComponentInChildren<Text>().text = _roomName;
                        break;
                    case "room num text": // 방넘버
                        child.GetComponent<Text>().text = _roomNum.ToString();
                        break;
                    case "room cnt text": // 방인원수
                        child.GetComponent<Text>().text = _roomCnt.ToString() + " / 3";
                        break;
                }
            }
        }

        public void Click_EnterStageBtn() // 방들어가기
        {
            //WindowMgr.Instance.ChangeState(WindowMgr.WINDOW_TYPE.LOBBY, WindowMgr.WINDOW_TYPE.STAGE);

            // send는 t_Eve가 아니라.. 그냥... sendbuf라는 구조체를 따로 만들어서 사용하는 게 나을 것 같다!
            // 누른 방의 number도 같이 보낸다.
            // buf data
            // - room number
            t_Eve eve = new t_Eve();

            uint protocol = 0;
            ProtocolMgr.Instance.AddSubProtocol(ref protocol, (uint)LobbyMgr.SUB_PROTOCOL.ROOM);
            ProtocolMgr.Instance.AddDetailProtocol(ref protocol, (uint)LobbyMgr.DETAIL_PROTOCOL.ENTER_ROOM);

            eve.buf_size = Packpacket(
                ref eve.buf,
                (int)protocol,
                m_roomNum);

            NetMgr.Instance.m_sendQue.Enqueue(eve);

            Debug.Log("방 들어가기");
        }

        // 이걸 매니저에다 두고... 흐음
        public int Packpacket(ref Byte[] _buf, int _protocol, int _data)
        {
            _buf = new Byte[4096];

            Byte[] data_buf = new Byte[4096];

            int len = 0;

            BitConverter.GetBytes(_data).CopyTo(data_buf, len);
            len = len + sizeof(int);

            return NetMgr.Instance.m_netWork.PackPacket(ref _buf, _protocol, data_buf, len);
        }
    }
}

