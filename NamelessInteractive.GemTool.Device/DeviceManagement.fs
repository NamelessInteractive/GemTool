namespace NamelessInteractive.GemTool.Device

open System
open System.Runtime.InteropServices

type DeviceManagement() =
    member this.DeviceNameMatch(message: System.Windows.Forms.Message, devicePathName: string) =
        try
            let mutable structure = DeviceManagementApiDeclarations.DEV_BROADCAST_DEVICE_INTERFACE_1()
            let devBroadcastHdr = DeviceManagementApiDeclarations.DEV_BROADCAST_HDR()
            Marshal.PtrToStructure(message.LParam,devBroadcastHdr)
            if devBroadcastHdr.dbch_devicetype = 5 then
                let length = int (Math.Round((float devBroadcastHdr.dbch_size) - 32.0 / 2.0))
                structure.dbcc_name <- Array.zeroCreate<char>(length + 1)
                Marshal.PtrToStructure(message.LParam, structure)
                let res = String(structure.dbcc_name, 0 ,length)
                String.Compare(res, devicePathName, true) = 0
            else
                false
        with
        | ex -> 
            printfn "%s" ex.Message
            false



