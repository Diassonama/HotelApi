using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Application.DTOs.Request
{
    public class TenantMetadataRequest
    {
        public string Region { get; set; }
        public int MaxUsers { get; set; }
        public bool IsActive { get; set; }
        public Dictionary<string, string> CustomSettings { get; set; } //public string CustomSettings { get; set; }
    }
}