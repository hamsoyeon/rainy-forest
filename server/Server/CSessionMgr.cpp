#include "pch.h"
#include "CSessionMgr.h"

#include "CSession.h"

CSessionMgr* CSessionMgr::m_instance = nullptr;

CSessionMgr::CSessionMgr()
{
}

CSessionMgr::~CSessionMgr()
{
}

void CSessionMgr::Create()
{
	if (nullptr == m_instance)
		m_instance = new CSessionMgr;
}

void CSessionMgr::Destroy()
{
	if (nullptr != m_instance)
		delete m_instance;
}

void CSessionMgr::AddSession(SOCKET _sock)
{
	CSession* session = new CSession(_sock);
	printf("\n[TCP ����] session ����: IP �ּ�=%s, ��Ʈ ��ȣ=%d\n",
		inet_ntoa(session->GetAddr().sin_addr), ntohs(session->GetAddr().sin_port));
	session_list.push_back(session);
}

void CSessionMgr::RemoveSession(CSession* _session)
{
	CSession* session;
	for (CSession* item : session_list)
	{
		if (!memcmp(item, _session, sizeof(CSession)))
		{
			session = item;
			break;
		}
	}
	printf("[TCP ����] session ����: IP �ּ�=%s, ��Ʈ ��ȣ=%d\n",
		inet_ntoa(session->GetAddr().sin_addr), ntohs(session->GetAddr().sin_port));
	session_list.remove(session);
	delete session;
}


