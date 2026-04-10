using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Domain.Entities
{
    public class ProductType
	{
		[Key]
		public string ProductTypeCode { get; set; }
		public string ProductTypeDescription { get; set; }
		public string CaminhoImagem { get; set; } = "";
		public ICollection<Produtos> Produtos { get; set; }

	}
}