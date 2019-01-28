using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static BlazorWebWorkerHelper.classes.BwwEnums;

namespace BlazorWebWorkerHelper.WsClasses
{
    public class BWebSocket
    {
        public ushort bWebSocketID { get; set; }
        public string id { get; set; }
        public string url { get; set; }
        public BwwState state { get; set; }
        public BwsTransportType bwsTransportType { get; private set; } = BwsTransportType.Text;
        private string ParentID { get; set; }

        public BWebSocket(string Par_ParentID)
        {
            ParentID = Par_ParentID;
        }

        public void SetTransportType(BwsTransportType par_bwsTransportType)
        {
            
            if (bwsTransportType != par_bwsTransportType)
            {
                bwsTransportType = par_bwsTransportType;

                _setTransportType();
            }
        }

        private void _setTransportType()
        {

            switch (bwsTransportType)
            {
                case BwsTransportType.Text:
                    break;
                case BwsTransportType.ArrayBuffer:
                    if (StaticClass.webWorkerHelpers_List.Any())
                    {
                        if (StaticClass.webWorkerHelpers_List.Any(x => x._id.Equals(ParentID, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            //StaticClass.webWorkerHelpers_List.Single(x => x._id.Equals(ParentID, StringComparison.InvariantCultureIgnoreCase)).Send(BCommandType.WsSetBinaryType,
                            //Json.Serialize(new { wsID = id, wsBinaryType = "arraybuffer" }));
                            StaticClass.webWorkerHelpers_List.Single(x => x._id.Equals(ParentID, StringComparison.InvariantCultureIgnoreCase)).Send(
                                BCommandType.WsSetBinaryType,
                                "arraybuffer", id);
                        }
                    };
                    break;
                case BwsTransportType.Blob:
                    if (StaticClass.webWorkerHelpers_List.Any())
                    {
                        if (StaticClass.webWorkerHelpers_List.Any(x => x._id.Equals(ParentID, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            //StaticClass.webWorkerHelpers_List.Single(x => x._id.Equals(ParentID, StringComparison.InvariantCultureIgnoreCase)).Send(BCommandType.WsSetBinaryType,
                            //Json.Serialize(new { wsID = id, wsBinaryType = "blob" }));
                            StaticClass.webWorkerHelpers_List.Single(x => x._id.Equals(ParentID, StringComparison.InvariantCultureIgnoreCase)).Send(
                                BCommandType.WsSetBinaryType,
                                "blob", id);
                        }
                    };
                    break;
                default:
                    break;
            }

        }

    }
}
