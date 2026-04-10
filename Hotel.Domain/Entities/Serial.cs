using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Common;

namespace Hotel.Domain.Entities
{
    public class Serial : BaseDomainEntity
    {
        public int NumSerial { get; set; }
        //   public string Codigo { get; set; }
        public string Chave { get; set; } //ChaveHD
        public string DataInicial { get; set; }
        public string ContadorData { get; set; }
        public string UltimoAcesso { get; set; }
        public int Prazo { get; set; }

        public Serial(int numSerial, string contadorData, string dataInicial, int prazo)
        {
            SetNumSerial(numSerial);
            SetContadorData(contadorData);
            //  SetChave(chave);
            SetDataInicial(dataInicial);
            SetPrazo(prazo);
            // ContadorData = 0;
            UltimoAcesso = dataInicial;
        }

        // Validações encapsuladas
        public void SetNumSerial(int numSerial)
        {
            if (numSerial <= 0)
                //  if(string.IsNullOrEmpty(numSerial) )
                throw new ArgumentException("O número de série não deve ser null.");
            NumSerial = numSerial;
        }
       /*  public void SetDataUltimoAcesso(DateTime data)
        {

            UltimoAcesso = data;
        } */

        public void SetChave(string value)
        {
            // if (value <= 0)
            if (string.IsNullOrEmpty(value))
                throw new ArgumentException("A chave não deve ser null.");
            Chave = value;
        }
        public void SetContadorData(string value)
        {
            // if (value <= 0)
            if (string.IsNullOrEmpty(value))
                throw new ArgumentException("O valor do contador não deve ser null.");
            ContadorData = value;
        }
        /*  public void SetCodigo(int codigo)
         {
             if (codigo <= 0)
                 throw new ArgumentException("O código deve ser maior que zero.");
             Codigo = codigo;
         } */

        /*  public void SetChave(int chave)
         {
             if (chave <= 0)
                 throw new ArgumentException("A chave deve ser maior que zero.");
             Chave = chave;
         } */

        public void SetDataInicial(string dataInicial)
        {
            
            DataInicial = dataInicial;
        }

        public void SetPrazo(int prazo)
        {
            if (prazo <= 0)
                throw new ArgumentException("O prazo deve ser positivo.");

            Prazo = prazo;
        }

        // Métodos para manipular o domínio
        public void AtualizarUltimoAcesso()
        {
            UltimoAcesso = DateTime.Now.Date.ToString();
        }

       

       /*  public void IncrementarContador()
        {
            ContadorData++;
        } */
    }
}