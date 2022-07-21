#pragma once
class CLock
{
public:
	CLock()
	{
		InitializeCriticalSection(&cs);
	}
	void lock()
	{
		EnterCriticalSection(&cs);
	}
	void unlock()
	{
		LeaveCriticalSection(&cs);
	}
	~CLock()
	{
		DeleteCriticalSection(&cs);
	}

private:
	CRITICAL_SECTION cs;
};