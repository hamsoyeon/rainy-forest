#pragma once
class CProtocolMgr
{
public:
	static CProtocolMgr* GetInst();
	static void Create();
	static void Destroy();

	unsigned long GetMainProtocol(unsigned long _protocolmem);
	unsigned long GetSubProtocol(unsigned long _protocolmem);
	unsigned long GetDetailProtocol(unsigned long _protocolmem);

	void AddMainProtocol(unsigned long* _protocolmem, unsigned long _protocoltype);
	void AddSubProtocol(unsigned long* _protocolmem, unsigned long _protocoltype);
	void AddDetailProtocol(unsigned long* _protocolmem, unsigned long _protocoltype);

private:
	CProtocolMgr() {}
	~CProtocolMgr() {}
	static CProtocolMgr* m_instance;

	const int mainshift = 24;
	const int subshift = 16;
	const int byteshift = 8;

	const unsigned long or_mainbin = 0xff000000;
	const unsigned long or_subbin = 0x00ff0000;
	const unsigned long or_detailbin = 0x0000ffff;

};

