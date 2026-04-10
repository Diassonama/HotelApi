using Hotel.Domain.Common;

namespace Hotel.Domain.Entities
{
    public class Reserva : BaseDomainEntity
    {

        public int EmpresaId { get; private set; }
        public int NPX { get; private set; }
        public int QuantidadeQuartos { get; private set; }
        public decimal TotalGeral { get; private set; }

        // Navegação
        public Empresa Empresas { get; private set; }
        public Utilizador Utilizadores { get; private set; }
        private readonly List<ApartamentosReservado> _apartamentosReservados = new();
        public IReadOnlyCollection<ApartamentosReservado> ApartamentosReservados => _apartamentosReservados.AsReadOnly();
        public Reserva() { }
        // Construtor
        public Reserva(int empresaId, int npx, int quantidadeQuartos)
        {
            EmpresaId = empresaId;
            NPX = npx;
            QuantidadeQuartos = quantidadeQuartos;
            TotalGeral = 0;
            IsActive = true; // Define a reserva como ativa por padrão
        }

        public void AtualizarDados(int empresaId, int npx, int quantidadeQuartos)
        {
            EmpresaId = empresaId;
            NPX = npx;
            QuantidadeQuartos = quantidadeQuartos;
            AtualizarTotalGeral();
        }

        // Método para adicionar um apartamento reservado
        public void AdicionarApartamentoReservado(ApartamentosReservado apartamentoReservado)
        {
            if (apartamentoReservado == null)
                throw new ArgumentNullException(nameof(apartamentoReservado));
                apartamentoReservado.IsActive = true; // Define o ID da reserva no apartamento reservado
            _apartamentosReservados.Add(apartamentoReservado);
            AtualizarTotalGeral();
        }

        // Método para remover um apartamento reservado
        public void RemoverApartamentoReservado(ApartamentosReservado apartamentoReservado)
        {
            if (apartamentoReservado == null)
                throw new ArgumentNullException(nameof(apartamentoReservado));

            _apartamentosReservados.Remove(apartamentoReservado);
            AtualizarTotalGeral();
        }

        // Método privado para atualizar o total geral da reserva
        private void AtualizarTotalGeral()
        {
            TotalGeral = _apartamentosReservados.Sum(a => a.Total);
        }
    }
}