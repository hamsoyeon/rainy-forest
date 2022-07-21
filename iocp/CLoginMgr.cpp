#include "pch.h"
#include "CLoginMgr.h"
#include "CSession.h"
#include "CMainMgr.h"
#include "CDBMgr.h"
#include "CLogMgr.h"
#include "CProtocolMgr.h"
#include "CLock.h"
#include "CLockGuard.h"
#include "CLobbyMgr.h"

CLoginMgr* CLoginMgr::m_instance = nullptr;

CLoginMgr* CLoginMgr::GetInst()
{
	return m_instance;
}

void CLoginMgr::Create()
{
	if (m_instance == nullptr)
		m_instance = new CLoginMgr();
}

void CLoginMgr::Destroy()
{
	delete m_instance;
}

CLoginMgr::CLoginMgr()
	: m_lock(new CLock)
{

}

CLoginMgr::~CLoginMgr()
{
	delete m_lock;
}

void CLoginMgr::Init()
{

}

void CLoginMgr::End()
{
	if (m_joinlist.size() == 0)return;
	//CDBMgr::GetInst()->SetJoin(m_joinlist);
	//로그인 정보 모두 로그아웃 시키기.
	m_loginlist.clear();
	m_joinlist.clear();
}


/* 메뉴 로그인버튼 회원가입버튼 */
void CLoginMgr::LoginProc(CSession* _ptr)
{
	CLock_Guard<CLock> lock(m_lock);
	BYTE buf[BUFSIZE];
	TCHAR msg[BUFSIZE];
	TCHAR ID[BUFSIZE], PW[BUFSIZE], NICK[BUFSIZE], temp[BUFSIZE];
	ZeroMemory(buf, BUFSIZE);
	ZeroMemory(ID, BUFSIZE);
	ZeroMemory(PW, BUFSIZE * 2);
	ZeroMemory(NICK, BUFSIZE * 2);
	ZeroMemory(temp, BUFSIZE * 2);
	ZeroMemory(msg, BUFSIZE * 2);

	int size = 0;
	int p;

	bool result = false;

	//1. 클라에서 로그인 정보 받아옴.
	//2. 받아온 정보 UnPackPacket

	_ptr->UnPacking(buf);

	UnPacking(buf, ID, PW);
	ZeroMemory(buf, BUFSIZE);
	//3. 받아온 정보로 로그인 가능 여부 체크
	if (LoginCheck(ID, PW, NICK))//로그인 성공
	{
		_ptr->SetUserInfo(ID, PW, NICK, true);
		m_loginlist.push_back(_ptr->GetUserInfo());
		wsprintf(msg, L"%s님이 로그인 성공! 환영합니다!", NICK);
		result = true;
#ifdef _DEBUG
		CLogMgr::GetInst()->WriteLog(L"데이터: 로그인 성공! %s, %s, %s\n", ID, PW, NICK);
#endif	
	}
	else//로그인 실패
	{
#ifdef _DEBUG
		CLogMgr::GetInst()->WriteLog(L"데이터: 로그인 실패! %s, %s, %s\n", ID, PW, NICK);
#endif	
		//_wsetlocale(LC_ALL, _T("korean"));
		wcscpy(msg, L"로그인 실패!");
	}
	unsigned long protocol = 0;
	// test용
	CProtocolMgr::GetInst()->AddMainProtocol(&protocol, (unsigned long)MAIN_PROTOCOL::LOGIN);
	CProtocolMgr::GetInst()->AddSubProtocol(&protocol, (unsigned long)SUB_PROTOCOL::LoginResult);
	//4. 로그인 성공 여부를 패킹 sendlist가 보내고 있는 중인 데이터가 없다면 바로 클라에 전송.
	Packing(protocol, result, msg, _ptr);
}

