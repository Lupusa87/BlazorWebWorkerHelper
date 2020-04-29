using BlazorWebWorkerHelper.classes;
using BlazorWebWorkerHelper.WsClasses;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static BlazorWebWorkerHelper.classes.BwwEnums;


namespace BlazorWebWorkerHelper
{
    public class WebWorkerHelper : IDisposable
    {

        BwwJsInterop bwwJsInterop;

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

        private IJSRuntime _JSRuntime;

        public WebWorkerHelper(string Par_URL,string Par_NameForSharedWW, BWorkerType par_WorkerType, BwwTransportType par_TransportType, IJSRuntime jsRuntime)
        {
            _JSRuntime = jsRuntime ??
           throw new ArgumentNullException($"{nameof(jsRuntime)} missing. Try injecting it in your component, then passing it from OnAfterRender.");


            bwwJsInterop = new BwwJsInterop(_JSRuntime);

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
            bwwJsInterop.WwAdd(_id, _url, _NameForSharedWW, bworkerType, DotNetObjectReference.Create(this));
        }

        public void Send(BCommandType WCommandType, string Par_Message, string AdditionalArgs, bool AddToLog=true)
        {
            if (!string.IsNullOrEmpty(Par_Message))
            {
             
                bwwJsInterop.WwSend(_id, bworkerType, WCommandType, Par_Message, AdditionalArgs);


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

        public void Send(BCommandType WCommandType, byte[] Par_Message, string AdditionalArgs, bool AddToLog = true)
        {
            string result = string.Empty;

            if (Par_Message.Length > 0)
            {

               
                bwwJsInterop.WwSend(_id, bworkerType, WCommandType, Par_Message, AdditionalArgs);

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
           
            BwwBag msg = JsonSerializer.Deserialize<BwwBag>(par_message);
         
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


        private object ByteArrayToObject(byte[] arrBytes)
        {
            MemoryStream memStream = new MemoryStream();
            BinaryFormatter binForm = new BinaryFormatter();
            memStream.Write(arrBytes, 0, arrBytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            object obj = (object)binForm.Deserialize(memStream);

            return obj;
        }

        public void InvokeOnMessageBinary(byte[] data, string bag)
        {

            BwwBag msg = JsonSerializer.Deserialize<BwwBag>(bag);
            msg.binarydata = data;


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
            bwwJsInterop.WwRemove(_id);


            StaticClass.webWorkerHelpers_List.Remove(this);


            IsDisposed = true;
            GC.SuppressFinalize(this);
        }

    }
}
