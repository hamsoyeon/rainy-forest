#pragma once
class CLogMgr
{
public:
	static CLogMgr* GetInst();
	static void Create();
	static void Destroy();
	void Init();
	void End();
	TCHAR* WriteLog(const TCHAR* fmt, ...);
	TCHAR* FileWriteLog(const TCHAR* fmt, ...);
	TCHAR* FileReadLogLast();
	TCHAR* FileReadLogAll();

	// ++ 
	// C#은 유니코드 문자셋을 사용하므로, 소켓통신시 같은 문자셋으로 맞추어야 한다.
	void err_quit(const TCHAR* msg);
	void err_display(const TCHAR* msg);

private:
	ifstream readFile;
	ofstream writeFile;
	static CLogMgr* m_instance;
	char m_logfilename[100];
	time_t timer;
	tm* t;
	CLogMgr();
	~CLogMgr();

};

