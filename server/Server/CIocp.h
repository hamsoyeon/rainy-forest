#pragma once
class CIocp
{
public:
	virtual void Init();
	virtual void End();

	static DWORD WorkTrd(LPVOID _iocp);
	virtual void Disconnect(void* _session) = 0;

	virtual void sizecheck_and_recv(void* _session, int _completebyte) = 0;
	virtual void sizecheck_and_send(void* _session, int _completebyte) = 0;

	void PostQueueAccept(SOCKET _sock);

private:
	HANDLE m_hcp;

};

