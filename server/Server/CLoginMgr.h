#pragma once

class CSession;

enum class PROTOCOL
{
	LOGIN_INFO,
	LOGIN_RESULT,
	JOIN_INFO,
	JOIN_RESULT,
	LOGOUT_RESULT,
};

enum class RESULT
{
	LOGIN_FAIL,
	LOGIN_SUCCESS,
	ID_ERROR,
	NICK_ERROR,
};

class CLoginMgr
{
	SINGLE(CLoginMgr)
public:
	PROTOCOL GetProtocol(char* _recvbuf);
	int PackPacket(char* _buf, const char* _id, const char* _pw);
	int PackPacket(char* _buf, const char* _str);
	void UnPackPacket(const char* _buf, char* _id, char* _pw);
	void UnPackPacket(const char* _buf, char* _id, char* _pw, char* _nick);

public:
	void Init();
	void End();

	void LoginProc(CSession* _session);
	void JoinProc(CSession* _session);
	void LogOutProc(CSession* _session);

	bool LoginCheck(wstring _id, wstring _pw, wstring _nick);
	bool JoinCheck(wstring& _msg, wstring _id, wstring _nick);

private:
	list<t_userinfo*> login_list;
	list<t_userinfo*> join_list;

};


