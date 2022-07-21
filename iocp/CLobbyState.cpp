#include "pch.h"
#include "CLobbyState.h"
#include "CRoomMgr.h"
#include "CSession.h"
#include "CLobbyMgr.h"
#include "CProtocolMgr.h"

void CLobbyState::Recv()
{
	int protocol;
	TCHAR buf[BUFSIZE]; ZeroMemory(buf, BUFSIZE);
	m_session->UnPacking(protocol);
	unsigned long sub_protocol = CProtocolMgr::GetInst()->GetSubProtocol(protocol);
	unsigned long detail_protocol = CProtocolMgr::GetInst()->GetDetailProtocol(protocol);

	switch (sub_protocol)
	{
	case (int)CLobbyMgr::SUB_PROTOCOL::RoomlistUpdate: // �� ����Ʈ�� ������Ʈ �ش޶�� �޽����� ���� �ش� Ŭ�󿡰� ���� ������.
		switch (detail_protocol)
		{
		case (int)CLobbyMgr::DETAIL_PROTOCOL::Multi:
			CLobbyMgr::GetInst()->RoomListUpdateProc(m_session);
			break;
		}
		break;
	case (int)CLobbyMgr::SUB_PROTOCOL::ChatSend: // Ŭ�� ä���� ������...
		switch (detail_protocol)
		{
		case (int)CLobbyMgr::DETAIL_PROTOCOL::AllMsg:
			CLobbyMgr::GetInst()->ChatSendProc(m_session);
			break;
		}
		break;
	case (int)CLobbyMgr::SUB_PROTOCOL::CreateRoom: // Ŭ�󿡰Լ� ���� �����ߴٴ� �̺�Ʈ�� �߻�
		break;
	}
}

void CLobbyState::Send()
{

}
