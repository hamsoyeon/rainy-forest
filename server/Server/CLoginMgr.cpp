#include "pch.h"
#include "CLoginMgr.h"

#include "CSession.h"

PROTOCOL CLoginMgr::GetProtocol(char* _recvbuf)
{
	return PROTOCOL();
}

int CLoginMgr::PackPacket(char* _buf, const char* _id, const char* _pw)
{
	char* ptr = _buf;
	int size = 0;
	int strsize = strlen(_id);

	memcpy(ptr, &strsize, sizeof(int));
	size = size + sizeof(int);
	ptr = ptr + sizeof(int);

	memcpy(ptr, _id, strsize);
	size = size + strsize;
	ptr = ptr + strsize;

	strsize = strlen(_pw);

	memcpy(ptr, &strsize, sizeof(int));
	size = size + sizeof(int);
	ptr = ptr + sizeof(int);

	memcpy(ptr, _pw, strsize);
	size = size + strsize;
	ptr = ptr + strsize;

	return size;
}

int CLoginMgr::PackPacket(char* _buf, const char* _str)
{
	char* ptr = _buf;
	int size = 0;
	int strsize = strlen(_str);

	memcpy(ptr, &strsize, sizeof(int));
	size = size + sizeof(int);
	ptr = ptr + sizeof(int);

	memcpy(ptr, _str, strsize);
	size = size + strsize;
	ptr = ptr + strsize;

	return size;
}

void CLoginMgr::UnPackPacket(const char* _buf, char* _id, char* _pw)
{
	const char* ptr = _buf;
	int strsize = 0;

	memcpy(&strsize, ptr, sizeof(int));
	ptr = ptr + sizeof(int);

	memcpy(_id, ptr, strsize);
	ptr = ptr + strsize;

	memcpy(&strsize, ptr, sizeof(int));
	ptr = ptr + sizeof(int);

	memcpy(_pw, ptr, strsize);
	ptr = ptr + strsize;
}

void CLoginMgr::UnPackPacket(const char* _buf, char* _id, char* _pw, char* _nick)
{
	const char* ptr = _buf;
	int strsize = 0;

	memcpy(&strsize, ptr, sizeof(int));
	ptr = ptr + sizeof(int);

	memcpy(_id, ptr, strsize);
	ptr = ptr + strsize;

	memcpy(&strsize, ptr, sizeof(int));
	ptr = ptr + sizeof(int);

	memcpy(_pw, ptr, strsize);
	ptr = ptr + strsize;

	memcpy(&strsize, ptr, sizeof(int));
	ptr = ptr + sizeof(int);

	memcpy(_nick, ptr, strsize);
	ptr = ptr + strsize;
}

void CLoginMgr::Init()
{

}

void CLoginMgr::End()
{

}

void CLoginMgr::LoginProc(CSession* _session)
{
	char buf[BUFSIZE], ID
}

void CLoginMgr::JoinProc(CSession* _session)
{
}

void CLoginMgr::LogOutProc(CSession* _session)
{
}

bool CLoginMgr::LoginCheck(wstring _id, wstring _pw, wstring _nick)
{
	return false;
}

bool CLoginMgr::JoinCheck(wstring& _msg, wstring _id, wstring _nick)
{
	return false;
}


