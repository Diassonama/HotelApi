using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Common;

namespace Hotel.Domain.Entities
{
    public class Caixa: BaseDomainEntity
    {
        public float SaldoInicial { get; set; }
        public float SaldoFinal { get; set; }
        public float SaldoAtual { get; set; }
        public float SaldoPendenteCaixaAnterior { get; set; }
        public float SaldoPendeteCaixaAtual { get; set; }
        public DateTime DataDeAbertura { get; set; }
        public DateTime? DataDeFechamento { get; set; } 
        public float Entrada { get; set; }
        public float Saida { get; set; }
        public string UtilizadoresId { get; set; }
        public Utilizador Utilizadores { get; set; }
       // public AspNetUsers MyProperty { get; set; }
        public ICollection<LancamentoCaixa> LancamentoCaixas { get; set; }
        public ICollection<Checkins>  Checkins { get; set; }

public Caixa()
{
    
}
public Caixa(float saldoInicial, string utilizadoresId)
    {
        SaldoInicial = saldoInicial;
        SaldoAtual = saldoInicial;
        UtilizadoresId = utilizadoresId;
        DataDeAbertura = DateTime.Now;
        LancamentoCaixas = new List<LancamentoCaixa>();
        Checkins = new List<Checkins>();
    }

    public void AdicionarEntrada(float valor)
    {
        if (valor < 0)
            throw new ArgumentException("Valor de entrada deve ser positivo");

        Entrada += valor;
        SaldoAtual += valor;
    }

    public void AdicionarSaida(float valor)
    {
        if (valor < 0)
            throw new ArgumentException("Valor de saída deve ser positivo");
        
        Saida += valor;
        SaldoAtual -= valor;
    }

    public void FecharCaixa()
    {
        /* if (DataDeFechamento.)
            throw new InvalidOperationException("O caixa já foi fechado.");
  */       
        DataDeFechamento = DateTime.Now;
        SaldoFinal = SaldoAtual;
    }


/* CAIXA DOMINIO RICO NOVO

public class Caixa
{
    public float SaldoInicial { get; set; }
    public float SaldoFinal { get; private set; }
    public float SaldoAtual { get; private set; }
    public float SaldoPendenteCaixaAnterior { get; set; }
    public float SaldoPendeteCaixaAtual { get; private set; }
    public DateTime DataDeAbertura { get; set; }
    public DateTime DataDeFechamento { get; set; }
    public float Entrada { get; private set; }
    public float Saida { get; private set; }
    public string UtilizadoresId { get; set; }
    public Utilizador Utilizadores { get; set; }
    public ICollection<LancamentoCaixa> LancamentoCaixas { get; set; } = new List<LancamentoCaixa>();
    public ICollection<Checkins> Checkins { get; set; } = new List<Checkins>();

    // Construtor
    public Caixa(float saldoInicial)
    {
        SaldoInicial = saldoInicial;
        SaldoAtual = saldoInicial;
        SaldoPendeteCaixaAtual = 0;
    }

    // Método para adicionar um lançamento no caixa
    public void AdicionarLancamento(LancamentoCaixa lancamento)
    {
        if (lancamento == null) throw new ArgumentNullException(nameof(lancamento));

        LancamentoCaixas.Add(lancamento);

        if (lancamento.Tipo == TipoLancamento.Entrada)
        {
            Entrada += lancamento.Valor;
            SaldoAtual += lancamento.Valor;
        }
        else if (lancamento.Tipo == TipoLancamento.Saida)
        {
            Saida += lancamento.Valor;
            SaldoAtual -= lancamento.Valor;
        }

        AtualizarSaldos();
    }

    // Método para calcular os saldos com base nos lançamentos
    private void AtualizarSaldos()
    {
        SaldoFinal = SaldoAtual;
        SaldoPendeteCaixaAtual = LancamentoCaixas
            .Where(l => !l.Confirmado)
            .Sum(l => l.Valor * (l.Tipo == TipoLancamento.Entrada ? 1 : -1));
    }

    // Método para fechar o caixa
    public void FecharCaixa()
    {
        if (DataDeFechamento != default)
            throw new InvalidOperationException("O caixa já foi fechado.");

        DataDeFechamento = DateTime.Now;
        SaldoFinal = SaldoAtual;
    }
}

public class LancamentoCaixa
{
    public int Id { get; set; }
    public string Descricao { get; set; }
    public float Valor { get; set; }
    public TipoLancamento Tipo { get; set; } // Enum: Entrada ou Saída
    public bool Confirmado { get; set; } = true;
    public DateTime Data { get; set; }
}

public enum TipoLancamento
{
    Entrada,
    Saida
}




 */


    }
}