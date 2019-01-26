using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorWebWorkerHelper
{
    public static class StaticClass
    {
        public static List<WebWorkerHelper> webWorkerHelpers_List = new List<WebWorkerHelper>();
       
        [JSInvokable]
        public static byte[] AllocateArray(string length)
        {
            return new byte[int.Parse(length)];
        }


        [JSInvokable]
        public static void HandleMessageBinary(byte[] par_message, string wwID)
        {

            if (webWorkerHelpers_List.Any())
            {
                if (webWorkerHelpers_List.Any(x => x._id.Equals(wwID, StringComparison.InvariantCultureIgnoreCase)))
                {
                    webWorkerHelpers_List.Single(x => x._id.Equals(wwID, StringComparison.InvariantCultureIgnoreCase)).InvokeOnMessageBinary(par_message);
                }
            }
        }



    }
}