void CLoginMgr::JoinProc(CSession* _ptr)
{
	CLock_Guard<CLock> lock(m_lock);
	//1. 클라가 보낸 회원가입 정보를 받으면, 
	//2. 등록된 정보인지 확인하여 결과에 따라 packet을 조립하여 Send한다.

	BYTE buf[BUFSIZE], sendbuf[BUFSIZE];
	TCHAR ID[BUFSIZE], PW[BUFSIZE], NICK[BUFSIZE];
	ZeroMemory(buf, BUFSIZE);
	ZeroMemory(ID, BUFSIZE);
	ZeroMemory(PW, BUFSIZE);
	ZeroMemory(NICK, BUFSIZE);
	ZeroMemory(sendbuf, BUFSIZE);
	int size = 0;
	int p;
	TCHAR msg[BUFSIZE];
	TCHAR temp[BUFSIZE];
	BOOL result = false;
	_ptr->UnPacking(buf);

	UnPacking(buf, ID, PW, NICK);
	ZeroMemory(buf, BUFSIZE);
	if (joinCheck(msg, ID, NICK)) // 회원가입 성공
	{
		t_UserInfo* user = new t_UserInfo(ID, PW, NICK);
		m_joinlist.push_back(user);
		CDBMgr::GetInst()->InsertJointbl(user);
		//FileDataAdd(user);
		wsprintf((TCHAR*)buf, L"%s님 회원가입에 성공하였습니다!", NICK);
		ZeroMemory(temp, BUFSIZE);
		result = true;
#ifdef _DEBUG
		CLogMgr::GetInst()->FileWriteLog(L"데이터: 회원가입 성공! %s, %s, %s\n", ID, PW, NICK);
		//CLogMgr::GetInst()->FileReadLogLast();
		printf("\n=====\n");
		//CLogMgr::GetInst()->FileReadLogAll();
		//CLogMgr::GetInst()->WriteLog("데이터: 회원가입 성공! %s, %s, %s\n", ID, PW, NICK);
		TCHAR temp2[BUFSIZE];
		ZeroMemory(temp2, BUFSIZE);
		wsprintf(temp2, L"데이터: 회원가입 성공! %s, %s, %s\n", ID, PW, NICK);
		CDBMgr::GetInst()->InsertJoinLog(temp2);
#else
		wsprintf(temp, "회원가입 성공 %s, %s , %s\n", ID, PW, NICK);
		CDBMgr::GetInst()->InsertJoinLog(temp);
#endif	
	}
	else // 회원가입 실패
	{
#ifdef _DEBUG
		CLogMgr::GetInst()->WriteLog(L"데이터: 회원가입 실패! %s, %s, %s\n", ID, PW, NICK);
#endif	
		wcscpy((TCHAR*)buf, msg);
	}

	unsigned long protocol = 0;
	CProtocolMgr::GetInst()->AddSubProtocol(&protocol, (unsigned long)SUB_PROTOCOL::JoinResult);
	Packing(protocol, result, (TCHAR*)buf, _ptr);
}

void CLoginMgr::LogOutProc(CSession* _ptr)
{
	CLock_Guard<CLock> lock(m_lock);
	BYTE sendbuf[BUFSIZE];
	ZeroMemory(sendbuf, BUFSIZE);
	m_loginlist.remove(_ptr->GetUserInfo());
	_ptr->SetUserInfo(NULL, NULL, NULL, false);
	unsigned long protocol = 0;
	CProtocolMgr::GetInst()->AddSubProtocol(&protocol, (unsigned long)SUB_PROTOCOL::LogoutResult);
	Packing(protocol, L"로그아웃에 성공하였습니다!", _ptr);
}

