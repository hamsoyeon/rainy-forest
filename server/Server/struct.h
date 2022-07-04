#pragma once

struct OVERLAP_EX
{
	OVERLAPPED overlapped;
	IO_TYPE type;
	void* session;
};

struct t_sendbuf
{
	t_sendbuf()
	{
		ZeroMemory(sendbuf, BUFSIZE);
		sendbytes = comp_sendbytes = 0;
	}
	t_sendbuf(char* _data, int _sendbytes)
	{
		ZeroMemory(sendbuf, BUFSIZE);
		memcpy(sendbuf, _data, _sendbytes);
		sendbytes = _sendbytes;
		comp_sendbytes = 0;
	}

	int comp_sendbytes;
	int sendbytes;
	char sendbuf[BUFSIZE];
};

struct t_recvbuf
{
	t_recvbuf()
	{
		ZeroMemory(recvbuf, BUFSIZE);
		is_flag = false;
		recvbytes = sizeof(int);
		comp_recvbytes = 0;
	}
	t_recvbuf(char* _data, int _recvbytes)
	{
		memcpy(recvbuf, _data, _recvbytes);
		recvbytes = _recvbytes;
		comp_recvbytes = 0;
	}

	int comp_recvbytes;
	int recvbytes;
	char recvbuf[BUFSIZE];
	bool is_flag;			// size가 잘 왔는지 체크하는 변수
};

struct t_userinfo
{
	t_userinfo()
	{
		ZeroMemory(id, IDSIZE);
		ZeroMemory(pw, PWSIZE);
		ZeroMemory(nick, NICKSIZE);
	}
	t_userinfo(char* _id, char* _pw, char* _nickname)
	{
		ZeroMemory(id, IDSIZE);
		ZeroMemory(pw, PWSIZE);
		ZeroMemory(nick, NICKSIZE);
		strcpy(id, _id);
		strcpy(pw, _pw);
		strcpy(nick, _nickname);
	}

	char id[IDSIZE];
	char pw[PWSIZE];
	char nick[NICKSIZE];
};