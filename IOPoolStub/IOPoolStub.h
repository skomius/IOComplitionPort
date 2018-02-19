#pragma once

#ifdef IOPOOLSTUB_EXPORTS  
#define IOPOOLSTUB_API __declspec(dllexport)   
#else  
#define IOPOOLSTUB_API __declspec(dllimport)   
#endif  


#include "stdafx.h"
#include "windows.h"
#include <iostream> 
using namespace std;
extern "C" { 
IOPOOLSTUB_API HANDLE CreateIoCompletionPortStub(HANDLE, HANDLE, DWORD, DWORD);
IOPOOLSTUB_API bool GetQueuedStub(HANDLE,LPDWORD, LPDWORD, LPWORD, DWORD);
IOPOOLSTUB_API bool bdmabsmfnabfasmnbfa(); }
                               

                               
