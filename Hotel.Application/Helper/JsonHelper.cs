using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;

namespace Hotel.Application.Helper
{
    public class JsonHelper
    {
        public static string Serialize(object obj)
    {
        return JsonSerializer.Serialize(obj, new JsonSerializerOptions { WriteIndented = false });
    }

    public static T? Deserialize<T>(string json)
    {
        return string.IsNullOrEmpty(json) ? default : JsonSerializer.Deserialize<T>(json);
    }
    }
}