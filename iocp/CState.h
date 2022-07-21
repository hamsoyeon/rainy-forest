#pragma once

class CSession;

class CState
{
public:
	CState(CSession* _session) { m_session = _session; }
	virtual void Recv() = 0;
	virtual void Send() = 0;

protected:
	CSession* m_session;

};

