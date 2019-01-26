using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static BlazorWebWorkerHelper.classes.BwwEnums;

namespace BlazorWebWorkerHelper.classes
{
    public class BwwBag
    {
        public short Cmd { get; set; }
        public bool isBinary { get; set; }
        public byte[] binarydata { get; set; }
        public string data { get; set; }
        public short ClientID { get; set; }
    }
}
