#pragma once
//#include "CLock.h"
class CLock;
template<typename T>
class CLock_Guard
{
public:
	CLock_Guard(T* a);
	~CLock_Guard();
	//CLock_Guard(T* a)
	//	: m_key(a)
	//{
	//	printf("생성\n");
	//	a->lock();
	//}
	//~CLock_Guard()
	//{
	//	m_key->unlock();
	//}

private:
	T* m_key;

};
/* 이런식으로 쓰면 됨.
class Man
{

	CLock c;
	함수A
	{
	LockGaurd a(c);
	}
};
*/
