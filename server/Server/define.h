#pragma once

#define SINGLE(type) public:\
						static void Create();\
						static void Destroy();\
						static type* GetInst() { return m_instance; }\
					private:\
						static type* m_instance;\
						type();\
						~type();

#define BUFSIZE 4096

enum class IO_TYPE
{
	ACCEPT,
	SEND,
	RECV,
	DISCONNECT,
};

enum class SOC
{
	SOC_TRUE,
	SOC_FALSE,
};
