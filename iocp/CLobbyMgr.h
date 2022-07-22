#pragma once

class CSession;

// ������ �� ���������� ������ �� ���������� ������ ������

class CLobbyMgr
{
public:
	// 128 8��Ʈ
	enum class SUB_PROTOCOL
	{
		LOBBY,
		CHAT,
		ROOM,

		LOBBY_RESULTL,
		CHAT_RESULT,
		ROOM_RESULT,
	};

	enum class DETAIL_PROTOCOL
	{
		// LOBBY
		MULTI = 1,
		SINGLE = 2,
		ENTER_LOBBY = 4,
		EXIT_LOBBY = 8,

		// CHAT
		ALL_MSG = 1, // ��ü �޽���
		NOTICE_MSG = 2, // ���� �޽���

		// ROOM
		ALL_ROOM = 1,
		PAGE_ROOM = 2,
		MAKE_ROOM = 4,
		ROOMLIST_UPDATE = 8,

		ENTER_ROOM = 16,
	};

	enum class SERVER_DETAIL_PROTOCOL
	{
		// ROOM_RESULT
		ROOMLIST_UPDATE_SUCCESS = 1,
		ROOMLIST_UPDATE_FAIL = 2,
		MAKE_ROOM_SUCCESS = 4,
		MAKE_ROOM_FAIL = 8,

		OVER_PLAYERS = 16,   // �� �ο��ʰ�
		ENTER_ROOM_SUCCESS = 32, // �� ���� ����

		// CHAT_RESULT
		ALL_MSG_SUCCESS = 1,
		ALL_MSG_FAIL = 2,
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

	void MakeRoomProc(CSession* _ptr);
	void EnterRoomProc(CSession* _ptr);

	//void EnterLobbyProc(CSession* _ptr);
	//void EnterRoomProc(CSession* _ptr);
	//void MakeRoomProc(CSession* _ptr);

public:
	void Packing(unsigned long _protocol, const TCHAR* _roomName, const int _roomNum, const int _roomCnt, CSession* _ptr);
	void Packing(unsigned long _protocol, const TCHAR* _msg, CSession* _ptr);

	void UnPacking(const BYTE* _recvbuf, TCHAR* _str);
	void UnPacking(const BYTE* _recvbuf, int& _data);

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

