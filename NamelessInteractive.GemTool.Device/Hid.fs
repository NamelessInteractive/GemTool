namespace NamelessInteractive.GemTool.Device

open NamelessInteractive.GemTool.Device.HidApiDeclarations
open NamelessInteractive.GemTool.Device.FileIOApiDeclarations
open System

type ReadResponse = 
    {
        DeviceDetected: bool
        Buffer: byte[]
        Success: bool
    }

[<AbstractClass>]
type internal DeviceReport() =
    [<DefaultValue>]
    val mutable HIDHandle : int
    [<DefaultValue>]
    val mutable DeviceDetected: bool
    [<DefaultValue>]
    val mutable ReadHandle: int
    [<DefaultValue>]
    val mutable Result: int
    abstract member ProtectedRead : int * int * int -> ReadResponse option
    member this.Read(readHandle: int, hidHandle: int, writeHandle: int) : ReadResponse option =
        try
            this.ProtectedRead(readHandle,hidHandle,writeHandle)
        with 
        | ex -> 
            printfn "%s" ex.Message
            None

type internal InFeatureReport() =
    inherit DeviceReport()
    override this.ProtectedRead(readHandle: int, hidHandle: int, writeHandle: int) =
        try
            let mutable buffer = Array.zeroCreate<byte>(256)
            let success = HidD_GetFeature(hidHandle,&&((buffer).[0]), buffer.GetUpperBound(1) + 1)
            {
                DeviceDetected = true
                Buffer=  buffer
                Success = success
            } |> Some
        with
        | ex -> 
            printfn "%s" ex.Message
            None

type internal InputReport() =
    inherit DeviceReport()
    let mutable ReadyForOverlappedTransfer = false
    member this.PrepareForOverlappedTransfer() =
        try
            let mutable securityAttributes = new SECURITY_ATTRIBUTES()
            securityAttributes.lpSecurityDescriptor <- 0
            securityAttributes.bInheritHandle <- 0
            securityAttributes.nLength <- sizeof<SECURITY_ATTRIBUTES>
            let eventObject = FileIOApiDeclarations.CreateEvent(&&securityAttributes, 0,-1,"")
            let mutable hidOverlapped = new OVERLAPPED()
            hidOverlapped.Offset <- 0
            hidOverlapped.OffsetHigh <- 0
            hidOverlapped.hEvent <- eventObject
            ReadyForOverlappedTransfer <- true
            (hidOverlapped, eventObject) |> Some

        with
        | ex -> 
            printfn "%s" ex.Message
            None

    member this.CancelTransfer(readHandle: int, hidHandle: int, writeHandle: int) : unit =
        try
            ignore(FileIOApiDeclarations.CancelIO(readHandle))
            if (hidHandle <> 0) then
                ignore(FileIOApiDeclarations.CloseHandle(hidHandle))
            if readHandle <> 0 then
                ignore(FileIOApiDeclarations.CloseHandle(readHandle))
            if writeHandle<>0 then
                ignore(FileIOApiDeclarations.CloseHandle(writeHandle))
        with
        | ex -> printfn "%s" ex.Message
        
    override this.ProtectedRead(readHandle, hidHandle, writeHandle) =
        try
            let overlappedResponse = 
                if not ReadyForOverlappedTransfer then
                    this.PrepareForOverlappedTransfer()
                else
                    None
            let mutable overlapped, eventObject =
                match overlappedResponse with
                | Some(overlapped,eventObject) -> overlapped, eventObject
                | None -> failwith "No overlapped response"
            let mutable buffer = Array.zeroCreate<byte>(256)
            let mutable bytesRead = 0
            let mutable success = false
            let mutable deviceDetected = true
            let response = FileIOApiDeclarations.ReadFile(readHandle, &&buffer.[0], buffer.GetUpperBound(1) + 1, &&bytesRead, &&overlapped)
            match response with 
            | 0 -> 
                success <- true
            | 258 -> 
                this.CancelTransfer(readHandle,hidHandle,writeHandle)
                success <- false
                deviceDetected <- false
            | _ ->    
                this.CancelTransfer(readHandle,hidHandle,writeHandle)
                success <- false
                deviceDetected <- false
            {
                ReadResponse.Buffer = buffer
                ReadResponse.Success = success
                ReadResponse.DeviceDetected = deviceDetected
            } |> Some
        with
        | ex -> 
            printfn "%s" ex.Message
            None

