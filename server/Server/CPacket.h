#pragma once

#include "CSocket.h"

class CPacket :public CSocket
{
public:
	CPacket()
		: send_packet_num(0)
		, recv_packet_num(0)
	{}
	CPacket(SOCKET _sock)
		: CSocket(_sock)
		, send_packet_num(0)
		, recv_packet_num(0)
	{}

	void Pack(int _protocol, char* _data, int _size);
	void UnPack(int& _protocol, char* _buf);

private:
	int send_packet_num;
	int recv_packet_num;

};

