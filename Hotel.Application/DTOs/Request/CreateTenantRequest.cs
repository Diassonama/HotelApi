using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Application.DTOs.Request
{
    public class CreateTenantRequest
    {
    public string Id { get; set; }
    public string Name { get; set; }
    public string DatabaseServerName { get; set; }
    public string UserID { get; set; }
    public string Password { get; set; }
    public string DatabaseName { get; set; }
    public TenantMetadataRequest Metadata { get; set; }
    }



    
}