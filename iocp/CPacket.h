#pragma once
#include "CSocket.h"
class CPacket :public CSocket
{
public:
	CPacket() 
	{ 
		m_s_packNo = 0;
		m_r_packNo = 0;
	}
	CPacket(SOCKET _sock):CSocket(_sock)
	{
		m_s_packNo = 0;
		m_r_packNo = 0;
	}
	// 패킹, 언패킹 기능을 명시적으로 
	void Packing(unsigned long _protocol, BYTE* _data, int _size);
	void UnPacking(int& _protocol, BYTE* _buf);
	void UnPacking(int& _protocol);
	void UnPacking(BYTE* _data);

private:
	int m_s_packNo; 
	int m_r_packNo;

};

