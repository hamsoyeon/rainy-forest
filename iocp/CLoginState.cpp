#include "pch.h"
#include "CLoginState.h"
#include "CSession.h"
#include "CLoginMgr.h"
#include "CProtocolMgr.h"

void CLoginState::Recv()
{
	int protocol;
	TCHAR buf[BUFSIZE]; ZeroMemory(buf, BUFSIZE);
	m_session->UnPacking(protocol);
	// test용
	int sub_protocol = CProtocolMgr::GetInst()->GetSubProtocol(protocol);
	//현재 받아온게 메뉴정보인지 로그인정보인지 회원가입정보인지 구분
	switch (sub_protocol)
	{
	case (int)CLoginMgr::SUB_PROTOCOL::LoginInfo:
		CLoginMgr::GetInst()->LoginProc(m_session);
		break;
	case (int)CLoginMgr::SUB_PROTOCOL::JoinInfo:
		CLoginMgr::GetInst()->JoinProc(m_session);
		break;
	case (int)CLoginMgr::SUB_PROTOCOL::LogoutInfo:
		CLoginMgr::GetInst()->LogOutProc(m_session);
		break;
	case (int)CLoginMgr::SUB_PROTOCOL::LobbyEnter:
		CLoginMgr::GetInst()->EnterLobbyProc(m_session);
		//is_lobby = true;
		break;
	}
}

void CLoginState::Send()
{
	//로그인중인지 체크
	//if (is_lobby)
	//	m_session->SetState(m_session->GetLobbyState());
}
