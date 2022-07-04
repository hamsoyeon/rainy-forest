#pragma once

class CPacket;

class CSocket
{
public:
	CSocket();
	CSocket(const char* _addr, int _port);	// listen socket용 생성자
	CSocket(SOCKET _sock);	
	virtual ~CSocket();

public:
	bool WSASEND();
	bool WSASEND(char* _buf, int _size);
	bool WSARECV();

	SOC Complate_Recv(int _completebyte);
	SOC Complate_Send(int _completebyte);

	void CreateTCPSocket();
	SOCKET Accept();
	void BindAndListen();

	void SendQuePush(t_sendbuf* _sendbuf);
	void SendQuePop();
	void DelaySend();

public:
	SOCKET GetSock() { return m_sock; }
	SOCKADDR_IN GetAddr() { return m_addr; }
	void SetSock(SOCKET _sock) { m_sock = _sock; }
	void SetAddr(SOCKADDR_IN _addr) { memcpy(&m_addr, &_addr, sizeof(SOCKADDR_IN)); }

protected:
	OVERLAP_EX			r_overlap;
	OVERLAP_EX			s_overlap;
	SOCKET				m_sock;
	SOCKADDR_IN			m_addr;

	t_recvbuf			t_recvbuf;
	queue<t_sendbuf*>	m_sendque;

};

