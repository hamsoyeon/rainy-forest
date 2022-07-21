#pragma once

#define HOST_IP "127.0.0.1"
#define USER "root"
#define PASSWORD "1234"
#define DATABASE "Yggdrasil"

enum class SOC
{
	SOC_TRUE,
	SOC_FALSE,
};

enum class MAIN_PROTOCOL
{
	NONE,
	LOGIN,
	LOBBY,
	MAX
};