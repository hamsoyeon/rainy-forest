#include "pch.h"
#include "CSocket.h"

CSocket::CSocket()
{
}

CSocket::CSocket(const char* _addr, int _port)
{
	int retval = 0;

	m_sock = socket(AF_INET, SOCK_STREAM, 0);
	if (m_sock == INVALID_SOCKET)
	{
		err_quit(L"socket()");
	}
	ZeroMemory(&m_addr, sizeof(SOCKADDR_IN));
	m_addr.sin_family = AF_INET;
	if (_addr[0] == '\0')
	{
		m_addr.sin_addr.s_addr = htonl(INADDR_ANY);
	}
	else
	{
#pragma region inet_addr 설명
		/// <summary>
		/// 함수 파라미터 값에 IP주소 문자열의 시작주소를 넣어주면
		/// 이 함수가 알아서 빅엔디안 32비트 unsigned log 형의 값으로 만들어준다.
		/// </summary>
#pragma endregion
		m_addr.sin_addr.s_addr = inet_addr(_addr);
	}

	m_addr.sin_port = htons(_port);

	retval = bind(m_sock, (SOCKADDR*)&m_addr, sizeof(m_addr));
	if (retval == SOCKET_ERROR)
	{
		err_quit(L"bind()");
	}
	retval = listen(m_sock, SOMAXCONN);
	if (retval == SOCKET_ERROR)
	{
		err_quit(L"listen()");
	}
}

CSocket::CSocket(SOCKET _sock)
{
	m_sock = _sock;
	int len = 0;
	SOCKADDR_IN addr;
	len = sizeof(addr);
	ZeroMemory(&m_addr, sizeof(SOCKADDR));
	getpeername(_sock, (SOCKADDR*)&m_addr, &len);
	printf("접속 %s %d\n", inet_ntoa(m_addr.sin_addr), ntohs(m_addr.sin_port));
}

CSocket::~CSocket()
{
}

/// <summary>
/// bytes가 전부 받아지지 않았을 때, 다시 호출된다.
/// </summary>
/// <returns></returns>
bool CSocket::WSASEND()
{
	int retval = 0;
	DWORD sendbytes, flags;
	flags = 0;
	WSABUF wsa_sendbuf;

	t_sendbuf* sendbuf = m_sendque.front();
	ZeroMemory(&s_overlap.overlapped, sizeof(OVERLAPPED));

	wsa_sendbuf.len = sendbuf->sendbytes - sendbuf->comp_sendbytes;
	wsa_sendbuf.buf = sendbuf->sendbuf + sendbuf->comp_sendbytes;

	retval = WSASend(m_sock, &wsa_sendbuf, 1, &sendbytes, flags, &s_overlap.overlapped, NULL);
	if (retval == SOCKET_ERROR)
	{
		if (WSAGetLastError() != ERROR_IO_PENDING)
		{
			err_display(L"WSASend()");
			return false;
		}
	}
	return true;
}

bool CSocket::WSASEND(char* _buf, int _size)
{
	t_sendbuf* sendbuf = new t_sendbuf(_buf, _size);
	m_sendque.push(sendbuf);

	if (m_sendque.size() == 1)
	{
		if (!WSASEND())
			return false;
	}
	return true;
}

bool CSocket::WSARECV()
{
	int retval = 0;
	DWORD recvbytes, flags;
	flags = 0;
	ZeroMemory(&r_overlap.overlapped, sizeof(OVERLAPPED));
	WSABUF wsa_recvbuf;

	wsa_recvbuf.len = t_recvbuf.recvbytes - t_recvbuf.comp_recvbytes;
	wsa_recvbuf.buf = t_recvbuf.recvbuf + t_recvbuf.comp_recvbytes;

	retval = WSARecv(m_sock, &wsa_recvbuf, 1, &recvbytes, &flags, &r_overlap.overlapped, NULL);
	if (retval == SOCKET_ERROR)
	{
		if (WSAGetLastError() != ERROR_IO_PENDING)
		{
			err_display(L"WSARecv()");
			return false;
		}
	}
	return true;
}

SOC CSocket::Complate_Recv(int _completebyte) // complate 
{
	t_recvbuf.comp_recvbytes += _completebyte;

	if (t_recvbuf.comp_recvbytes == t_recvbuf.recvbytes)
	{
		if (!t_recvbuf.is_flag)
		{
			memcpy(&t_recvbuf.recvbytes, t_recvbuf.recvbuf, sizeof(int));
			ZeroMemory(t_recvbuf.recvbuf, BUFSIZE);
			t_recvbuf.comp_recvbytes = 0;
			t_recvbuf.is_flag = true;
			return SOC::SOC_FALSE;
		}

		t_recvbuf.comp_recvbytes = 0;
		t_recvbuf.recvbytes = 0;
		t_recvbuf.is_flag = false;
		return SOC::SOC_TRUE;
	}
	else
	{
		return SOC::SOC_FALSE;
	}

}

SOC CSocket::Complate_Send(int _completebyte)
{
	/// <summary>
	/// 첫번째 sendbuf를 가져와, bytes가 전부 다 받아졌는지 확인한다.
	/// 다 받아졌다면 que를 pop하고, 다 받아지지 않은 경우 다시 WSAsend를 보낸다.
	/// </summary>
	/// <param name="_completebyte"></param>
	/// <returns></returns>
	t_sendbuf* sendbuf = m_sendque.front();
	sendbuf->comp_sendbytes += _completebyte;
	if (sendbuf->comp_sendbytes == sendbuf->sendbytes)
	{
		sendbuf->comp_sendbytes = sendbuf->sendbytes = 0;
		ZeroMemory(sendbuf->sendbuf, BUFSIZE);
		m_sendque.pop();
		return SOC::SOC_TRUE;
	}
	else
	{
		return SOC::SOC_FALSE;
	}
}

void CSocket::CreateTCPSocket()
{
	m_sock = socket(AF_INET, SOCK_STREAM, 0);
	if (m_sock == INVALID_SOCKET)
	{
		err_quit(L"socket()");
	}
}

SOCKET CSocket::Accept()
{
	SOCKADDR_IN _addr;
	int _addrlen = sizeof(_addr);
	ZeroMemory(&_addr, sizeof(SOCKADDR_IN));
	SOCKET sock = accept(m_sock, (SOCKADDR*)&_addr, &_addrlen);
	return sock;
}

void CSocket::BindAndListen()
{
	int retval = 0;

	retval = bind(m_sock, (SOCKADDR*)&m_addr, sizeof(m_addr));
	if (retval == SOCKET_ERROR)
	{
		err_quit(L"bind()");
	}
	retval = listen(m_sock, SOMAXCONN);
	if (retval == SOCKET_ERROR)
	{
		err_quit(L"listen()");
	}
}

void CSocket::SendQuePush(t_sendbuf* _sendbuf)
{
	m_sendque.push(_sendbuf);
}

void CSocket::SendQuePop()
{
	m_sendque.pop();
}

void CSocket::DelaySend()
{
	if (m_sendque.size() != 0)
	{
		WSASEND();
	}
}
