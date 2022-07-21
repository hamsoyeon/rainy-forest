#pragma once
#include "CSession.h"
// recv, send, accept, disconnected
class CIocp
{
public:
	virtual void Init();
	virtual void End();

	static DWORD WorkThread(LPVOID _iocp);
	virtual BOOL Send(void* _session) = 0;
	virtual BOOL Recv(void* _session) = 0;
	virtual int DisConnect(OVERLAP_EX* _overlap) = 0;
	virtual void* GetQueueAccept(ULONG_PTR _com_key, OVERLAP_EX* _overlap) = 0;
	virtual void PostQueueAccept(SOCKET _clientsock) = 0;
	virtual void SizeCheck_And_Recv(void* _session, int _combytes) = 0;
	virtual void SizeCheck_And_Send(void* _session, int _combytes) = 0;
	void PostDisConnect(void* _ptr);
	virtual bool GetQueueErrorCheck(int _retval, int _cb_t, OVERLAP_EX* _overlapex); // iocp에서만 쓰는 함수

protected:
	HANDLE m_hcp;
};