void CLoginMgr::EnterLobbyProc(CSession* _ptr)
{
	CLock_Guard<CLock> lock(m_lock);

	// 로딩자료들 넘기는 역할수행 현재는 dummy 보냄
	// 현재 있는 방들의 리스트, 

	// 보낼 채팅창의 정보
	//list<tRoom*> roomList = CRoomMgr::GetInst()->GetRoomList();
	//for (tRoom* room : roomList)
	//{
	//	unsigned long protocol = 0;
	//	wcscpy(room->m_roomName, L"roomName");
	//	room->m_roomNum = 1;
	//	CProtocolMgr::GetInst()->AddSubProtocol(&protocol, (unsigned long)SUB_PROTOCOL::LobbyResult);
	//	Packing(protocol, room->m_roomNum, room->m_roomName, _ptr);
	//}

	//tRoom* room = new tRoom(L"roomName", 1);
	//unsigned long protocol = 0;
	//CProtocolMgr::GetInst()->AddSubProtocol(&protocol, (unsigned long)SUB_PROTOCOL::LobbyResult);
	//Packing(protocol, room->m_roomNum, room->m_roomName, _ptr);

	// 로비입장시 로비인원을 추가한다.
	CLobbyMgr::GetInst()->AddSessionList(_ptr);

	_ptr->SetState(_ptr->GetLobbyState()); // lobby state로 전환한다.

	TCHAR msg[BUFSIZE]; ZeroMemory(msg, BUFSIZE * 2);
	wsprintf(msg, L"%s님이 로비에 입장하였습니다.", _ptr->GetUserInfo()->nickname);

	unsigned long protocol = 0;
	CProtocolMgr::GetInst()->AddSubProtocol(&protocol, (unsigned long)SUB_PROTOCOL::LobbyResult);
	Packing(protocol, msg, _ptr);
}

BOOL CLoginMgr::LoginCheck(TCHAR* _id, TCHAR* _pw, TCHAR* _nick)
{
	//1.회원정보와 일치하는 지 체크
	if (m_joinlist.size() > 0)
	{
		for (t_UserInfo* userinfo : m_joinlist)
		{
			if (!wcscmp(userinfo->id, _id) && !wcscmp(userinfo->pw, _pw))
			{
				if (m_loginlist.size() == 0)
				{
					wcscpy(_nick, userinfo->nickname);
					return true;
				}
				//2.로그인중인 list와 일치하지 않는지 체크
				for (t_UserInfo* loginfo : m_loginlist)
				{
					//이미 로그인 중인 경우
					if (!memcmp(loginfo, userinfo, sizeof(t_UserInfo)))
					{
						return false;
					}
					else//로그인 성공
					{
						wcscpy(_nick, userinfo->nickname);
						return true;
					}
				}

			}
		}
	}
	return false;
}

BOOL CLoginMgr::joinCheck(TCHAR* _msg, TCHAR* _id, TCHAR* _nick)
{
	for (t_UserInfo* tuserinfo : m_joinlist)
	{
		// 등록된 ID를 찾은 경우
		if (!wcscmp(tuserinfo->id, _id))
		{
			wcscpy(_msg, L"이미 등록된 ID입니다.\n");
			return false;
		}
		// 등록된 NICK을 찾은 경우
		if (!wcscmp(tuserinfo->nickname, _nick))
		{
			wcscpy(_msg, L"이미 존재하는 NICKNAME입니다.\n");
			return false;
		}
	}
	return true;
}

