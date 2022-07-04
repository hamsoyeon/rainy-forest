#include "pch.h"
#include "CIocp.h"

#include "CSession.h"

#include "CSessionMgr.h"

void CIocp::Init()
{
	m_hcp = CreateIoCompletionPort(INVALID_HANDLE_VALUE, NULL, 0, 0);
	if (m_hcp == NULL)
		exit(1);

	SYSTEM_INFO si;
	GetSystemInfo(&si);

	HANDLE hTrd;
	for (int i = 0; i < (int)si.dwNumberOfProcessors * 2; i++)
	{
		hTrd = CreateThread(NULL, 0, WorkTrd, this, 0, NULL);
		if (hTrd == NULL) exit(1);
		CloseHandle(hTrd);
	}
}

void CIocp::End()
{
}

DWORD CIocp::WorkTrd(LPVOID _iocp)
{
	int retval;

	CIocp* iocp = (CIocp*)_iocp;

	while (1)
	{
		DWORD cbTransferred;
		SOCKET client_sock;
		OVERLAP_EX* overlap_ptr;

		retval = GetQueuedCompletionStatus(iocp->m_hcp, &cbTransferred, &client_sock, (LPOVERLAPPED*)&overlap_ptr, INFINITE);

		CSession* session = (CSession*)overlap_ptr->session;

		if (retval == 0 || cbTransferred == 0)
		{
			if (retval == 0)
			{
				DWORD temp1, temp2;
				WSAGetOverlappedResult(session->GetSock(), &overlap_ptr->overlapped, &temp1, FALSE, &temp2);
				err_display(L"WSAGetOverlappedResult()");
			}
			overlap_ptr->type = IO_TYPE::DISCONNECT;
		}

		switch (overlap_ptr->type)
		{
		case IO_TYPE::ACCEPT:
			iocp->PostQueueAccept(session->GetSock());
			session->WSARECV();
			break;
		case IO_TYPE::RECV:
			iocp->sizecheck_and_recv(session, cbTransferred);
			break;
		case IO_TYPE::SEND:
			iocp->sizecheck_and_send(session, cbTransferred);
			break;
		case IO_TYPE::DISCONNECT:
			iocp->Disconnect(session);
			break;
		}
	}

	return 0;
}

void CIocp::PostQueueAccept(SOCKET _sock)
{
	/// <summary>
	///  Accept 요청이 들어오면 session을 만들어준다.
	/// </summary>
	/// <param name="_sock"></param>
	OVERLAP_EX* overlap = new OVERLAP_EX();
	overlap->type = IO_TYPE::ACCEPT;
	PostQueuedCompletionStatus(m_hcp, -1, _sock, (LPOVERLAPPED)overlap);
	CSessionMgr::GetInst()->AddSession(_sock);
}

