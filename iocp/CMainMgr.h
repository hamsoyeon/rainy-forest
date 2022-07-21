#pragma once
#include "CIocp.h"

class CSocket;

class CMainMgr :
	public CIocp
{
public:
	static CMainMgr* GetInst();
	static void Create();
	static void Destroy();

private:
	static CMainMgr* m_instance;

	CMainMgr();
	~CMainMgr();

private:
	//SOC RecvSizeCheck(int _cb_t, CSession* _ptr);
	//SOC SendSizeCheck(int _cb_t, CSession* _ptr);

public:
	void Loop();
	void Run();
	virtual void Init();
	virtual void End();

	virtual BOOL Send(void* _session)final;
	virtual BOOL Recv(void* _session)final;
	virtual int DisConnect(OVERLAP_EX* _overlap)final;
	virtual void* GetQueueAccept(ULONG_PTR _com_key, OVERLAP_EX* _overlap)final;
	void PostQueueAccept(SOCKET _clientsock);
	void PostDisConnect(CSession* _ptr);
	//virtual bool GetQueueErrorCheck(int _retval, int _cb_t, OVERLAP_EX* _overlapex)final;
	virtual void SizeCheck_And_Recv(void* _session, int _combytes)final;
	virtual void SizeCheck_And_Send(void* _session, int _combytes)final;

private:
	CSocket*	listen_sock;

};

