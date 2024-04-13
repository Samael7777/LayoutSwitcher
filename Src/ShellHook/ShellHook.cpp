// ReSharper disable CppInconsistentNaming

#pragma comment(linker, "/MERGE:.pdata=.data")
#pragma comment(linker, "/MERGE:.rdata=.data")
//#pragma comment(linker, "/SECTION:.text,EWR") //Sample!

#include "ShellHook.h"

struct SharedData
{
	HWND MessageWindow;
	HHOOK Hook;
};

DWORD ShellHookMsg = 0;
HMODULE ModuleHandle = nullptr;

HANDLE MapObject = nullptr;
SharedData* Shared = nullptr;

LRESULT ShellHookProc(int code, WPARAM w_param, LPARAM l_param);

namespace SharedMem
{
	bool CreateSharedDataMapping()
	{
		MapObject = CreateFileMapping(
			INVALID_HANDLE_VALUE,
			nullptr,
			PAGE_READWRITE,
			0,
			sizeof(SharedData),
			MemMapFilename);
		if (MapObject == nullptr) return false;
		Shared = static_cast<SharedData*>(MapViewOfFile(
			MapObject,
			FILE_MAP_WRITE,
			0,
			0,
			0));
		if (Shared == nullptr) return false;
		return true;
	}

	void CloseMappedFiles()
	{
		if (Shared != nullptr)
		{
			UnmapViewOfFile(Shared);
			Shared = nullptr;
		}
		if (MapObject != nullptr)
		{
			CloseHandle(MapObject);
			MapObject = nullptr;
		}
	}
}


DWORD GetShellHookMessageCode()
{
	return ShellHookMsg;
}

HHOOK RegisterShellHook(HWND messageWnd)
{
	Shared->MessageWindow = messageWnd;
	Shared->Hook = SetWindowsHookEx(WH_SHELL, (HOOKPROC)ShellHookProc, ModuleHandle, 0);
	return Shared->Hook;
}

bool UnregisterShellHook()
{
	return Shared->Hook != nullptr ? UnhookWindowsHookEx(Shared->Hook) : true;
}

LRESULT ShellHookProc(int code, WPARAM w_param, LPARAM l_param)
{
	if (code == HSHELL_LANGUAGE && Shared->MessageWindow != nullptr)
	{
		PostMessage(Shared->MessageWindow, ShellHookMsg, static_cast<WPARAM>(code), l_param);
	}
	return CallNextHookEx(Shared->Hook, code, w_param, l_param);
}

BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved
)
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
		DisableThreadLibraryCalls(hModule);
		ModuleHandle = hModule;
		ShellHookMsg = RegisterWindowMessage(ShellHookMsgName);
		SharedMem::CreateSharedDataMapping();
		break;
    case DLL_PROCESS_DETACH:
		SharedMem::CloseMappedFiles();
		break;
    case DLL_THREAD_ATTACH:  // NOLINT(bugprone-branch-clone)
    case DLL_THREAD_DETACH:
		break;
	default: 
		break;
    }
    return TRUE;
}


