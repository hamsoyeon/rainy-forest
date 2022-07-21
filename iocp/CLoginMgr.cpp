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
	//�α��� ���� ��� �α׾ƿ� ��Ű��.
	m_loginlist.clear();
	m_joinlist.clear();
}


/* �޴� �α��ι�ư ȸ�����Թ�ư */
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

	//1. Ŭ�󿡼� �α��� ���� �޾ƿ�.
	//2. �޾ƿ� ���� UnPackPacket

	_ptr->UnPacking(buf);

	UnPacking(buf, ID, PW);
	ZeroMemory(buf, BUFSIZE);
	//3. �޾ƿ� ������ �α��� ���� ���� üũ
	if (LoginCheck(ID, PW, NICK))//�α��� ����
	{
		_ptr->SetUserInfo(ID, PW, NICK, true);
		m_loginlist.push_back(_ptr->GetUserInfo());
		wsprintf(msg, L"%s���� �α��� ����! ȯ���մϴ�!", NICK);
		result = true;
#ifdef _DEBUG
		CLogMgr::GetInst()->WriteLog(L"������: �α��� ����! %s, %s, %s\n", ID, PW, NICK);
#endif	
	}
	else//�α��� ����
	{
#ifdef _DEBUG
		CLogMgr::GetInst()->WriteLog(L"������: �α��� ����! %s, %s, %s\n", ID, PW, NICK);
#endif	
		//_wsetlocale(LC_ALL, _T("korean"));
		wcscpy(msg, L"�α��� ����!");
	}
	unsigned long protocol = 0;
	// test��
	CProtocolMgr::GetInst()->AddMainProtocol(&protocol, (unsigned long)MAIN_PROTOCOL::LOGIN);
	CProtocolMgr::GetInst()->AddSubProtocol(&protocol, (unsigned long)SUB_PROTOCOL::LoginResult);
	//4. �α��� ���� ���θ� ��ŷ sendlist�� ������ �ִ� ���� �����Ͱ� ���ٸ� �ٷ� Ŭ�� ����.
	Packing(protocol, result, msg, _ptr);
}

void CLoginMgr::JoinProc(CSession* _ptr)
{
	CLock_Guard<CLock> lock(m_lock);
	//1. Ŭ�� ���� ȸ������ ������ ������, 
	//2. ��ϵ� �������� Ȯ���Ͽ� ����� ���� packet�� �����Ͽ� Send�Ѵ�.

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
	if (joinCheck(msg, ID, NICK)) // ȸ������ ����
	{
		t_UserInfo* user = new t_UserInfo(ID, PW, NICK);
		m_joinlist.push_back(user);
		CDBMgr::GetInst()->InsertJointbl(user);
		//FileDataAdd(user);
		wsprintf((TCHAR*)buf, L"%s�� ȸ�����Կ� �����Ͽ����ϴ�!", NICK);
		ZeroMemory(temp, BUFSIZE);
		result = true;
#ifdef _DEBUG
		CLogMgr::GetInst()->FileWriteLog(L"������: ȸ������ ����! %s, %s, %s\n", ID, PW, NICK);
		//CLogMgr::GetInst()->FileReadLogLast();
		printf("\n=====\n");
		//CLogMgr::GetInst()->FileReadLogAll();
		//CLogMgr::GetInst()->WriteLog("������: ȸ������ ����! %s, %s, %s\n", ID, PW, NICK);
		TCHAR temp2[BUFSIZE];
		ZeroMemory(temp2, BUFSIZE);
		wsprintf(temp2, L"������: ȸ������ ����! %s, %s, %s\n", ID, PW, NICK);
		CDBMgr::GetInst()->InsertJoinLog(temp2);
#else
		wsprintf(temp, "ȸ������ ���� %s, %s , %s\n", ID, PW, NICK);
		CDBMgr::GetInst()->InsertJoinLog(temp);
#endif	
	}
	else // ȸ������ ����
	{
#ifdef _DEBUG
		CLogMgr::GetInst()->WriteLog(L"������: ȸ������ ����! %s, %s, %s\n", ID, PW, NICK);
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
	Packing(protocol, L"�α׾ƿ��� �����Ͽ����ϴ�!", _ptr);
}

void CLoginMgr::EnterLobbyProc(CSession* _ptr)
{
	CLock_Guard<CLock> lock(m_lock);

	// �ε��ڷ�� �ѱ�� ���Ҽ��� ����� dummy ����
	// ���� �ִ� ����� ����Ʈ, 

	// ���� ä��â�� ����
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

	// �κ������ �κ��ο��� �߰��Ѵ�.
	CLobbyMgr::GetInst()->AddSessionList(_ptr);

	_ptr->SetState(_ptr->GetLobbyState()); // lobby state�� ��ȯ�Ѵ�.

	TCHAR msg[BUFSIZE]; ZeroMemory(msg, BUFSIZE * 2);
	wsprintf(msg, L"%s���� �κ� �����Ͽ����ϴ�.", _ptr->GetUserInfo()->nickname);

	unsigned long protocol = 0;
	CProtocolMgr::GetInst()->AddSubProtocol(&protocol, (unsigned long)SUB_PROTOCOL::LobbyResult);
	Packing(protocol, msg, _ptr);
}

BOOL CLoginMgr::LoginCheck(TCHAR* _id, TCHAR* _pw, TCHAR* _nick)
{
	//1.ȸ�������� ��ġ�ϴ� �� üũ
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
				//2.�α������� list�� ��ġ���� �ʴ��� üũ
				for (t_UserInfo* loginfo : m_loginlist)
				{
					//�̹� �α��� ���� ���
					if (!memcmp(loginfo, userinfo, sizeof(t_UserInfo)))
					{
						return false;
					}
					else//�α��� ����
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
		// ��ϵ� ID�� ã�� ���
		if (!wcscmp(tuserinfo->id, _id))
		{
			wcscpy(_msg, L"�̹� ��ϵ� ID�Դϴ�.\n");
			return false;
		}
		// ��ϵ� NICK�� ã�� ���
		if (!wcscmp(tuserinfo->nickname, _nick))
		{
			wcscpy(_msg, L"�̹� �����ϴ� NICKNAME�Դϴ�.\n");
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

