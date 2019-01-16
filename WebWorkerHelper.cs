using BlazorWebWorkerHelper.classes;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static BlazorWebWorkerHelper.classes.BwwEnums;

namespace BlazorWebWorkerHelper
{
    public class WebWorkerHelper : IDisposable
    {
        public BWorkerType bworkerType { get; private set; } = BWorkerType.dedicated;

        public BwwState bwwState = BwwState.Undefined;

        public bool IsDisposed = false;

        private string _id = BwwFunctions.Cmd_Get_UniqueID();

        private string _url = string.Empty;

        public Action<short> OnStateChange { get; set; }
        public Action<string> OnMessage { get; set; }
        public Action<string> OnError { get; set; }

        public List<BwwError> BwwError = new List<BwwError>();


        public bool DoLog { get; set; } = true;
        public int LogMaxCount { get; set; } = 100;

        public List<BwwMessage> Log = new List<BwwMessage>();

        public WebWorkerHelper(string Par_URL, BWorkerType par_type = BWorkerType.dedicated)
        {

            _initialize(Par_URL, par_type);
        }



        private void _initialize(string Par_URL, BWorkerType par_type)
        {
            if (!string.IsNullOrEmpty(Par_URL))
            {
                _url = Par_URL;
                bworkerType = par_type;
                _create();
            }
            else
            {
                BwwError.Add(new BwwError { Message = "Url is not provided!", Description = string.Empty });
            }
        }


        private void _create()
        {
            BwwJsInterop.WwAdd(_id, _url, bworkerType, new DotNetObjectRef(this));
        }

        public void send(string Par_Message)
        {
            if (!string.IsNullOrEmpty(Par_Message))
            {
                BwwJsInterop.WwSend(_id, bworkerType, Par_Message);


                if (DoLog)
                {
                    Log.Add(new BwwMessage { ID = GetNewIDFromLog(), Date = DateTime.Now, Message = Par_Message, MessageType = BwwMessageType.send });

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
        public void InvokeOnMessage(string par_message)
        {

            if (DoLog)
            {
                Log.Add(new BwwMessage { ID = GetNewIDFromLog(), Date = DateTime.Now, Message = par_message, MessageType = BwwMessageType.received });

                if (Log.Count > LogMaxCount)
                {
                    Log.RemoveAt(0);
                }
            }

            OnMessage?.Invoke(par_message);
        }

        [JSInvokable]
        public void InvokeOnError(string par_error)
        {
            bwwState = BwwState.Error;
            OnError?.Invoke(par_error);
        }

        [JSInvokable]
        public void InvokeStateChanged(short par_state)
        {
            bwwState = BwwFunctions.ConvertStatus(par_state);
            OnStateChange?.Invoke(par_state);
        }


        public void Close()
        {
            if (DoLog)
            {
                Log = new List<BwwMessage>();
            }
            InvokeStateChanged(1);
            BwwJsInterop.WwClose(_id);
        }

        public void Dispose()
        {
            if (DoLog)
            {
                Log = new List<BwwMessage>();
            }
            InvokeStateChanged(1);
            BwwJsInterop.WwRemove(_id);
            IsDisposed = true;
            GC.SuppressFinalize(this);
        }

    }
}
