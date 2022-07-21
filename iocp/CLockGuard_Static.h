#pragma once
class CLock;
template<typename T>
class CLockGuard_Static
{
public:
	class c_lock_guard
	{
		c_lock_guard()
		{
			m_key.lock();
		}
		~c_lock_guard()
		{
			m_key.unlock();
		}
	};

	friend c_lock_guard;

private:
	static CLock m_key;
};
//사용시 CLockGuard_Static<Class명>
