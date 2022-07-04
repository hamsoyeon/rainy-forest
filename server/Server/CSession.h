#pragma once

#include "CPacket.h"

#define IDSIZE 10
#define PWSIZE 10
#define NICKSIZE 10

enum class STATE
{
	MENU_SELECT,
	LOGIN,
	JOIN,
	LOGOUT,
};

class CSession :public CPacket
{
public:
	CSession();
	CSession(SOCKET _sock);

public:
	STATE GetState() { return m_state; }
	void SetState(STATE _state) { m_state = _state; }
	t_userinfo* GetUserInfo() { return m_userinfo; }
	void SetUserInfo(t_userinfo _userinfo) {
		memcpy(&m_userinfo, &_userinfo, sizeof(t_userinfo));
	}
	void SetUserInfo(char* _id, char* _pw, char* _nick) {
		RtlZeroMemory(m_userinfo.id, IDSIZE);
		RtlZeroMemory(m_userinfo.pw, IDSIZE);
		RtlZeroMemory(m_userinfo.nick, IDSIZE);
		strcpy(m_userinfo.id, _id);
		strcpy(m_userinfo.pw, _pw);
		strcpy(m_userinfo.nick, _nick);
	}

private:
	t_userinfo* m_userinfo;
	STATE m_state;

};

