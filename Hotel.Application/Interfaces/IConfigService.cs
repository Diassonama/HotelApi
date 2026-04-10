using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Application.Interfaces
{
    public interface IConfigService
    {
        string Getvalue(string value);
        Task<string> GetValueAsync(string key);
    }
}