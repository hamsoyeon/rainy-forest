#include "pch.h"
#include "CPacket.h"

#include "CCrypt.h"

// ����� ���� ��ȣȭ�� �Ͽ� ��ŷ
// ��ȣȭ�ϰ� ��ȣȭ�ϰ�

void CPacket::Packing(unsigned long _protocol, BYTE* _data, int _size)
{
	BYTE buf[BUFSIZE];
	ZeroMemory(buf, BUFSIZE);

	BYTE* ptr = buf + sizeof(int); // protocol / data // data -> protocol
	int size = 0;

	// ��ü ������ ������ / ��Ŷ�ѹ� / �������� / ������ ������ / ������

	memcpy(ptr, &m_s_packNo, sizeof(int));
	ptr += sizeof(int);
	size += sizeof(int);
	m_s_packNo++;

	memcpy(ptr, &_protocol, sizeof(unsigned long));
	ptr += sizeof(unsigned long);
	size += sizeof(unsigned long);

	if (_data == nullptr)
	{
		CSocket::WSASEND(buf, size);
		return;
	}

	//�������� size�� �־��ش�.
	memcpy(ptr, &_size, sizeof(int));
	ptr += sizeof(int);
	size += sizeof(int);

	memcpy(ptr, _data, _size);
	ptr = buf;
	size += _size;

	memcpy(ptr, &size, sizeof(int));
	size += sizeof(int);

	/// ��ȣȭ �ڵ�
	/// 1��° �Ķ���� : ��ŷ�� ������
	/// 2��° �Ķ���� : ��ȣȭ�� ����
	/// 3��° �Ķ���� : ������ ������
	/// - ��ȣȭ�� ������
	/// ��ŷ�� ������ �տ� �ִ� ��ü ������ ������� �����ؼ� ��ȣȭ�Ѵ�.
	//CCrypt::Encrypt((BYTE*)buf + sizeof(int), (BYTE*)buf + sizeof(int), size - sizeof(int));

	// �θ�Ŭ�������� �ϴ� ���� ��������� ���ش�.
	CSocket::WSASEND(buf, size);
}
//�޾ƿ����� ��Ŷ ���� packetno/protocol/data

//protocol size data 
void CPacket::UnPacking(int& _protocol, BYTE* _buf)
{
	//Ŭ�󿡼� packetno �ٿ��� �������� �ߴµ� �ϴ� �׽�Ʈ�� ���� packetno�� ���ٴ� �����Ͽ� ����.
	//const TCHAR* ptr = m_trecvbuf.recvbuf + sizeof(int);

	const BYTE* ptr = m_trecvbuf.recvbuf;
	int size = 0;
	memcpy(&_protocol, ptr, sizeof(int));
	ptr += sizeof(int);
	memcpy(&size, ptr, sizeof(int));
	ptr += sizeof(int);
	memcpy(_buf, ptr, size);
}
void CPacket::UnPacking(int& _protocol)
{
	//Ŭ�󿡼� packetno �ٿ��� �������� �ߴµ� �ϴ� �׽�Ʈ�� ���� packetno�� ���ٴ� �����Ͽ� ����.
	//const TCHAR* ptr = m_trecvbuf.recvbuf + sizeof(int);
	const BYTE* ptr = m_trecvbuf.recvbuf + sizeof(int);
	int size = 0;
	memcpy(&_protocol, ptr, sizeof(int));
}

void CPacket::UnPacking(BYTE* _data)
{
	//Ŭ�󿡼� packetno �ٿ��� �������� �ߴµ� �ϴ� �׽�Ʈ�� ���� packetno�� ���ٴ� �����Ͽ� ����.
	//const TCHAR* ptr = m_trecvbuf.recvbuf + sizeof(int);
	const BYTE* ptr = m_trecvbuf.recvbuf + sizeof(int) + sizeof(int);
	int size = 0;

	memcpy(&size, ptr, sizeof(int));
	ptr += sizeof(int);

	memcpy(_data, ptr, size);
}
