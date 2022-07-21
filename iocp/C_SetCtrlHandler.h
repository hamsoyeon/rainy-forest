#pragma once
class C_SetCtrlHandler
{
public:
	static void Create();
	static void Destroy();
	static C_SetCtrlHandler* GetInst();
	void Init();
	void End();
	static BOOL WINAPI CtrlHandler(DWORD _CtrlType);

private:
	C_SetCtrlHandler() {}
	~C_SetCtrlHandler() {}

private:
	static C_SetCtrlHandler* instance;

};

