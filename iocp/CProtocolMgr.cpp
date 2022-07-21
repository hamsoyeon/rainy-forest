#include "pch.h"
#include "CProtocolMgr.h"
CProtocolMgr* CProtocolMgr::m_instance = nullptr;

CProtocolMgr* CProtocolMgr::GetInst()
{
	return m_instance;
}

void CProtocolMgr::Create()
{
	if (m_instance == nullptr)
	{
		m_instance = new CProtocolMgr();
	}
}

void CProtocolMgr::Destroy()
{
	delete m_instance;
}

unsigned long CProtocolMgr::GetMainProtocol(unsigned long _protocolmem)
{
	return (_protocolmem & or_mainbin) >> mainshift;
}

unsigned long CProtocolMgr::GetSubProtocol(unsigned long _protocolmem)
{
	return (_protocolmem & or_subbin) >> subshift;
}

// 0000 0000 / 0000 0000 / 0000 0000  0000 0000 // main, sub, detail 
unsigned long CProtocolMgr::GetDetailProtocol(unsigned long _protocolmem)
{
	return (_protocolmem & 0x0000ffff) >> byteshift;
}

void CProtocolMgr::AddMainProtocol(unsigned long* _protocolmem, unsigned long _protocoltype)
{
	unsigned long main_protocol = _protocoltype << 24;
	*_protocolmem |= main_protocol;
	//_protocoltype = _protocoltype << 24;
	//*_protocolmem = *_protocolmem | _protocoltype;
}

void CProtocolMgr::AddSubProtocol(unsigned long* _protocolmem, unsigned long _protocoltype)
{
	unsigned long sub_protocol = _protocoltype << 16;
	*_protocolmem |= sub_protocol;
	//_protocoltype = _protocoltype << subshift;
	//*_protocolmem = *_protocolmem | _protocoltype;
}

void CProtocolMgr::AddDetailProtocol(unsigned long* _protocolmem, unsigned long _protocoltype)
{
	unsigned long detail_protocol = _protocoltype << 16;
	*_protocolmem |= detail_protocol;
	//*_protocolmem = *_protocolmem | _protocoltype;
}
