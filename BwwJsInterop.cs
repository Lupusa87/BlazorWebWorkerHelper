using Microsoft.JSInterop;
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


        public static Task<bool> WwSend(string WwID, BWorkerType WsType, string WwMessage)
        {
            if (WsType == BWorkerType.shared)
            {
                return JSRuntime.Current.InvokeAsync<bool>("BwwJsFunctions.WwSendShared", new { WwID, WwMessage });
            }

            return JSRuntime.Current.InvokeAsync<bool>("BwwJsFunctions.WwSendDedicated", new { WwID, WwMessage });



        }

        public static Task<bool> WwClose(string WsID)
        {

            return JSRuntime.Current.InvokeAsync<bool>("BwwJsFunctions.WwClose", WsID);
        }

        public static Task<bool> WwRemove(string WsID)
        {
            return JSRuntime.Current.InvokeAsync<bool>("BwwJsFunctions.WwRemove", WsID);
        }

    }
}
