using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Common;

namespace Hotel.Domain.Entities
{
    public class Series: BaseDomainEntity
    {
        public string TipoDoc  { get; set; }
        public string Serie  { get; set; }
        public string Descricao  { get; set; }
        public int Numerador  { get; set; }
        public DateTime DataUltimoDocumento  { get; set; }
        public int Ano  { get; set; }
        public string NumVias  { get; set; }
        public bool AutoFacturacao  { get; set; }
        public string Assinatura  { get; set; }
    }
}