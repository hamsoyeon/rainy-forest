#include "pch.h"
#include "CMainMgr.h"

int main()
{
	CMainMgr::GetInst()->Create();
	CMainMgr::GetInst()->Loop();
	CMainMgr::GetInst()->End();
	return 0;
}