using System.Collections.Generic;

namespace Hotel.Application.Responses
{
    public class BaseQueryResponse<T>
    {
        public bool Success { get; set; } = true;
        public string Message { get; set; } = string.Empty;
        public T Data { get; set; }
        public int Count { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }
}