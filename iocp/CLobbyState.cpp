#include "pch.h"
#include "CLobbyState.h"
#include "CRoomMgr.h"
#include "CSession.h"
#include "CLobbyMgr.h"
#include "CProtocolMgr.h"

// multi�� single�� sub protocol�� �ְų� �ϱ�.. �� �������ΰ�..

void CLobbyState::Recv()
{
	int protocol;
	TCHAR buf[BUFSIZE]; ZeroMemory(buf, BUFSIZE);
	m_session->UnPacking(protocol);
	unsigned long sub_protocol = CProtocolMgr::GetInst()->GetSubProtocol(protocol);
	unsigned long detail_protocol = CProtocolMgr::GetInst()->GetDetailProtocol(protocol);

	switch (sub_protocol)
	{
	case (int)CLobbyMgr::SUB_PROTOCOL::LOBBY: // �� ����Ʈ�� ������Ʈ �ش޶�� �޽����� ���� �ش� Ŭ�󿡰� ���� ������.
		switch (detail_protocol)
		{
		case (int)CLobbyMgr::DETAIL_PROTOCOL::MULTI:
			CLobbyMgr::GetInst()->RoomListUpdateProc(m_session);
			break;
		}

	case (int)CLobbyMgr::SUB_PROTOCOL::CHAT: // Ŭ�� ä���� ������...
		switch (detail_protocol)
		{
		case (int)CLobbyMgr::DETAIL_PROTOCOL::ALL_MSG:
			CLobbyMgr::GetInst()->ChatSendProc(m_session);
			break;
		}
		break;
	case (int)CLobbyMgr::SUB_PROTOCOL::ROOM: // Ŭ�󿡰Լ� ���� �����ߴٴ� �̺�Ʈ�� �߻�
		switch (detail_protocol)
		{
		case (int)CLobbyMgr::DETAIL_PROTOCOL::MAKE_ROOM:
			CLobbyMgr::GetInst()->MakeRoomProc(m_session);
			break;
		case (int)CLobbyMgr::DETAIL_PROTOCOL::ENTER_ROOM:
			CLobbyMgr::GetInst()->EnterRoomProc(m_session);
			break;
		}
		break;
	}
}

void CLobbyState::Send()
{

}
