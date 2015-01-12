module NamelessInteractive.GemTool.Device.DeviceManagementApiDeclarations

open System.Runtime.InteropServices
open System

#nowarn "9"

[<Literal>]
let DBT_DEVICEARRIVAL = 32768
[<Literal>]
let DBT_DEVICEREMOVECOMPLETE = 32772
[<Literal>]
let DBT_DEVTYP_DEVICEINTERFACE = 5
[<Literal>]
let DBT_DEVTYP_HANDLE = 6
[<Literal>]
let DEVICE_NOTIFY_ALL_INTERFACE_CLASSES = 4
[<Literal>]
let DEVICE_NOTIFY_SERVICE_HANDLE = 1
[<Literal>]
let DEVICE_NOTIFY_WINDOW_HANDLE = 0
[<Literal>]
let WM_DEVICECHANGE = 537
[<Literal>]
let DIGCF_DEVICEINTERFACE = 16s
[<Literal>]
let DIGCF_PRESENT = 2s

[<Struct; StructLayout(LayoutKind.Sequential)>]
type DEV_BROADCAST_DEVICE_INTERFACE =
    val mutable dbcc_size: int
    val mutable dbcc_devicetype: int
    val mutable dbcc_reserved: int
    val mutable dbcc_classguid: Guid
    val mutable dbcc_name: int16

[<Struct; StructLayout(LayoutKind.Sequential)>]
type DEV_BROADCAST_DEVICE_INTERFACE_1 =
    val mutable dbcc_size: int
    val mutable dbcc_devicetype: int
    val mutable dbcc_reserved: int
    [<MarshalAs(UnmanagedType.ByValArray, SizeConst=16, ArraySubType=UnmanagedType.U1)>]
    val mutable dbcc_classguid: byte[]
    [<MarshalAs(UnmanagedType.ByValArray, SizeConst=255)>]
    val mutable dbcc_name: char[]

[<Struct; StructLayout(LayoutKind.Sequential)>]
type DEV_BROADCAST_HANDLE =
    val mutable dbch_size: int
    val mutable dbch_devicetype: int
    val mutable dbch_reserved: int
    val mutable dbch_handle: int
    val mutable dbch_hdevnotify: int

[<Struct;StructLayout(LayoutKind.Sequential)>]
type DEV_BROADCAST_HDR =
    val mutable dbch_size: int
    val mutable dbch_devicetype: int
    val mutable dbch_reserved: int

[<Struct;StructLayout(LayoutKind.Sequential)>]
type SP_DEVICE_INTERFACE_DATA =
    val mutable cbSize: int
    val mutable InterfaceClassGuid: Guid
    val mutable Flags: int
    val mutable Reserved: int

[<Struct;StructLayout(LayoutKind.Sequential)>]
type SP_DEVICE_INTERFACE_DETAIL_DATA =
    val mutable cbSize: int 
    val mutable DevicePath: string

[<Struct;StructLayout(LayoutKind.Sequential)>]
type SP_DEVINFO_DATA =
    val mutable cbSize: int
    val mutable ClassGuid: Guid
    val mutable DevInst: int
    val mutable Reserved: int

[<DllImport("user32.dll",CharSet=CharSet.Auto)>]
extern IntPtr RegisterDeviceNotification(IntPtr hRecipient, IntPtr NotificationFilter, int Flags)
[<DllImport("setupapi.dll")>]
extern int SetupDiCreateDeviceInfoList(Guid* ClassGuid, int hwndParent)
[<DllImport("setupapi.dll")>]
extern int SetupDiDestroyDeviceInfoList(IntPtr DeviceInfoSet)
[<DllImport("setupapi.dll")>]
extern bool SetupDiEnumDeviceInterfaces(IntPtr DeviceInfoSet, int DeviceInfoData, Guid* InterfaceClassGuid, int MemberIndex, SP_DEVICE_INTERFACE_DATA* DeviceInterfaceData)
[<DllImport("setupapi.dll")>]
extern bool SetupDiGetClassDevs(Guid ClassGuid, string Enumerator, int hwndParent, int Flags)
[<DllImport("setupapi.dll", CharSet=CharSet.Auto)>]
extern bool SetupDiGetDeviceInterfaceDetail(IntPtr DeviceInfoSet, SP_DEVICE_INTERFACE_DATA* DeviceInterfaceData, IntPtr DeviceInterfaceDetailData, int DeviceInterfaceDetailDataSize, int* RequiredSize, IntPtr DeviceInfoData)
[<DllImport("user32.dll")>]
extern bool UnregisterDeviceNotification(IntPtr Handle)