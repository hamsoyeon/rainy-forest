#include "pch.h"
#include "CMainMgr.h"

int main()
{
	//_CrtSetDbgFlag(_CRTDBG_ALLOC_MEM_DF | _CRTDBG_LEAK_CHECK_DF);
	//_CrtDumpMemoryLeaks();
	CMainMgr::Create();
	CMainMgr::GetInst()->Loop();
	CMainMgr::Destroy();
	return 0;
}

/*
오늘 할일
1.db 여러 레코드 결과값을 가져오는 함수 만들기. (proc)
2.join 업데이트 될때마다 즉각적으로 db에 업로드
3.logwritelast 수정하기.
*/

/* 임계영역 정하기

- CDBMgr

- CLoinMgr

- CSessionMgr

- CSocket

*/