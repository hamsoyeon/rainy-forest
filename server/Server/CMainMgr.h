#pragma once

#include "CIocp.h"

class CSocket;

class CMainMgr :public CIocp
{
	SINGLE(CMainMgr)
public:
	virtual void Init() final;
	virtual void End() final;

	virtual void Disconnect(void* _session) final;

	void RecvProc(void* _session);
	void SendProc(void* _session);

	virtual void sizecheck_and_recv(void* _session, int _completebyte) final;
	virtual void sizecheck_and_send(void* _session, int _completebyte) final;

	void Run();
	void Loop();

private:
	CSocket* listen_sock;

};

// 
// 
// 
// 
