#pragma once

#define ROOM_NAME_LEN 32

class CSession;
class CRoom;
class CLock;

struct tRoom
{
	tRoom(const TCHAR* _roomName, int _roomNum) {
		wcscpy(roomName, _roomName);
		roomNum = _roomNum;
	}

	TCHAR roomName[ROOM_NAME_LEN];
	int roomNum;
	list<CSession*> playerList;

};

class CRoomMgr
{
public:
	static CRoomMgr* GetInst();
	static void Create();
	static void Destroy();

private:
	static CRoomMgr* m_instance;

	CRoomMgr();
	~CRoomMgr();

public:
	void Init();
	void End();

	//void EnterRoomProc(CSession* _ptr);
	//void ExitRoomProc(CSession* _ptr);
	void MakeRoom(CSession* _ptr);
	void DeleteRoom(CSession* _ptr);

public:
	enum class SUB_PROTOCOL
	{
		NONE,
		ENTER_ROOM,
		EXIT_ROOM,
		MAKE_ROOM,
		DELETE_ROOM,
		SUCESS_MAKE_ROOM,
		FAIL_MAKE_ROOM,
		SUCESS_ENTER_ROOM,
		FAIL_ENTER_ROOM,
		MAX,
	};

public:
	list<tRoom*>* GetRoomList() { return &m_roomList; }

private:
	int m_roomNum;
	int m_roomCnt;

	list<tRoom*> m_roomList;
	CLock* m_lock;

};

