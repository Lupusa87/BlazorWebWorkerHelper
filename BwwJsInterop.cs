using Microsoft.JSInterop;
using Mono.WebAssembly.Interop;
using System.Threading.Tasks;
using static BlazorWebWorkerHelper.classes.BwwEnums;


namespace BlazorWebWorkerHelper
{
    public class BwwJsInterop
    {

        public static Task<string> Alert(string message)
        {
            return JSRuntime.Current.InvokeAsync<string>(
                "BwwJsFunctions.alert",
                message);
        }


        public static Task<string> Prompt(string message)
        {
            return JSRuntime.Current.InvokeAsync<string>(
                "BwwJsFunctions.showPrompt",
                message);
        }


        public static Task<bool> WwAdd(string WwID, string WwUrl, BWorkerType WsType, DotNetObjectRef dotnethelper)
        {

            if (WsType == BWorkerType.shared)
            {  
               return JSRuntime.Current.InvokeAsync<bool>("BwwJsFunctions.WwAddShared", new { WwID, WwUrl, dotnethelper });    
            }

            return JSRuntime.Current.InvokeAsync<bool>("BwwJsFunctions.WwAddDedicated", new { WwID, WwUrl, dotnethelper });

        }


        public static Task<bool> WwSend(string WwID, BWorkerType WwType, BCommandType WCommandType, string WwMessage)
        {


            if (WwType == BWorkerType.shared)
            {
                return JSRuntime.Current.InvokeAsync<bool>("BwwJsFunctions.WwSendShared", new { WwID, WCommandType, WwMessage });
            }

            return JSRuntime.Current.InvokeAsync<bool>("BwwJsFunctions.WwSendDedicated", new { WwID, WCommandType, WwMessage });



        }


        public static bool WwSend(string WwID, BWorkerType WwType, BCommandType WCommandType, byte[] WsMessage)
        {
            if (WwType == BWorkerType.shared)
            {
                if (JSRuntime.Current is MonoWebAssemblyJSRuntime mono)
                {
                    return mono.InvokeUnmarshalled<string, short, byte[], bool>(
                        "BwwJsFunctions.WwSendSharedBinary",
                        WwID,
                        (short)WCommandType,
                        WsMessage);
                }
            }
            else
            {
                if (JSRuntime.Current is MonoWebAssemblyJSRuntime mono)
                {
                    return mono.InvokeUnmarshalled<string, short, byte[], bool>(
                        "BwwJsFunctions.WwSendDedicatedBinary",
                        WwID,
                        (short)WCommandType,
                        WsMessage);
                }
            }

            

            return false;

        }


        public static Task<bool> WwRemove(string WsID)
        {
            return JSRuntime.Current.InvokeAsync<bool>("BwwJsFunctions.WwRemove", WsID);
        }

    }
}