CLoginMgr::SUB_PROTOCOL CLoginMgr::GetProtocol(BYTE* _recvbuf)
{
	SUB_PROTOCOL protocol;
	memcpy(&protocol, _recvbuf, sizeof(SUB_PROTOCOL));
	return protocol;
}
void CLoginMgr::Packing(unsigned long _protocol, const TCHAR* _id, const TCHAR* _pw, CSession* _ptr)
{
	BYTE buf[BUFSIZE]; ZeroMemory(buf, BUFSIZE);
	BYTE* ptr = buf;
	int size = 0;
	int strsize = wcslen(_id);
	unsigned long protocol = _protocol;

	memcpy(ptr, &strsize, sizeof(int));
	size += sizeof(int);
	ptr += sizeof(int);

	memcpy(ptr, _id, strsize);
	size += strsize;
	ptr += strsize;

	strsize = wcslen(_pw);
	memcpy(ptr, &strsize, sizeof(int));
	size += sizeof(int);
	ptr += sizeof(int);

	memcpy(ptr, _pw, strsize);
	size += strsize;
	ptr += strsize;

	_ptr->Packing(protocol, buf, size);
}
void CLoginMgr::Packing(unsigned long _protocol, const TCHAR* _str, CSession* _ptr)
{
	BYTE buf[BUFSIZE]; ZeroMemory(buf, BUFSIZE);
	BYTE* ptr = buf;
	int size = 0;
	int strsize = wcslen(_str) * 2;
	unsigned long protocol = _protocol;

	memcpy(ptr, &strsize, sizeof(int));
	size += sizeof(int);
	ptr += sizeof(int);

	memcpy(ptr, _str, strsize);
	size += strsize;
	ptr += strsize;

	_ptr->Packing(protocol, buf, size);
}
void CLoginMgr::Packing(unsigned long _protocol, BOOL _flag, const TCHAR* _str, CSession* _ptr)
{
	BYTE buf[BUFSIZE]; ZeroMemory(buf, BUFSIZE);
	BYTE* ptr = buf;
	int size = 0;
	int strsize = wcslen(_str) * 2;
	unsigned long protocol = _protocol;

	memcpy(ptr, &_flag, sizeof(BOOL));
	size += sizeof(BOOL);
	ptr += sizeof(BOOL);

	memcpy(ptr, &strsize, sizeof(int));
	size += sizeof(int);
	ptr += sizeof(int);

	memcpy(ptr, _str, strsize);
	size += strsize;
	ptr += strsize;

	_ptr->Packing(protocol, buf, size);
}



void CLoginMgr::UnPacking(const BYTE* _buf, TCHAR* _id, TCHAR* _pw)
{
	const BYTE* ptr = _buf;
	int strsize = 0;

	memcpy(&strsize, ptr, sizeof(int));
	ptr += sizeof(int);

	memcpy(_id, ptr, strsize);
	ptr += strsize;

	memcpy(&strsize, ptr, sizeof(int));
	ptr += sizeof(int);

	memcpy(_pw, ptr, strsize);
	ptr += strsize;
}

void CLoginMgr::UnPacking(const BYTE* _buf, TCHAR* _id, TCHAR* _pw, TCHAR* _nickname)
{
	const BYTE* ptr = _buf;
	int strsize = 0;

	memcpy(&strsize, ptr, sizeof(int));
	ptr += sizeof(int);

	memcpy(_id, ptr, strsize);
	ptr += strsize;

	memcpy(&strsize, ptr, sizeof(int));
	ptr += sizeof(int);

	memcpy(_pw, ptr, strsize);
	ptr += strsize;

	memcpy(&strsize, ptr, sizeof(int));
	ptr += sizeof(int);

	memcpy(_nickname, ptr, strsize);
	ptr += strsize;
}

BOOL CLoginMgr::SearchFile(const wchar_t* filename)
{
	WIN32_FIND_DATA FindFileData;
	HANDLE hFindFile = FindFirstFile(filename, &FindFileData);
	if (hFindFile == INVALID_HANDLE_VALUE)
		return FALSE;
	else {
		FindClose(hFindFile);
		return TRUE;
	}
}

bool CLoginMgr::FileDataLoad()
{
	if (!SearchFile(L"UserInfo.info"))
	{
		FILE* fp = fopen("UserInfo.info", "wb");
		fclose(fp);
		return true;
	}

	FILE* fp = fopen("UserInfo.info", "rb");
	if (fp == NULL)
	{
		return false;
	}

	t_UserInfo info;
	memset(&info, 0, sizeof(t_UserInfo));

	while (1)
	{
		fread(&info, sizeof(t_UserInfo), 1, fp);
		if (feof(fp))
		{
			break;
		}
		t_UserInfo* ptr = new t_UserInfo;
		memcpy(ptr, &info, sizeof(t_UserInfo));
		m_joinlist.push_back(ptr);
	}

	fclose(fp);
	return true;
}

bool CLoginMgr::FileDataAdd(t_UserInfo* _info)
{
	FILE* fp = fopen("UserInfo.info", "ab");
	if (fp == NULL)
	{
		return false;
	}

	int retval = fwrite(_info, 1, sizeof(t_UserInfo), fp);

	if (retval != sizeof(t_UserInfo))
	{
		return false;
	}

	fclose(fp);
	return true;
}

