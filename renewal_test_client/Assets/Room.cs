using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace test_client_unity
{
    // ���̸�
    // ���ȣ
    // ���ο���

    public class Room : MonoBehaviour
    {
        public string m_roomName { get; set; }
        public int m_roomNum { get; set; }
        public int m_roomCnt { get; set; }

        public void SetRoom(string _roomName, int _roomNum, int _roomCnt = 0)
        {
            // ������
            m_roomName = _roomName; m_roomNum = _roomNum; m_roomCnt = m_roomCnt; 

            Transform[] allchildren = this.GetComponentsInChildren<Transform>();

            // ���� obj�� child�� Ž��
            // - ��ư
            // - ��ѹ�
            // - ���� ���ο���
            foreach (Transform child in allchildren)
            {
                switch (child.gameObject.name)
                {
                    case "enter room button": // ���̸�
                        child.GetComponent<Button>().onClick.AddListener(Click_EnterStageBtn);
                        child.GetComponentInChildren<Text>().text = _roomName;
                        break;
                    case "room num text": // ��ѹ�
                        child.GetComponent<Text>().text = _roomNum.ToString();
                        break;
                    case "room cnt text": // ���ο���
                        child.GetComponent<Text>().text = _roomCnt.ToString() + " / 3";
                        break;
                }
            }
        }

        public void Click_EnterStageBtn() // �����
        {
            //WindowMgr.Instance.ChangeState(WindowMgr.WINDOW_TYPE.LOBBY, WindowMgr.WINDOW_TYPE.STAGE);

            // send�� t_Eve�� �ƴ϶�.. �׳�... sendbuf��� ����ü�� ���� ���� ����ϴ� �� ���� �� ����!
            // ���� ���� number�� ���� ������.
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

            Debug.Log("�� ����");
        }

        // �̰� �Ŵ������� �ΰ�... ����
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

