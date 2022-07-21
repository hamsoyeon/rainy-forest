#pragma once

class CSession;
class CLock;

#include "CSession.h"

class CLoginMgr
{
public:
	enum class SUB_PROTOCOL
	{
		NONE,
		LoginInfo,
		LoginResult,
		JoinInfo,
		JoinResult,
		LogoutInfo,
		LogoutResult,
		LobbyEnter,
		LobbyResult,
		MAX
	};

public:
	static CLoginMgr* GetInst();
	static void Create();
	static void Destroy();

private:
	static CLoginMgr* m_instance;

	CLoginMgr();
	~CLoginMgr();

public:
	void Init();
	void End();

	void LoginProc(CSession* _ptr);
	void JoinProc(CSession* _ptr);
	void LogOutProc(CSession* _ptr);
	void EnterLobbyProc(CSession* _ptr);
private:
	//---- login func----
	BOOL LoginCheck(TCHAR* _id, TCHAR* _pw, TCHAR* _nick);
	//-------------------

	//---- join func ----
	BOOL joinCheck(TCHAR* _msg, TCHAR* _id, TCHAR* _nick);
	//-------------------

public:

	SUB_PROTOCOL GetProtocol(BYTE* _recvbuf);
	void Packing(unsigned long _protocol, const TCHAR* _id, const TCHAR* _pw, CSession* _ptr);
	void Packing(unsigned long _protocol, const TCHAR* _str, CSession* _ptr);
	void Packing(unsigned long _protocol, BOOL _flag, const TCHAR* _str, CSession* _ptr);

	void UnPacking(const BYTE* _recvbuf, TCHAR* _id, TCHAR* _pw);
	void UnPacking(const BYTE* _recvbuf, TCHAR* _id, TCHAR* _pw, TCHAR* _nickname);

	BOOL SearchFile(const wchar_t* filename);
	bool FileDataLoad();
	bool FileDataAdd(t_UserInfo* _info);

	void SetJoinlist(list<t_UserInfo*> _list)
	{
		for (t_UserInfo* item : _list)
		{
			m_joinlist.push_back(item);
		}
	}

private:
	list<t_UserInfo*>	m_loginlist;
	list<t_UserInfo*>	m_joinlist;
	CLock* m_lock;

};

