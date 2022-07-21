#include "pch.h"
#include "CLockGuard.h"
#include "CLock.h"

template<typename T>
CLock_Guard<T>::CLock_Guard(T* a)
	: m_key(a)
{
	a->lock();
}

template<typename T>
CLock_Guard<T>::~CLock_Guard()
{
	m_key->unlock();
}