using Microsoft.JSInterop;
using System.Text.Json;
using System.Threading.Tasks;
using static BlazorWebWorkerHelper.classes.BwwEnums;


namespace BlazorWebWorkerHelper
{
    public class BwwJsInterop
    {

        private IJSRuntime _JSRuntime;
        private IJSUnmarshalledRuntime _jsUnmarshalledRuntime;

        public BwwJsInterop(IJSRuntime jsRuntime)
        {
            _JSRuntime = jsRuntime;
            _jsUnmarshalledRuntime = jsRuntime as IJSUnmarshalledRuntime;
            StaticClass._jsUnmarshalledRuntime = _jsUnmarshalledRuntime;
        }

        public ValueTask<string> Alert(string message)
        {
            return _JSRuntime.InvokeAsync<string>(
                "BwwJsFunctions.alert",
                message);
        }


        public ValueTask<string> Prompt(string message)
        {
            return _JSRuntime.InvokeAsync<string>(
                "BwwJsFunctions.showPrompt",
                message);
        }


        public ValueTask<bool> WwAdd(string WwID, string WwUrl, string WwName, BWorkerType WsType, DotNetObjectReference<WebWorkerHelper> dotnethelper)
        {

            if (WsType == BWorkerType.shared)
            {  
               return _JSRuntime.InvokeAsync<bool>("BwwJsFunctions.WwAddShared", new { WwID, WwUrl, WwName, dotnethelper });    
            }

            return _JSRuntime.InvokeAsync<bool>("BwwJsFunctions.WwAddDedicated", new { WwID, WwUrl, WwName, dotnethelper });

        }


        public ValueTask<bool> WwSend(string WwID, BWorkerType WwType, BCommandType WCommandType, string WwMessage, string AdditionalArgs)
        {


            if (WwType == BWorkerType.shared)
            {
                return _JSRuntime.InvokeAsync<bool>("BwwJsFunctions.WwSendShared", new { WwID, WCommandType, WwMessage, AdditionalArgs});
            }

            return _JSRuntime.InvokeAsync<bool>("BwwJsFunctions.WwSendDedicated", new { WwID, WCommandType, WwMessage, AdditionalArgs});



        }


        public bool WwSend(string WwID, BWorkerType WwType, BCommandType WCommandType,byte[] WsMessage, string AdditionalArgs)
        {
            string bag = JsonSerializer.Serialize(new { cmd = (short)WCommandType, args = AdditionalArgs });
            if (WwType == BWorkerType.shared)
            {


                    return _jsUnmarshalledRuntime.InvokeUnmarshalled<string, string, byte[], bool>(
                        "BwwJsFunctions.WwSendSharedBinary",
                        WwID,
                        bag,
                        WsMessage);
               
            }
            else
            {

                    return _jsUnmarshalledRuntime.InvokeUnmarshalled<string, string, byte[], bool>(
                        "BwwJsFunctions.WwSendDedicatedBinary",
                        WwID,
                        bag,
                        WsMessage);
            }

        }


        public ValueTask<bool> WwRemove(string WsID)
        {
            return _JSRuntime.InvokeAsync<bool>("BwwJsFunctions.WwRemove", WsID);
        }

    }
}
