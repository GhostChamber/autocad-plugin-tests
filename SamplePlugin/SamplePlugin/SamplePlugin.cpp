#include "stdafx.h"

#include "SamplePlugin.h"
#include "tchar.h"
#include <aced.h>
#include <rxregsvc.h>

void initApp()
{
	// register a command with the AutoCAD command mechanism
	acedRegCmds->addCommand(_T("HELLOWORLD_COMMANDS"),
		_T("Hello"),
		_T("Namaste"),
		ACRX_CMD_TRANSPARENT,
		helloWorld);
}

void unloadApp()
{
	acedRegCmds->removeGroup(_T("HELLOWORLD_COMMANDS"));
}

void helloWorld()
{
	acutPrintf(_T("\nHello World!"));
}

extern "C" AcRx::AppRetCode
acrxEntryPoint(AcRx::AppMsgCode msg, void* pkt)
{
	switch (msg)
	{

	case AcRx::kInitAppMsg:
		acrxDynamicLinker->unlockApplication(pkt);
		acrxRegisterAppMDIAware(pkt);
		initApp();
		break;
	case AcRx::kUnloadAppMsg:
		unloadApp();
		break;
	default:
		break;

	}

	return AcRx::kRetOK;
}