
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hotel.Domain.Common
{
    public abstract class BaseDomainEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime DateCreated { get; set; }
        public string CreatedBy { get; set; }
       // public DateTime LastModifiedBy  { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public Boolean IsActive { get; set; }
        public int IdTenant { get; set; }
        

    }
}