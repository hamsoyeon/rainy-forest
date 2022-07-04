#pragma once

class Session;

class CSessionMgr
{
	SINGLE(CSessionMgr)
public:
	void AddSession(SOCKET _sock);
	void RemoveSession(CSession* _ptr);

private:
	list<CSession*> session_list;

};

