#include "pch.h"
#include "CPacket.h"

void CPacket::Pack(int _protocol, char* _data, int _size)
{
	// 데이터 구조
	// bufsize / packet number / protocol / datasize / data

	char buf[BUFSIZE];
	ZeroMemory(buf, BUFSIZE);

	char* ptr = buf + sizeof(int);
	int size = 0;

	memcpy(ptr, &send_packet_num, sizeof(int));
	ptr = ptr + sizeof(int);
	size = size + sizeof(int);
	send_packet_num++;

	memcpy(ptr, &_protocol, sizeof(int));
	ptr = ptr + sizeof(int);
	size = size + sizeof(int);

	memcpy(ptr, &_size, sizeof(int));
	ptr = ptr + sizeof(int);
	size = size + sizeof(int);

	memcpy(ptr, _data, _size);
	size = size + _size;
	
	ptr = buf;
	memcpy(ptr, &size, sizeof(int));
	size = size + sizeof(int);

	CSocket::WSASEND(buf, size);
}

void CPacket::UnPack(int& _protocol, char* _buf)
{
	const char* ptr = t_recvbuf.recvbuf;
	int size = 0;
	memcpy(&_protocol, ptr, sizeof(int));
	ptr += sizeof(int);
	memcpy(&size, ptr, sizeof(int));
	ptr += sizeof(int);
	memcpy(_buf, ptr, size);
}
