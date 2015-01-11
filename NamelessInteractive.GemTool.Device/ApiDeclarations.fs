module NamelessInteractive.GemTool.Device.HidApiDeclarations

open System
open System.Runtime.InteropServices

[<Literal>]
let HidP_Feature = 2s
[<Literal>]
let HidP_Input = 0s
[<Literal>]
let HidP_Output = 1s

#nowarn "9"
[<Struct; StructLayout(LayoutKind.Sequential)>]
type HIDD_ATTRIBUTES =
    val mutable Size : int
    val mutable VendorId: int16
    val mutable ProductId: int16
    val mutable VersionNumber: int16

[<Struct; StructLayout(LayoutKind.Sequential)>]
type HIDP_CAPS =
    val mutable Usage: int16
    val mutable UsagePage: int16
    val mutable InputReportByteLength: int16
    val mutable OuputReportByteLength: int16
    val mutable FeatureReportByteLength: int16
    [<MarshalAs(UnmanagedType.ByValArray, SizeConst=17)>]
    val mutable Reserved: int16[]
    val mutable NumberLinkCollectionNodes: int16 
    val mutable NumberInputButtonCaps: int16
    val mutable NumberInputValueCaps: int16
    val mutable NumberInputDataIndicees: int16
    val mutable NumberOutputButtonCaps: int16
    val mutable NumberOutputValueCaps: int16
    val mutable NumberOutputDataIndices: int16
    val mutable NumberFeatureButtonCaps: int16
    val mutable NumberFeatureValueCpas: int16
    val mutable NumberFeatureDataIndices: int16

[<Struct; StructLayout(LayoutKind.Sequential)>]
type HidP_Value_Caps =
    val mutable UsagePage: int16
    val mutable ReportID : byte
    val mutable IsAlias: int
    val mutable BitField: int16
    val mutable LinkCollection: int16
    val mutable LinkUsage: int16
    val mutable LinkUsagePage: int16
    val mutable IsRange: int
    val mutable IsStringRange: int
    val mutable IsDesignatorRange: int
    val mutable IsAbsolute: int
    val mutable HasNull:int
    val mutable Reserved: byte
    val mutable BitSize: int16
    val mutable ReportCount: int16
    val mutable Reserved2: int16
    val mutable Reserved3: int16
    val mutable Reserved4: int16
    val mutable Reserved5: int16
    val mutable Reserved6: int16
    val mutable LogicalMin: int
    val mutable LogicalMax: int
    val mutable PhysicalMin: int
    val mutable PhysicalMax: int
    val mutable UsageMin: int16
    val mutable UsageMax: int16
    val mutable StringMin: int16
    val mutable StringMax: int16
    val mutable DesignatorMin: int16
    val mutable DesignatorMax: int16
    val mutable DataIndexMin: int16
    val mutable DataIndexMax: int16

[<DllImport("hid.dll")>]
extern bool HidD_FlushQueue(int HidDeviceObject) 
[<DllImport("hid.dll")>]
extern bool HidD_FreePreparsedData(IntPtr& PreparsedData)
[<DllImport("hid.dll")>]
extern bool HidD_GetAttributes(int HidDeviceObject, HIDD_ATTRIBUTES& Attributes)
[<DllImport("hid.dll")>]
extern bool HidD_GetFeature(int HidDeviceObject, byte* lpReportBuffer, int ReportBufferLength)
[<DllImport("hid.dll")>]
extern void HidD_GetHidGuid(Guid* HidGuid)
[<DllImport("hid.dll")>]
extern bool HidD_GetInputReport(int HidDeviceObject, byte* lpReportBuffer, int ReportBufferLength)
[<DllImport("hid.dll")>]
extern bool HidD_GetNumInputbuffers(int HidDeviceObject, int* NumberBuffers)
[<DllImport("hid.dll")>]
extern bool HidD_GetPreparsedData(int HidDeviceObject, nativeint* PreparsedData)
[<DllImport("hid.dll")>]
extern bool HidD_SetFeature(int HidDevcieObject, byte* lpReportBuffer, int ReportBufferLength)
[<DllImport("hid.dll")>]
extern bool HidD_SetNumInputBuffers(int HidDeviceObject, int NumBuffers)
[<DllImport("hid.dll")>]
extern bool HidD_SetOutputReport(int HidDeviceObject, byte* lpReportBuffer, int ReportBufferLength)
[<DllImport("hid.dll")>]
extern int HidP_GetCaps(IntPtr* PreparsedData,  HIDP_CAPS& Capabilities)
[<DllImport("hid.dll")>]
extern int HidP_GetValueCaps(int16 ReportType, byte* ValueCaps, int16* ValueCapsLength, IntPtr* PreparsedData)

