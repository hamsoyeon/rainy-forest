#include "pch.h"
#include "CLogMgr.h"

CLogMgr* CLogMgr::m_instance = nullptr;

CLogMgr* CLogMgr::GetInst()
{
	return m_instance;
}

void CLogMgr::Create()
{
	if (m_instance == nullptr)
		m_instance = new CLogMgr();
}

void CLogMgr::Destroy()
{
	delete m_instance;
}

void CLogMgr::Init()
{
	timer = time(NULL);
	t = localtime(&timer);
	sprintf(m_logfilename, "[%d_%d_%d]  Log.txt", t->tm_year + 1900, t->tm_mon + 1, t->tm_mday);
	readFile.open(m_logfilename);
}

void CLogMgr::End()
{
	readFile.close();
}

TCHAR* CLogMgr::WriteLog(const TCHAR* fmt, ...)
{
	va_list arg;
	va_start(arg, fmt);

	TCHAR cbuf[256]; ZeroMemory(cbuf, sizeof(cbuf));
	t = localtime(&timer);
	_wsetlocale(LC_ALL, _T("korean"));
	wsprintf(cbuf, L"날짜:%d년%d월%d일 시간: %d시 %d분 ", t->tm_year + 1900, t->tm_mon + 1, t->tm_mday, t->tm_hour, t->tm_min);
	int size = wcslen(cbuf);
	_vstprintf(cbuf + size, fmt, arg);
	_tprintf(L"%s", cbuf);

	va_end(arg);
	return cbuf;
}
TCHAR* CLogMgr::FileWriteLog(const TCHAR* fmt, ...)
{
	va_list arg;
	va_start(arg, fmt);

	TCHAR cbuf[256]; ZeroMemory(cbuf, sizeof(cbuf));
	t = localtime(&timer);
	_wsetlocale(LC_ALL, _T("korean"));
	wsprintf(cbuf, L"[날짜:%d년%d월%d일] [시간: %d시 %d분] ", t->tm_year + 1900, t->tm_mon + 1, t->tm_mday, t->tm_hour, t->tm_min);
	int size = wcslen(cbuf);
	_vstprintf(cbuf + size, fmt, arg);
	va_end(arg);
	sprintf(m_logfilename, "[%d_%d_%d]  Log.txt", t->tm_year + 1900, t->tm_mon + 1, t->tm_mday);

	writeFile.open(m_logfilename, std::ios::app);
	size = wcslen(cbuf);
	writeFile.write((char*)cbuf, size);
	writeFile.close();

	return cbuf;
}
//고치기 안나옴
TCHAR* CLogMgr::FileReadLogLast()
{
	char temp[256];
	ZeroMemory(temp, 256);
	t = localtime(&timer);
	sprintf(m_logfilename, "[%d_%d_%d]  Log.txt", t->tm_year + 1900, t->tm_mon + 1, t->tm_mday);
	//readFile.open(m_logfilename);
	//if (readFile.is_open())
	//{
	//	while (!readFile.eof())
	//	{
	//		readFile.getline((char*)temp, 256);
	//	}
	//	printf("%s\n", temp);
	//	readFile.clear();
	//	readFile.seekg(0, std::ios_base::beg);
	//}
	return 0;
}
TCHAR* CLogMgr::FileReadLogAll()
{
	if (readFile.is_open())
	{
		while (!readFile.eof())
		{
			char temp[256];
			ZeroMemory(temp, 256);
			readFile.getline((char*)temp, 256);
			printf("%s\n", temp);
		}
		readFile.clear();
		readFile.seekg(0, std::ios_base::beg);
	}
	return 0;
}
CLogMgr::CLogMgr()
{

}

CLogMgr::~CLogMgr()
{

}

void CLogMgr::err_quit(const TCHAR* msg)
{
	LPVOID lpMsgBuf;
	FormatMessage(FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM, NULL,
		WSAGetLastError(), MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT), (LPTSTR)&lpMsgBuf,
		0, NULL);
	MessageBox(NULL, (LPCTSTR)lpMsgBuf, msg, MB_OK);
	LocalFree(lpMsgBuf);
	exit(-1);
}

void CLogMgr::err_display(const TCHAR* msg)
{
	LPVOID lpMsgBuf;
	FormatMessage(FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM, NULL,
		WSAGetLastError(), MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT), (LPTSTR)&lpMsgBuf,
		0, NULL);
	printf("[%s] %s", msg, lpMsgBuf);
	LocalFree(lpMsgBuf);
}