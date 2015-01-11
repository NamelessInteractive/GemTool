module NamelessInteractive.GemTool.Device.FileIOApiDeclarations

#nowarn "9"

open System.Runtime.InteropServices

[<Struct; StructLayout(LayoutKind.Sequential)>]
type OVERLAPPED = 
    val mutable Internal: int
    val mutable InternalHigh: int
    val mutable Offset: int
    val mutable OffsetHigh: int
    val mutable hEvent: int

[<Struct; StructLayout(LayoutKind.Sequential)>]
type SECURITY_ATTRIBUTES =
    val mutable nLength: int
    val mutable lpSecurityDescriptor: int
    val mutable bInheritHandle: int

// Fields
[<Literal>] 
let FILE_FLAG_OVERLAPPED : int = 1073741824;
[<Literal>] 
let FILE_SHARE_READ : int16 = 1s;
[<Literal>]
let FILE_SHARE_WRITE : int16= 2s;
[<Literal>]
let GENERIC_READ : int = -2147483648;
[<Literal>]
let GENERIC_WRITE : int = 1073741824;
[<Literal>]
let INVALID_HANDLE_VALUE : int = -1;
[<Literal>]
let OPEN_EXISTING : int16 = 3s;
[<Literal>]
let WAIT_OBJECT_0 : int16 = 0s;
[<Literal>]
let WAIT_TIMEOUT : int= 258;

[<DllImport("kernel32.dll")>]
extern int CancelIO(int hFile)
[<DllImport("kernel32.dll")>]
extern bool CloseHandle(int hObject)
[<DllImport("kernel32.dll")>]
extern int CreateEvent(SECURITY_ATTRIBUTES* SecurityAttributes, int bManualReset, int bInitialState, string lpName)
[<DllImport("kernel32.dll", CharSet=CharSet.Auto)>]
extern int CreateFile(string lpFileName, int dwDesiredAccess, int dwShareMode, SECURITY_ATTRIBUTES* lpSecurityAttributes, int dwCreationDisposition, int dwFlagsAndAtrributes, int hTemplateFile)
[<DllImport("kernel32.dll")>]
extern int ReadFile(int hFile, byte* lpBuffer, int numberOfBytesToRead, int* lpNumberOfBytesRead, OVERLAPPED* lpOverlapped)
[<DllImport("kernel32.dll")>]
extern int WaitForSingleObject(int hHandle, int dwMilliseconds)
[<DllImport("kernel32.dll")>]
extern bool Writefile(int hFile, byte* lpBuffer, int nNumberOfBytesToWrite, int* lpNumberOfBytesWritten, int lpOverllapped)
