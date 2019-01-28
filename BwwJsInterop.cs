using Microsoft.JSInterop;
using Mono.WebAssembly.Interop;
using System;
using System.Reflection;
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


        public static Task<bool> WwAdd(string WwID, string WwUrl, string WwName, BWorkerType WsType, DotNetObjectRef dotnethelper)
        {

            if (WsType == BWorkerType.shared)
            {  
               return JSRuntime.Current.InvokeAsync<bool>("BwwJsFunctions.WwAddShared", new { WwID, WwUrl, WwName, dotnethelper });    
            }

            return JSRuntime.Current.InvokeAsync<bool>("BwwJsFunctions.WwAddDedicated", new { WwID, WwUrl, WwName, dotnethelper });

        }


        public static Task<bool> WwSend(string WwID, BWorkerType WwType, BCommandType WCommandType, string WwMessage, string AdditionalArgs)
        {


            if (WwType == BWorkerType.shared)
            {
                return JSRuntime.Current.InvokeAsync<bool>("BwwJsFunctions.WwSendShared", new { WwID, WCommandType, WwMessage, AdditionalArgs});
            }

            return JSRuntime.Current.InvokeAsync<bool>("BwwJsFunctions.WwSendDedicated", new { WwID, WCommandType, WwMessage, AdditionalArgs});



        }


        public static bool WwSend(string WwID, BWorkerType WwType, BCommandType WCommandType,byte[] WsMessage, string AdditionalArgs)
        {
            string bag = Json.Serialize(new { cmd = (short)WCommandType, args = AdditionalArgs });
            if (WwType == BWorkerType.shared)
            {

                if (JSRuntime.Current is MonoWebAssemblyJSRuntime mono)
                {

                    return mono.InvokeUnmarshalled<string, string, byte[], bool>(
                        "BwwJsFunctions.WwSendSharedBinary",
                        WwID,
                        bag,
                        WsMessage);
                }
            }
            else
            {
                
                if (JSRuntime.Current is MonoWebAssemblyJSRuntime mono)
                {
                    
                    return mono.InvokeUnmarshalled<string, string, byte[], bool>(
                        "BwwJsFunctions.WwSendDedicatedBinary",
                        WwID,
                        bag,
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
