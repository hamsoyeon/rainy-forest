#pragma once
#include "CState.h"

class CStageState :
    public CState
{
public:
	CStageState(CSession* _session) :CState(_session) {}
	virtual void Recv() final;
	virtual void Send() final;

};

