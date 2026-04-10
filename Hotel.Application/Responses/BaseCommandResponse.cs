using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Application.Responses
{
    public class BaseCommandResponse
    {
        public bool Success { get; set; }
        public object Data { get; set; }
        public string Message { get; set; }
        public List<string> Errors { get; set; }
        public string StackTrace { get; set; } = null;
        public bool ShowDetails { get; set; } = false;

        public static implicit operator BaseCommandResponse(string v)
        {
            throw new NotImplementedException();
        }

    }
}