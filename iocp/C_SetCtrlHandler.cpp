#include "pch.h"
#include "CMainMgr.h"
#include "C_SetCtrlHandler.h"
C_SetCtrlHandler* C_SetCtrlHandler::instance = nullptr;

void C_SetCtrlHandler::Create()
{
    if (instance == nullptr)
        instance = new C_SetCtrlHandler();
}

void C_SetCtrlHandler::Destroy()
{
    delete instance;
}

C_SetCtrlHandler* C_SetCtrlHandler::GetInst()
{
    return instance;
}

void C_SetCtrlHandler::Init()
{
    SetConsoleCtrlHandler((PHANDLER_ROUTINE)CtrlHandler, TRUE);
}

void C_SetCtrlHandler::End()
{
   
}

BOOL WINAPI C_SetCtrlHandler::CtrlHandler(DWORD _CtrlType)
{
    switch (_CtrlType)
    {
    case CTRL_C_EVENT:
    case CTRL_CLOSE_EVENT:
    case CTRL_BREAK_EVENT:
        CMainMgr::GetInst()->End();
        CMainMgr::GetInst()->Destroy();
        break;
    }
    MessageBox(NULL, L"1234", L"1234", MB_OK);
    return true;
}




