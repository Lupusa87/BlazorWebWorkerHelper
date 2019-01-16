using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static BlazorWebWorkerHelper.classes.BwwEnums;

namespace BlazorWebWorkerHelper
{
    public static class BwwFunctions
    {
        public static string Cmd_Get_UniqueID()
        {
            long j = DateTime.Now.Ticks;
            string a = j.ToString();
            return a.Substring(a.Length - 8, 4) + Guid.NewGuid().ToString("d").Substring(1, 4);
        }


        public static BwwState ConvertStatus(short a)
        {
            BwwState result = BwwState.Undefined;


            switch (a)
            {
                case -1:
                    result = BwwState.Error;
                    break;
                case 0:
                    result = BwwState.Open;
                    break;
                case 1:
                    result = BwwState.Close;
                    break;
                default:
                    result = BwwState.Undefined;
                    break;
            }

            return result;

        }
    }
}
