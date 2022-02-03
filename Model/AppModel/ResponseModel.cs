using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Model
{
    public class ResponseModel
    {
        public string Message { get; set; }
        public APIStatus Status { get; set; }
        public object Data { get; set; }
    }
    public enum APIStatus
    {
        Successful=0,
        Error=1,
        SystemError=2
    }
}
