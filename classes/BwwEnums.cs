using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorWebWorkerHelper.classes
{
    public class BwwEnums
    {

        public enum BWorkerType
        {
            dedicated,
            shared,
        }

        public enum BwwMessageType
        {
            send,
            received,
        }

        public enum BwwState
        {
            Undefined,
            Open,
            Close,
            Error,
        }
    }
}
