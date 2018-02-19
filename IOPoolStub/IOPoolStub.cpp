//
#include "stdafx.h"
#include "IOPoolStub.h"
#include "windows.h"
#include <iostream> 
using namespace std;

HANDLE CreateIoCompletionPortStub(HANDLE fileHandle, HANDLE existing, DWORD completionKey, DWORD concurency) {
	ULONG_PTR completionKey_PTR = (ULONG_PTR)completionKey;
	return CreateIoCompletionPort(fileHandle, existing, completionKey_PTR, concurency);
}

bool GetQueuedStub(HANDLE port, _Out_ LPDWORD lpNumberOfBytes, _Out_  LPDWORD lpCompletionKey, _Out_ LPWORD handle, DWORD wMilliseconds) {

	PULONG_PTR completionKey_PTR = (PULONG_PTR)lpCompletionKey;
	LPOVERLAPPED overlaped;


	if (true == GetQueuedCompletionStatus(port, lpNumberOfBytes, completionKey_PTR, &overlaped, wMilliseconds) )
	{
		cout << (DWORD)overlaped->hEvent << endl;
		*handle = (DWORD)overlaped->hEvent;
		return true;
	}
	else {
		return false;
	}
}

bool bdmabsmfnabfasmnbfa() {
	return true;
}
//PTP_IO createPoolIO() {
	//LPVOID PTPStruct = CreateThreadpoolIo();
//}


