#include "pch.h"
#include "CSessionMgr.h"
#include "CSession.h"
#include "CLock.h"
#include "CLockGuard.h"

#include "CLobbyMgr.h"

CSessionMgr* CSessionMgr::m_instance = nullptr;

void CSessionMgr::Init()
{
	m_lock = new CLock;
}

void CSessionMgr::End()
{
	for (CSession* item : m_session_list)
	{
		printf("종료 %s %d\n", inet_ntoa(item->GetAddr().sin_addr), ntohs(item->GetAddr().sin_port));
		delete item;
	}
	m_session_list.clear();
	delete m_lock;
}

CSessionMgr* CSessionMgr::GetInst()
{
	return m_instance;
}

void CSessionMgr::Create()
{
	if (m_instance == nullptr)
		m_instance = new CSessionMgr();
}

void CSessionMgr::Destroy()
{
	delete m_instance;
}

CSessionMgr::CSessionMgr()
{

}

CSessionMgr::~CSessionMgr()
{

}

CSession* CSessionMgr::AddSession(SOCKET _sock)
{
	CLock_Guard<CLock> lock(m_lock);

	CSession* session = new CSession(_sock);

	session->Init();

	m_session_list.push_back(session);

	return session;
}

void CSessionMgr::RemoveSession(CSession* _ptr)
{
	CLock_Guard<CLock> lock(m_lock);

	CSession* item;
	for (CSession* session : m_session_list)
	{
		if (!memcmp(session, _ptr, sizeof(CSession)))
		{
			item = session;
			break;
		}
	}

	CLobbyMgr::GetInst()->RemoveSessionList(item); // lobby에 있다면 나간다.

	printf("종료 %s %d\n", inet_ntoa(item->GetAddr().sin_addr), ntohs(item->GetAddr().sin_port));
	m_session_list.remove(item);
	delete item;
}
