#include "pch.h"
#include "CSession.h"

CSession::CSession()
{
}

CSession::CSession(SOCKET _sock)
	: CPacket(_sock)
{
	m_state = STATE::MENU_SELECT;
	m_userinfo = new t_userinfo();

	ZeroMemory(&r_overlap.overlapped, sizeof(OVERLAPPED));
	r_overlap.type = IO_TYPE::RECV;
	r_overlap.session = this;

	ZeroMemory(&s_overlap.overlapped, sizeof(OVERLAPPED));
	s_overlap.type = IO_TYPE::SEND;
	s_overlap.session = this;
}
