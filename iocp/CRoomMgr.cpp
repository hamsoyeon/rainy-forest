#include "pch.h"
#include "CRoomMgr.h"
#include "CRoom.h"
#include "CSession.h"

CRoomMgr* CRoomMgr::m_instance = nullptr;

CRoomMgr* CRoomMgr::GetInst()
{
    return m_instance;
}

void CRoomMgr::Create()
{
    if (m_instance == nullptr)
        m_instance = new CRoomMgr();
}

void CRoomMgr::Destroy()
{
    delete m_instance;
}

CRoomMgr::CRoomMgr()
{
}

CRoomMgr::~CRoomMgr()
{
}

void CRoomMgr::Init()
{
    m_roomList.clear();

    //for (CRoom* room : m_roomList)
    //{
    //    room->GetRoomName();
    //}
}

void CRoomMgr::End()
{
}

//void CRoomMgr::EnterRoomProc(CSession* _ptr)
//{
//
//}

//void CRoomMgr::ExitRoomProc(CSession* _ptr)
//{
//
//}

void CRoomMgr::MakeRoom(CSession* _ptr)
{
    //BYTE buf[BUFSIZE]; ZeroMemory(buf, BUFSIZE);
    //_ptr->UnPacking(buf);
    tRoom* room = new tRoom(L"TestRoom",1);
    //room->playerList.push_back(_ptr); // 해당방에 들어간다.
    m_roomList.push_back(room);
}






