using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace test_client_unity
{
    public class RoomMgr : Singleton<RoomMgr>
    {
        public List<GameObject> m_roomList = new List<GameObject>();
        public int m_curPageNum = new int();

        void Start()
        {
            m_curPageNum = 0;

            CreateRoom("test", 1, 5);
            CreateRoom("test", 2, 5);
            CreateRoom("test", 3, 5);
            CreateRoom("test", 4, 5);
            CreateRoom("test", 5, 5);
            CreateRoom("test", 6, 5);
            CreateRoom("test", 7, 5);
            CreateRoom("test", 8, 5);
            CreateRoom("test", 9, 5);

            CreateRoom("test", 10, 5);
            CreateRoom("test", 11, 5);

            UpdatePage();
        }

        public void OnLeftBtn()
        {
            if (m_curPageNum == 0)
            {
                return;
            }

            m_curPageNum--;

            Debug.Log(m_curPageNum);
        }

        public void OnRightBtn()
        {
            if (m_curPageNum == m_roomList.Count / 9)
            {
                return;
            }

            m_curPageNum++;

            Debug.Log(m_curPageNum);
        }

        public void UpdatePage() // 현재 페이지 
        {
            for (int i = 0; i < m_roomList.Count / 9; i++)
            {
                GameObject page_obj = GameObject.Find("page_" + m_roomList.Count / 9);
                if (!page_obj.name.Contains(m_curPageNum.ToString()))
                {
                    page_obj.SetActive(false);
                }
            }
        }

        public void CreateRoom(string _roomName, int _roomNum, int _roomCnt = 0) // 현재 존재하는 모든 방 생성
        {
            // unity error : Setting the parent of a transform which resides in a Prefab Asset is disabled to prevent data corruption
            // 코드 상에서 프리팹을 불러오고 parent를 수정시, 인스턴스화된 obj를 변수로 받아서 세팅한다.
            GameObject room = Resources.Load("Prefab/room") as GameObject;
            GameObject room_inst = PrefabUtility.InstantiatePrefab(room) as GameObject;

            room_inst.transform.GetComponent<Room>().SetRoom(_roomName, _roomNum, _roomCnt);

            GameObject page_obj;
            page_obj = GameObject.Find("page_" + m_roomList.Count / 9);

            if (page_obj == null) // page_ obj가 없다면 생성
            {
                page_obj = Resources.Load("Prefab/page") as GameObject;
                GameObject page_inst = PrefabUtility.InstantiatePrefab(page_obj) as GameObject;

                // 페이지 생성
                int page_num = m_roomList.Count / 9; // 페이지 넘버
                page_inst.transform.parent = GameObject.FindWithTag("Room Panel").transform;
                page_inst.name = "page_" + page_num.ToString();
                page_inst.transform.localPosition = new Vector3(0f, 0f, 0f);
                Instantiate(page_inst, page_inst.transform.localPosition, Quaternion.identity);

                // 해당 페이지에 방을 넣는다.
                room_inst.transform.parent = page_inst.transform;

            }
            else
            {
                // Lobby window 오브젝트를 찾아서 하위에 현재 생성할 room 오브젝트를 붙인다. 
                room_inst.transform.parent = page_obj.transform;
            }

            Instantiate(room_inst, room_inst.transform.position, Quaternion.identity);

            m_roomList.Add(room_inst);

        }
    }
}

