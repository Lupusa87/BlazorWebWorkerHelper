using BlazorWebWorkerHelper.classes;
using Microsoft.JSInterop;
using Mono.WebAssembly.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;

namespace BlazorWebWorkerHelper
{
    public static class StaticClass
    {
        public static List<WebWorkerHelper> webWorkerHelpers_List = new List<WebWorkerHelper>();

        public static MonoWebAssemblyJSRuntime monoWebAssemblyJSRuntime = new MonoWebAssemblyJSRuntime();

        [JSInvokable]
        public static void AllocateArray(int length, string a, string a2)
        {
           
            byte[] b = new byte[length];


            monoWebAssemblyJSRuntime.InvokeUnmarshalled<byte[], bool>("BwwJsFunctions.GetBinaryData", b);

            HandleMessageBinary(b, a, a2);

        }


        [JSInvokable]
        public static void HandleMessageBinary(byte[] par_message, string wwID, string par_bag)
        {

            if (webWorkerHelpers_List.Any())
            {
                if (webWorkerHelpers_List.Any(x => x._id.Equals(wwID, StringComparison.InvariantCultureIgnoreCase)))
                {
                    webWorkerHelpers_List.Single(x => x._id.Equals(
                        wwID, StringComparison.InvariantCultureIgnoreCase)
                        ).InvokeOnMessageBinary(par_message, par_bag);
                }
            }
        }



    }

}