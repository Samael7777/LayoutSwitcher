#pragma once
#include <windows.h>

#define DLL_EXPORT __declspec(dllexport)

#ifdef _M_X64
const LPCTSTR MemMapFilename = reinterpret_cast<LPCTSTR>(L"ShellHookSharedMemory{58ABC9EF}_X64");
#else
const LPCTSTR MemMapFilename = reinterpret_cast<LPCTSTR>(L"ShellHookSharedMemory{58ABC9EF}_X86");
#endif

inline const WCHAR* ShellHookMsgName = L"ShellHookMsg";

#ifdef __cplusplus
extern "C"
{
#endif
	DLL_EXPORT HHOOK RegisterShellHook(HWND messageWnd);
	DLL_EXPORT bool UnregisterShellHook();
	DLL_EXPORT DWORD GetShellHookMessageCode();
#ifdef __cplusplus
}
#endif
