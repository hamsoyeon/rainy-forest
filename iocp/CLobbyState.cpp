#include "pch.h"
#include "CLobbyState.h"
#include "CRoomMgr.h"
#include "CSession.h"
#include "CLobbyMgr.h"
#include "CProtocolMgr.h"

// multi와 single을 sub protocol에 넣거나 하기.. 뭐 이정도인가..

void CLobbyState::Recv()
{
	int protocol;
	TCHAR buf[BUFSIZE]; ZeroMemory(buf, BUFSIZE);
	m_session->UnPacking(protocol);
	unsigned long sub_protocol = CProtocolMgr::GetInst()->GetSubProtocol(protocol);
	unsigned long detail_protocol = CProtocolMgr::GetInst()->GetDetailProtocol(protocol);

	switch (sub_protocol)
	{
	case (int)CLobbyMgr::SUB_PROTOCOL::LOBBY: // 방 리스트를 업데이트 해달라는 메시지가 오면 해당 클라에게 방을 보낸다.
		switch (detail_protocol)
		{
		case (int)CLobbyMgr::DETAIL_PROTOCOL::MULTI:
			CLobbyMgr::GetInst()->RoomListUpdateProc(m_session);
			break;
		}

	case (int)CLobbyMgr::SUB_PROTOCOL::CHAT: // 클라가 채팅을 보내면...
		switch (detail_protocol)
		{
		case (int)CLobbyMgr::DETAIL_PROTOCOL::ALL_MSG:
			CLobbyMgr::GetInst()->ChatSendProc(m_session);
			break;
		}
		break;
	case (int)CLobbyMgr::SUB_PROTOCOL::ROOM: // 클라에게서 방을 생성했다는 이벤트가 발생
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
