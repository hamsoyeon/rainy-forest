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
            Transform[] allchildren = this.GetComponentsInChildren<Transform>();

            foreach (Transform child in allchildren)
            {
                switch (child.gameObject.name)
                {
                    case "enter room button": // ���̸�
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
    }
}

