#pragma once

class CSession;

class CLobbyMgr
{
public:
	enum class SUB_PROTOCOL
	{
		NONE = -1,
		LobbyResult,
		CreateRoom,
		CreateRoomResult,
		ChatSend,
		ChatRecv,
		RoomlistUpdate, // Ŭ�� �� ����Ʈ �������ϸ� ������ ������
		RoomlistResult, // Ŭ������ �渮��Ʈ�� ������
		Max
	};

	enum class DETAIL_PROTOCOL
	{
		NONE = -1,
		Multi = 1,
		Single,
		NoticeMsg,
		AllMsg,	// ���� �޽���
		AllRoom,
		PageRoom,
		MAX		// ��ü �޽���
	};

public:
	static CLobbyMgr* GetInst();
	static void Create();
	static void Destroy();

private:
	static CLobbyMgr* m_instance;

	CLobbyMgr();
	~CLobbyMgr();

public:
	void Init();
	void End();

	void RoomListUpdateProc(CSession* _ptr);
	void ChatSendProc(CSession* _ptr);

	//void EnterLobbyProc(CSession* _ptr);
	//void EnterRoomProc(CSession* _ptr);
	//void MakeRoomProc(CSession* _ptr);

public:
	void Packing(unsigned long _protocol, const TCHAR* _roomName, const int _roomNum, const int _roomCnt, CSession* _ptr);
	void Packing(unsigned long _protocol, const TCHAR* _msg, CSession* _ptr);

	void UnPacking(const BYTE* _recvbuf, TCHAR* _msg);

public:
	void AddSessionList(CSession* _ptr) {
		m_sessionList.push_back(_ptr);
	};
	void RemoveSessionList(CSession* _ptr) {
		for (CSession* session : m_sessionList)
		{
			if (session == _ptr)
			{
				m_sessionList.remove(_ptr);
				break;
			}
		}
	};

private:
	list<CSession*> m_sessionList; // ���� �κ� �ִ� session����Ʈ

};

