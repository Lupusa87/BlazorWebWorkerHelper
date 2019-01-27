using BlazorWebWorkerHelper.classes;
using BlazorWebWorkerHelper.WsClasses;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BlazorWebWorkerHelper.classes.BwwEnums;


namespace BlazorWebWorkerHelper
{
    public class WebWorkerHelper : IDisposable
    {
        public BWorkerType bworkerType { get; private set; } = BWorkerType.dedicated;

        public BwwTransportType bwwTransportType { get; private set; } = BwwTransportType.Text;

        public BwwState bwwState = BwwState.Undefined;

        public bool IsDisposed = false;

        public string _id { get; private set; } = BwwFunctions.Cmd_Get_UniqueID();

        public string _NameForSharedWW { get; private set; }


        private string _url = string.Empty;

        public Action<short> OnStateChange { get; set; }
        public Action<BwwMessage> OnMessage { get; set; }
        public Action<string> OnError { get; set; }

        public List<BwwError> BwwError = new List<BwwError>();


        public bool DoLog { get; set; } = true;
        public int LogMaxCount { get; set; } = 100;

        public List<BwwMessage> Log = new List<BwwMessage>();

        public int Active_WebSocket_ID;

        public List<BWebSocket> Ws_List = new List<BWebSocket>();

        public WebWorkerHelper(string Par_URL,string Par_NameForSharedWW, BWorkerType par_WorkerType, BwwTransportType par_TransportType)
        {

            _initialize(Par_URL, Par_NameForSharedWW, par_WorkerType, par_TransportType);
        }


        private void _initialize(string Par_URL, string Par_NameForSharedWW, BWorkerType par_WorkerType, BwwTransportType par_TransportType)
        {
            if (!string.IsNullOrEmpty(Par_URL))
            {
                StaticClass.webWorkerHelpers_List.Add(this);
                _url = Par_URL;
                _NameForSharedWW = Par_NameForSharedWW;
                bworkerType = par_WorkerType;
                bwwTransportType = par_TransportType;
                _create();
            }
            else
            {
                BwwError.Add(new BwwError { Message = "Url is not provided!", Description = string.Empty });
            }
        }


        private void _create()
        {
            BwwJsInterop.WwAdd(_id, _url, _NameForSharedWW, bworkerType, new DotNetObjectRef(this));
        }

        public void Send(BCommandType WCommandType, string Par_Message, bool AddToLog=true)
        {
            if (!string.IsNullOrEmpty(Par_Message))
            {
             
                BwwJsInterop.WwSend(_id, bworkerType, WCommandType, Par_Message);


                if (DoLog && AddToLog)
                {

                    Log.Add(new BwwMessage { ID = GetNewIDFromLog(),
                        Date = DateTime.Now,
                        MessageType = BwwMessageType.send,
                        TransportType =  BwwTransportType.Text,
                        WwBag = new BwwBag { data = Par_Message},
                    });

                    if (Log.Count > LogMaxCount)
                    {
                        Log.RemoveAt(0);
                    }
                }
            }
            else
            {
                Console.WriteLine("input is empty, method send");
            }
        }

        public void Send(BCommandType WCommandType, byte[] Par_Message, bool AddToLog = true)
        {
            string result = string.Empty;

            if (Par_Message.Length > 0)
            {
                BwwJsInterop.WwSend(_id, bworkerType, WCommandType, Par_Message);

                if (DoLog && AddToLog)
                {

                    Log.Add(new BwwMessage
                    {
                        ID = GetNewIDFromLog(),
                        Date = DateTime.Now,
                        MessageType = BwwMessageType.send,
                        TransportType =  BwwTransportType.Binary,
                        WwBag = new BwwBag { binarydata = Par_Message },
                    });
                    if (Log.Count > LogMaxCount)
                    {
                        Log.RemoveAt(0);
                    }
                }

            }

        }

        private int GetNewIDFromLog()
        {

            if (Log.Any())
            {
                return Log.Max(x => x.ID) + 1;
            }
            else
            {
                return 1;
            }
        }

        [JSInvokable]
        public void InvokeOnError(string par_error)
        {
            InvokeStateChanged(3);
            OnError?.Invoke(par_error);
        }

        [JSInvokable]
        public void InvokeStateChanged(short par_state)
        {

            bwwState = BwwFunctions.ConvertStatus(par_state);
            OnStateChange?.Invoke(par_state);
        }


        [JSInvokable]
        public void InvokeOnMessage(string par_message)
        {
           
            BwwBag msg = Json.Deserialize<BwwBag>(par_message);
         
            BwwMessage b = new BwwMessage
            {
                ID = GetNewIDFromLog(),
                Date = DateTime.Now,
                MessageType = BwwMessageType.received,
                TransportType = BwwTransportType.Text,
                WwBag = msg,
            };


            if (DoLog)
            {
                Log.Add(b);

                if (Log.Count > LogMaxCount)
                {
                    Log.RemoveAt(0);
                }
            }

            OnMessage?.Invoke(b);
        }

      


        public void InvokeOnMessageBinary(byte[] data)
        {

            BwwBag msg = Json.Deserialize<BwwBag>(Encoding.UTF8.GetString(data));


            BwwMessage b = new BwwMessage
            {
                ID = GetNewIDFromLog(),
                Date = DateTime.Now,
                MessageType = BwwMessageType.received,
                TransportType =  BwwTransportType.Binary,
                WwBag = msg
            };

            if (DoLog)
            {

                Log.Add(b);

                if (Log.Count > LogMaxCount)
                {
                    Log.RemoveAt(0);
                }
            }


            OnMessage?.Invoke(b);
        }

       

        public BWebSocket GetActiveWebSocket()
        {
            BWebSocket result = new BWebSocket(_id);
            if (Ws_List.Any())
            {
                result = Ws_List.Single(x => x.bWebSocketID == Active_WebSocket_ID);
            }

            return result;
        }



        public void SetTransportType(BwwTransportType par_bwwTransportType)
        {
            if (bwwTransportType!=par_bwwTransportType)
            {
                bwwTransportType = par_bwwTransportType;

                if (Ws_List.Any())
                {
                    foreach (var item in Ws_List)
                    {
                        item.SetTransportType((BwsTransportType)par_bwwTransportType);
                    }
                }
            }
        }

            public void Dispose()
        {
            if (DoLog)
            {
                Log = new List<BwwMessage>();
            }
            InvokeStateChanged(2);
            BwwJsInterop.WwRemove(_id);


            StaticClass.webWorkerHelpers_List.Remove(this);


            IsDisposed = true;
            GC.SuppressFinalize(this);
        }

    }
}
