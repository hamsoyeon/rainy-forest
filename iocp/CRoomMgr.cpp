#include "pch.h"
#include "CRoomMgr.h"
#include "CRoom.h"
#include "CSession.h"

CRoomMgr* CRoomMgr::m_instance = nullptr;

CRoomMgr* CRoomMgr::GetInst()
{
	return m_instance;
}

void CRoomMgr::Create()
{
	if (m_instance == nullptr)
		m_instance = new CRoomMgr();
}

void CRoomMgr::Destroy()
{
	delete m_instance;
}

CRoomMgr::CRoomMgr()
{
}

CRoomMgr::~CRoomMgr()
{
}

void CRoomMgr::Init()
{
	m_roomList.clear();
	m_roomNum = 0;

	//for (CRoom* room : m_roomList)
	//{
	//    room->GetRoomName();
	//}
}

void CRoomMgr::End()
{
}

//void CRoomMgr::EnterRoomProc(CSession* _ptr)
//{
//
//}

//void CRoomMgr::ExitRoomProc(CSession* _ptr)
//{
//
//}

void CRoomMgr::MakeRoom(TCHAR* _roomName, CSession* _ptr)
{
	// ���� ����� �ش� �뿡 Ŭ���̾�Ʈ�� �־��ְ�
	// - ���� �̸�, 
	// - ���ȣ(������ Ŭ�󿡰� �ִ� ��),
	// - �濡 �� Ŭ���̾�Ʈ

	//BYTE buf[BUFSIZE]; ZeroMemory(buf, BUFSIZE);
	//_ptr->UnPacking(buf);

	// ���ȣ�� ���� ptr�� ��� �濡 �ִ��� �ĺ��� ��
	// ���ȣ�� 

	tRoom* room = new tRoom(_roomName, m_roomNum++, _ptr);

	//room->playerList.push_back(_ptr); // �ش�濡 ����.
	m_roomList.push_back(room);
}

void CRoomMgr::EnterRoom(CSession* _ptr, int _num)
{
	// roomList���� �÷��̾ ������ num�� �ش��ϴ� �濡 ����.
	for (tRoom* room : m_roomList)
	{
		if (room->roomNum == _num)
		{
			room->playerList.push_back(_ptr); break;
		}
	}
}






