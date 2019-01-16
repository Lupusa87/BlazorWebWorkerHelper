using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static BlazorWebWorkerHelper.classes.BwwEnums;

namespace BlazorWebWorkerHelper.classes
{
    public class BwwMessage
    {
        public int ID { get; set; }
        public DateTime Date { get; set; }
        public string Message { get; set; }
        public BwwMessageType MessageType { get; set; }
    }
}
