using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Application
{
    public class Response
    {
        public bool Success { get;  set; }
        public object Data { get;  set; }
        public string Message { get; set; }
        public List<string> Errors { get; set; }
    }
}