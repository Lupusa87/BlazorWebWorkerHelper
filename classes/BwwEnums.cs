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


        public enum BwsTransportType
        {
            Text,
            ArrayBuffer,
            Blob,
            //   ArrayBufferView,
        }


        public enum BwwTransportType
        {
            Text,
            Binary,
        }


        public enum BCommandType
        {
            send,
            WwDisconnect,
            WwSync,
            WsAdd,
            WsRemove,
            WsSetBinaryType,
            MultyPurposeItem1,
            MultyPurposeItem2,
            MultyPurposeItem3,
        }

        public enum BResultType
        {
            ActualMessage,
            StateChange,
            MultyPurposeItem1,
            MultyPurposeItem2,
        }
    }
}
