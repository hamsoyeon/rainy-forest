#include "pch.h"
#include "CIocp.h"
#include "CSessionMgr.h"
#include "CMainMgr.h"
#include "CLogMgr.h"

DWORD CIocp::WorkThread(LPVOID _iocp)
{
	int retval;

	//CIocp* ciocp = (CIocp*)_iocp;
	CIocp* ciocp = static_cast<CIocp*>(_iocp);

	while (1)
	{
		DWORD cbTransferred;
		SOCKET clientsock;
		OVERLAP_EX* overlap_ptr;
		void* session;
		retval = GetQueuedCompletionStatus(ciocp->m_hcp, &cbTransferred, &clientsock, (LPOVERLAPPED*)&overlap_ptr, INFINITE);

		bool check = ciocp->GetQueueErrorCheck(retval, cbTransferred, overlap_ptr);
		// check에서 에러체크후 오륩발생시 type을 disconnected로 바꾸어준다.
		switch (overlap_ptr->type)
		{
		case IO_TYPE::ACCEPT:
			session = ciocp->GetQueueAccept(clientsock, overlap_ptr);
			ciocp->Recv(session);
			break;
		case IO_TYPE::RECV:
			session = overlap_ptr->session;
			ciocp->SizeCheck_And_Recv(session, cbTransferred); // 여기서 함수하나에 전부 처리하도록
			break;
		case IO_TYPE::SEND:
			session = overlap_ptr->session;
			ciocp->SizeCheck_And_Send(session, cbTransferred);
			break;
		case IO_TYPE::DISCONNECT:
			ciocp->DisConnect(overlap_ptr);
			break;
		}
	}

	return 0;
}


void CIocp::Init()
{
	m_hcp = CreateIoCompletionPort(INVALID_HANDLE_VALUE, NULL, 0, 0);
	if (m_hcp == NULL)
		exit(1);

	SYSTEM_INFO si;
	GetSystemInfo(&si);

	HANDLE hThread;
	for (int i = 0; i < (int)si.dwNumberOfProcessors * 2; i++)
	{
		hThread = CreateThread(NULL, 0, WorkThread, this, 0, NULL);
		if (hThread == NULL) exit(1);
		CloseHandle(hThread);
	}
}

void CIocp::End()
{

}

void CIocp::PostDisConnect(void* _ptr) // 일단 사용은 하지않고, 만약 사용한다면 static으로 선언후 오류시 WSAsend에서 사용한다.
{
	OVERLAP_EX* tempoverlap = new OVERLAP_EX();
	tempoverlap->type = IO_TYPE::DISCONNECT;
	tempoverlap->session = _ptr;

	PostQueuedCompletionStatus(m_hcp, -1, NULL, (LPOVERLAPPED)tempoverlap);
}

bool CIocp::GetQueueErrorCheck(int _retval, int _cb_t, OVERLAP_EX* _overlapex)
{
	//CSession* session = (CSession*)_overlapex->session;
	CSession* session = static_cast<CSession*>(_overlapex->session);

	if (_retval == 0 || _cb_t == 0) // 이걸 밖으로 빼고 이 경우에만 GEtQueueErrorcheck를 호출한다.
	{
		if (_retval == 0)
		{
			DWORD temp1, temp2;
			WSAGetOverlappedResult(session->GetSock(), &_overlapex->overlapped, &temp1, FALSE, &temp2);
			CLogMgr::GetInst()->err_display(L"WSAGetOverlappedResult()");
		}
		_overlapex->type = IO_TYPE::DISCONNECT;
		return true;
	}
	return false;
}

