#include "pch.h"
#include "CMainMgr.h"

#include "CSocket.h"
#include "CSession.h"

#include "CSessionMgr.h"

CMainMgr* CMainMgr::m_instance = nullptr;

CMainMgr::CMainMgr()
{
}

CMainMgr::~CMainMgr()
{
}

void CMainMgr::Create()
{
	if (nullptr == m_instance)
		m_instance = new CMainMgr;

	CSessionMgr::Create();
}

void CMainMgr::Destroy()
{
	if (nullptr != m_instance)
		delete m_instance;
}

void CMainMgr::Init()
{
	WSADATA ws;
	if (WSAStartup(MAKEWORD(2, 2), &ws) != 0)
		exit(1);

	listen_sock = new CSocket("", 9000);

	CIocp::Init();
}

void CMainMgr::End()
{
	CIocp::End();
}

void CMainMgr::Disconnect(void* _session)
{
	/// <summary>
	/// session 나감 처리
	/// </summary>
	/// <param name="_session"></param>

	CSession* session = (CSession*)_session;

}

void CMainMgr::RecvProc(void* _session)
{
}

void CMainMgr::SendProc(void* _session)
{
	CSession* session = (CSession*)_session;

	switch (session->GetState())
	{
	case STATE::LOGIN:
		break;
	}
}

void CMainMgr::sizecheck_and_recv(void* _session, int _completebyte)
{
	CSession* session = (CSession*)_session;

	SOC soc = session->Complate_Recv(_completebyte);

	switch (soc)
	{
	case SOC::SOC_TRUE:
		RecvProc(session);
		break;
	case SOC::SOC_FALSE:
		break;
	}

	session->WSARECV();
}

void CMainMgr::sizecheck_and_send(void* _session, int _completebyte)
{
	CSession* session = (CSession*)_session;

	SOC soc = session->Complate_Send(_completebyte);

	switch (soc)
	{
	case SOC::SOC_TRUE:
		SendProc(session);
		session->DelaySend();
		break;
	case SOC::SOC_FALSE:
		session->WSASEND();
		break;
	}
}

void CMainMgr::Run()
{
	while (1)
	{
		SOCKET client_sock;
		client_sock = listen_sock->Accept();
		if (client_sock == INVALID_SOCKET)
		{
			err_display(L"accept()");
			return;
		}

		PostQueueAccept(client_sock);
	}
}

void CMainMgr::Loop()
{
	Init();
	Run();
	End();
}
