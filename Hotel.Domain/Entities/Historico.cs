using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Common;

namespace Hotel.Domain.Entities
{
    /* public class Historico: BaseDomainEntity
    {

        public int CaixaAberto { get; set; }
   
        public DateTime DataHora { get; set; }
        public string Observacao { get; set; }
        public int UtilizadorId { get; set; }
        public int CheckinsId  { get; set; }

        public Utilizador Utilizadores { get; set; }
        public Checkins Checkins { get; set; }
        public Hospedagem Hospedagem { get; set; }


    } */

    public class Historico : BaseDomainEntity
    {
        public int CaixaAberto { get; private set; }
        public DateTime DataHora { get; private set; }
        public string Observacao { get; private set; }
        public string UtilizadoresId { get; private set; }
        public int CheckinsId { get; private set; }

        public Utilizador Utilizadores { get; private set; }
        public Checkins Checkins { get; private set; }
        public Hospedagem Hospedagem { get; private set; }

        // Construtor para garantir inicialização válida
        public Historico(int caixaAberto,  string utilizadoresId, int checkinsId)
        {
           // ValidarCaixaAberto(caixaAberto);
        
            CaixaAberto = caixaAberto;
            DataHora = DateTime.Now;
            UtilizadoresId = utilizadoresId;
            CheckinsId = checkinsId;
           // Observacao = observacao;
        }

        // Método para alterar o status do caixa
        public void AlterarStatusCaixa(int novoStatus)
        {
            ValidarCaixaAberto(novoStatus);
            CaixaAberto = novoStatus;
        }

        // Método para adicionar uma observação
        public void AdicionarObservacao(string Quarto, DateTime Previsao, DateTime Fim)
        {

            if(Previsao.Date != Fim.Date){
               // Observacao = "Checkin com previsão de saída: " + Previsao + " e fim: " + Fim;
                if(Previsao.Date > Fim.Date)
                {
                    Observacao = "Checkout do " + Quarto  + " foi antecipado de " + Previsao.Date.ToShortDateString() + " para " + Fim.Date.ToShortDateString() ;
                }
                else
                {
                    Observacao = "Checkout do " + Quarto  + " foi adiado de " + Previsao.Date.ToShortDateString() + " para " + Fim.Date.ToShortDateString();
                }
            }
        }

        public void HistoricoTransferencia(string QuartoOrigem, string QuartoDestino)
        {
            Observacao = "Hospedagem transferida do quarto " + QuartoOrigem + " para o quarto " + QuartoDestino;
        }

        public void AdicionarObservacao(string mensagem)
        {
            Observacao = mensagem;

        }

        // Método para associar um utilizador
        public void AssociarUtilizador(Utilizador utilizador)
        {
            if (utilizador == null)
            {
                throw new ArgumentException("O utilizador não pode ser nulo.");
            }

            Utilizadores = utilizador;
        }

        // Método para associar um check-in
        public void AssociarCheckin(Checkins checkin)
        {
            if (checkin == null)
            {
                throw new ArgumentException("O check-in não pode ser nulo.");
            }

            Checkins = checkin;
        }

        // Método para associar uma hospedagem
        public void AssociarHospedagem(Hospedagem hospedagem)
        {
            if (hospedagem == null)
            {
                throw new ArgumentException("A hospedagem não pode ser nula.");
            }

            Hospedagem = hospedagem;
        }

        // Validação de status do caixa
        private void ValidarCaixaAberto(int caixaAberto)
        {
            if (caixaAberto != 0 && caixaAberto != 1)
            {
                throw new ArgumentException("O status do caixa deve ser 0 (fechado) ou 1 (aberto).");
            }
        }

        // Validação de data e hora
        private void ValidarDataHora(DateTime dataHora)
        {
            if (dataHora > DateTime.Now)
            {
                throw new ArgumentException("A data e hora não podem estar no futuro.");
            }
        }
    }

}

