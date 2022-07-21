#pragma once

#define ROOM_NAME_LEN 32

class CRoom
{
public:
	TCHAR* GetRoomName() { return m_roomName; }
	int GetRoomNum() { return m_roomNum; }
	void SetRoomName(TCHAR* _name) { wcscpy(m_roomName, _name); }
	void SetRoomNum(int _num) { m_roomNum = _num; }

private:
	TCHAR m_roomName[ROOM_NAME_LEN];
	int m_roomNum;

};

