using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Hotel.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Hotel.Domain.Identity
{
    public class ApplicationUser : IdentityUser<string>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [JsonIgnore]
        public RefreshToken RefreshToken { get; set; }
       // public ICollection<IdentityUserRole<string>> UtilizadorRoles { get; set; }

       /*    [ForeignKey("Id")]
        public virtual AspNetUsers AspNetUser { get; set; } */
 


    }
}