type internal InputReportViaControlTransfer() =
    inherit DeviceReport()
    override this.ProtectedRead(readHandle, hidHandle, writeHandle) =
        try
            let mutable buffer = Array.zeroCreate<byte>(256)
            let success = HidD_GetInputReport(hidHandle,&&((buffer).[0]), buffer.GetUpperBound(1) + 1)
            {
                DeviceDetected = true
                Buffer=  buffer
                Success = success
            } |> Some
        with
        | ex -> 
            printfn "%s" ex.Message
            None

[<AbstractClass>]
type internal HostReport() =
    abstract member ProtectedWrite: deviceHandle: int * reportBuffer: byte[] -> bool
    member this.Write(reportBuffer: byte[] , deviceHandle: int) : bool =
        try
            this.ProtectedWrite(deviceHandle, reportBuffer)
        with
        | ex -> 
            printfn "%s" ex.Message
            false

type internal OutFeatureReport() =
    inherit HostReport()
    override this.ProtectedWrite(deviceHandle,reportBuffer) =
        try
            HidD_SetFeature(deviceHandle, &&reportBuffer.[0], reportBuffer.GetUpperBound(1) + 1)
        with
        | ex -> 
            printfn "%s" ex.Message
            false

type internal OutReport() =
    inherit HostReport()
    override this.ProtectedWrite(deviceHandle,reportBuffer) =
        try
            let mutable lpNumberOfBytesWritten = 0
            let response = FileIOApiDeclarations.Writefile(deviceHandle, &&reportBuffer.[0], reportBuffer.GetUpperBound(1)+1, &&lpNumberOfBytesWritten,0)
            if response then
                response
            else
                if deviceHandle <> 0 then
                    FileIOApiDeclarations.CloseHandle(deviceHandle) |> ignore
                response
        with
        | ex ->
            printfn "%s" ex.Message
            false

type internal OutputReportViaControlHandle() =
    inherit HostReport()
    override this.ProtectedWrite(deviceHandle,reportBuffer) =
        try
            HidD_SetOutputReport(deviceHandle,&&reportBuffer.[0], reportBuffer.GetUpperBound(1) + 1)
        with
        | ex ->
            printfn "%s" ex.Message
            false

type Hid() =
    member this.FlushQueue(hidHandle: int) : bool =
        try
            HidApiDeclarations.HidD_FlushQueue(hidHandle)
        with
        | ex ->
            printfn "%s" ex.Message
            false

    member this.GetDeviceCapabilities(hidHandle: int) : HIDP_CAPS =
        let inArray = Array.zeroCreate<byte>(30)
        let buffer = Array.zeroCreate<byte>(1024)
        let mutable caps = new HIDP_CAPS()
        try
            let mutable ptr = new IntPtr()
            let response = HidApiDeclarations.HidD_GetPreparsedData(hidHandle, &&ptr)
            if (HidApiDeclarations.HidP_GetCaps(&&ptr, &caps) <> 0) then
                let num = HidApiDeclarations.HidP_GetValueCaps(0s,&&buffer.[0], &&caps.NumberInputValueCaps, &&ptr)
                ignore (HidApiDeclarations.HidD_FreePreparsedData(&ptr))
            caps

        with
        | ex -> 
            printfn "%s" ex.Message
            caps

    member this.GetHIDUsage(caps: HIDP_CAPS) : string=
        try
            match (caps.UsagePage * 256s) + caps.Usage with
            | 258s -> "mouse"
            | 262s -> "keyboard"
            | _ -> ""
        with
        | ex ->
            printfn "%s" ex.Message
            ""

    member this.GetNumberOfInputBuffers(hidHandle: int) : bool * int =  
        try
            let mutable buffers = 0
            let response = HidApiDeclarations.HidD_GetNumInputbuffers(hidHandle,&&buffers)
            response, buffers
        with
        | ex -> 
            printfn "%s" ex.Message
            false, 0

    member this.SetNumberOfInputBuffers(hidHandle: int, numberBuffers: int) =   
        try
            HidApiDeclarations.HidD_SetNumInputBuffers(hidHandle,numberBuffers)
        with
        | ex ->
            printfn "%s" ex.Message
            false

type HidDevice() =
    let VendorId = 1240
    let ProductId = -1537
    let mutable DeviceDetected = false
    let mutable DeviceFirstAttached = false
    let Hid = new Hid()
