#include "pch.h"
#include "CLobbyMgr.h"

#include "CSession.h"
#include "CProtocolMgr.h"
#include "CRoomMgr.h"

CLobbyMgr* CLobbyMgr::m_instance = nullptr;

CLobbyMgr* CLobbyMgr::GetInst()
{
	return m_instance;
}

void CLobbyMgr::Create()
{
	if (m_instance == nullptr)
		m_instance = new CLobbyMgr();
}

void CLobbyMgr::Destroy()
{
	delete m_instance;
}

CLobbyMgr::CLobbyMgr()
{
}

CLobbyMgr::~CLobbyMgr()
{
}

void CLobbyMgr::Init()
{
}

void CLobbyMgr::End()
{
}

void CLobbyMgr::RoomListUpdateProc(CSession* _ptr)
{
	// roomlist��ü�� Ŭ���̾�Ʈ���� �����ش�.
	list<tRoom*>* roomList = CRoomMgr::GetInst()->GetRoomList();

	// test������ �游����
	//CRoomMgr::GetInst()->MakeRoom(_ptr);

	for (tRoom* room : *roomList) // ���� �����ϴ� ����� ������ Ŭ���̾�Ʈ���� ������.
	{
		unsigned long protocol = 0;
		CProtocolMgr::GetInst()->AddSubProtocol(&protocol, (unsigned long)SUB_PROTOCOL::RoomlistResult);
		Packing(protocol, room->roomName, room->roomNum, room->playerList.size(), _ptr);
	}
}

void CLobbyMgr::ChatSendProc(CSession* _ptr)
{
	BYTE buf[BUFSIZE]; ZeroMemory(buf, BUFSIZE);
	TCHAR str[BUFSIZE]; ZeroMemory(str, BUFSIZE * 2);
	_ptr->UnPacking(buf);
	UnPacking(buf, str);

	// msg�� Ŭ���̾�Ʈ�� �г����� ���δ�.
	TCHAR msg[BUFSIZE]; ZeroMemory(msg, BUFSIZE * 2);
	wsprintf(msg, L"[%s��] : %s", _ptr->GetUserInfo()->nickname, str);

	for (CSession* session : m_sessionList) // ���� �κ� �ִ� Ŭ���̾�Ʈ�鿡�� �޽����� ������.
	{
		unsigned long protocol = 0;
		CProtocolMgr::GetInst()->AddSubProtocol(&protocol, (unsigned long)SUB_PROTOCOL::ChatRecv);
		Packing(protocol, msg, session);
	}
}

void CLobbyMgr::Packing(unsigned long _protocol, const TCHAR* _msg, CSession* _ptr)
{
	BYTE buf[BUFSIZE]; ZeroMemory(buf, BUFSIZE);
	BYTE* ptr = buf;
	int size = 0;
	int msg_size = wcslen(_msg) * 2;

	memcpy(ptr, &msg_size, sizeof(int));
	size = size + sizeof(int);
	ptr = ptr + sizeof(int);

	memcpy(ptr, _msg, msg_size);
	size = size + msg_size;

	_ptr->Packing(_protocol, buf, size);
}

void CLobbyMgr::Packing(unsigned long _protocol, const TCHAR* _roomName, const int _roomNum, const int _roomCnt, CSession* _ptr)
{
	BYTE buf[BUFSIZE]; ZeroMemory(buf, BUFSIZE);
	BYTE* ptr = buf;
	int size = 0;
	int roomName_size = wcslen(_roomName) * 2;

	memcpy(ptr, &roomName_size, sizeof(int));
	size += sizeof(int);
	ptr += sizeof(int);

	memcpy(ptr, _roomName, roomName_size);
	size += roomName_size;
	ptr += roomName_size;

	memcpy(ptr, &_roomNum, sizeof(int));
	size += sizeof(int);
	ptr += sizeof(int);

	memcpy(ptr, &_roomCnt, sizeof(int));
	size += sizeof(int);
	ptr += sizeof(int);

	_ptr->Packing(_protocol, buf, size);
}

void CLobbyMgr::UnPacking(const BYTE* _buf, TCHAR* _msg)
{
	const BYTE* ptr = _buf;
	int msg_size = 0;

	memcpy(&msg_size, ptr, sizeof(int));
	ptr += sizeof(int);

	memcpy(_msg, ptr, msg_size);

}

