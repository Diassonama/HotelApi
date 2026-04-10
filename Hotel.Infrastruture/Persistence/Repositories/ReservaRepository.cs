using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Entities;
using Hotel.Domain.Interface;
using Hotel.Domain.Interface.Shared;
using Hotel.Infrastruture.Persistence.Context;
using Hotel.Infrastruture.Persistence.Shared;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Infrastruture.Persistence.Repositories
{
    public class ReservaRepository : RepositoryBase<Reserva>, IReservaRepository
    {
        private readonly GhotelDbContext _context;

        public ReservaRepository(GhotelDbContext dbContext) : base(dbContext)
        {
            _context = dbContext;
        }

        /// <summary>
        /// Insere uma nova reserva com seus apartamentos reservados
        /// </summary>
        /// <param name="reserva">Dados da reserva</param>
        /// <param name="apartamentosReservados">Lista de apartamentos a serem reservados</param>
        /// <returns>Reserva criada com ID gerado</returns>
        public async Task<Reserva> InserirReservaComApartamentosAsync(Reserva reserva, List<ApartamentosReservado> apartamentosReservados)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 1. Validar se todos os apartamentos estão disponíveis no período
                foreach (var apartamentoReservado in apartamentosReservados)
                {
                    var (isDisponivel, mensagemErro) = await VerificarDisponibilidadeApartamentoAsync(
                        apartamentoReservado.ApartamentosId,
                        apartamentoReservado.DataEntrada,
                        apartamentoReservado.DataSaida);

                    if (!isDisponivel)
                    {
                        throw new InvalidOperationException(mensagemErro);
                    }
                }

                // 2. Inserir a reserva principal
                _context.Reservas.Add(reserva);
                await _context.SaveChangesAsync();

                // 3. Inserir os apartamentos reservados vinculados à reserva
                foreach (var apartamentoReservado in apartamentosReservados)
                {
                    // Definir o ID da reserva criada
                    var novoApartamentoReservado = new ApartamentosReservado(
                        reserva.Id,
                        apartamentoReservado.ApartamentosId,
                        apartamentoReservado.DataEntrada,
                        apartamentoReservado.DataSaida,
                        apartamentoReservado.ClientesId,
                        apartamentoReservado.TipoHospedagensId,
                        apartamentoReservado.UtilizadoresId,
                        apartamentoReservado.ValorDiaria,
                        apartamentoReservado.ReservaConfirmada,
                        apartamentoReservado.ReservaNoShow
                    );

                    _context.ApartamentosReservados.Add(novoApartamentoReservado);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // 4. Retornar a reserva com os apartamentos carregados
                return await ObterReservaComApartamentosAsync(reserva.Id);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        /// <summary>
        /// Insere apenas apartamentos reservados para uma reserva existente
        /// </summary>
        /// <param name="reservaId">ID da reserva existente</param>
        /// <param name="apartamentosReservados">Lista de apartamentos a serem adicionados</param>
        /// <returns>True se inserido com sucesso</returns>
        public async Task<bool> InserirApartamentosReservadosAsync(int reservaId, List<ApartamentosReservado> apartamentosReservados)
        {
            // Verificar se a reserva existe
            var reservaExistente = await _context.Reservas.FindAsync(reservaId);
            if (reservaExistente == null)
            {
                throw new ArgumentException($"Reserva com ID {reservaId} não encontrada.");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Validar disponibilidade dos apartamentos
                foreach (var apartamentoReservado in apartamentosReservados)
                {
                    var (isDisponivel, mensagemErro) = await VerificarDisponibilidadeApartamentoAsync(
                        apartamentoReservado.ApartamentosId,
                        apartamentoReservado.DataEntrada,
                        apartamentoReservado.DataSaida);

                    if (!isDisponivel)
                    {
                        throw new InvalidOperationException(mensagemErro);
                    }
                }

                // Inserir os apartamentos reservados
                foreach (var apartamentoReservado in apartamentosReservados)
                {
                    var novoApartamentoReservado = new ApartamentosReservado(
                        reservaId,
                        apartamentoReservado.ApartamentosId,
                        apartamentoReservado.DataEntrada,
                        apartamentoReservado.DataSaida,
                        apartamentoReservado.ClientesId,
                        apartamentoReservado.TipoHospedagensId,
                        apartamentoReservado.UtilizadoresId,
                        apartamentoReservado.ValorDiaria,
                        apartamentoReservado.ReservaConfirmada,
                        apartamentoReservado.ReservaNoShow
                    );

                    _context.ApartamentosReservados.Add(novoApartamentoReservado);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        /// <summary>
        /// Verifica se um apartamento está disponível no período especificado (método público)
        /// </summary>
        /// <param name="apartamentoId">ID do apartamento</param>
        /// <param name="dataEntrada">Data de entrada</param>
        /// <param name="dataSaida">Data de saída</param>
        /// <returns>Resultado da verificação com disponibilidade e mensagem de erro</returns>
        public async Task<(bool IsDisponivel, string MensagemErro)> VerificarDisponibilidadeAsync(int apartamentoId, DateTime dataEntrada, DateTime dataSaida)
        {
            return await VerificarDisponibilidadeApartamentoAsync(apartamentoId, dataEntrada, dataSaida);
        }

        /// <summary>
        /// Verifica se um apartamento está disponível no período especificado, excluindo uma reserva específica da verificação
        /// </summary>
        /// <param name="apartamentoId">ID do apartamento</param>
        /// <param name="dataEntrada">Data de entrada</param>
        /// <param name="dataSaida">Data de saída</param>
        /// <param name="reservaIdExcluir">ID da reserva a ser excluída da verificação (para updates)</param>
        /// <returns>Resultado da verificação com disponibilidade e mensagem de erro</returns>
        public async Task<(bool IsDisponivel, string MensagemErro)> VerificarDisponibilidadeAsync(int apartamentoId, DateTime dataEntrada, DateTime dataSaida, int? reservaIdExcluir)
        {
            return await VerificarDisponibilidadeApartamentoAsync(apartamentoId, dataEntrada, dataSaida, reservaIdExcluir);
        }

        /// <summary>
        /// Verifica se um apartamento está disponível no período especificado
        /// </summary>
        /// <param name="apartamentoId">ID do apartamento</param>
        /// <param name="dataEntrada">Data de entrada</param>
        /// <param name="dataSaida">Data de saída</param>
        /// <returns>Resultado da verificação com disponibilidade e mensagem de erro</returns>
        private async Task<(bool IsDisponivel, string MensagemErro)> VerificarDisponibilidadeApartamentoAsync(int apartamentoId, DateTime dataEntrada, DateTime dataSaida)
        {
            // Obter informações do apartamento para mensagens mais detalhadas
            var apartamento = await _context.Apartamentos
                .Include(a => a.TipoApartamentos)
                .FirstOrDefaultAsync(a => a.Id == apartamentoId);

            if (apartamento == null)
            {
                return (false, $"Apartamento com ID {apartamentoId} não foi encontrado.");
            }

            // Verifica hospedagens existentes
            var hospedagemConflitante = await _context.Hospedagems
                .FirstOrDefaultAsync(h => h.ApartamentosId == apartamentoId &&
                              ((dataEntrada >= h.DataAbertura && dataEntrada < h.PrevisaoFechamento) ||
                               (dataSaida > h.DataAbertura && dataSaida <= h.PrevisaoFechamento) ||
                               (dataEntrada <= h.DataAbertura && dataSaida >= h.PrevisaoFechamento)));

            if (hospedagemConflitante != null)
            {
                return (false, $"Apartamento {apartamento.Codigo} ({apartamento.TipoApartamentos?.Descricao}) está ocupado " +
                              $"de {hospedagemConflitante.DataAbertura:dd/MM/yyyy} a {hospedagemConflitante.PrevisaoFechamento:dd/MM/yyyy}.");
            }

            // Verifica reservas existentes
            var reservaConflitante = await _context.ApartamentosReservados
                .Include(ar => ar.Reservas)
                .Include(ar => ar.Clientes)
                .FirstOrDefaultAsync(ar => ar.ApartamentosId == apartamentoId &&
                               ar.IsActive &&
                               ((dataEntrada >= ar.DataEntrada && dataEntrada < ar.DataSaida) ||
                                (dataSaida > ar.DataEntrada && dataSaida <= ar.DataSaida) ||
                                (dataEntrada <= ar.DataEntrada && dataSaida >= ar.DataSaida)));

            if (reservaConflitante != null)
            {
                var nomeCliente = reservaConflitante.Clientes?.Nome ?? "Cliente não identificado";
                return (false, $"Apartamento {apartamento.Codigo} ({apartamento.TipoApartamentos?.Descricao}) já está reservado para {nomeCliente} " +
                              $"de {reservaConflitante.DataEntrada:dd/MM/yyyy} a {reservaConflitante.DataSaida:dd/MM/yyyy} " +
                              $"(Reserva #{reservaConflitante.ReservasId}).");
            }

            return (true, string.Empty);
        }

        /// <summary>
        /// Verifica se um apartamento está disponível no período especificado, excluindo uma reserva específica
        /// </summary>
        /// <param name="apartamentoId">ID do apartamento</param>
        /// <param name="dataEntrada">Data de entrada</param>
        /// <param name="dataSaida">Data de saída</param>
        /// <param name="reservaIdExcluir">ID da reserva a ser excluída da verificação (para updates)</param>
        /// <returns>Resultado da verificação com disponibilidade e mensagem de erro</returns>
        private async Task<(bool IsDisponivel, string MensagemErro)> VerificarDisponibilidadeApartamentoAsync(int apartamentoId, DateTime dataEntrada, DateTime dataSaida, int? reservaIdExcluir)
        {
            // Obter informações do apartamento para mensagens mais detalhadas
            var apartamento = await _context.Apartamentos
                .Include(a => a.TipoApartamentos)
                .FirstOrDefaultAsync(a => a.Id == apartamentoId);

            if (apartamento == null)
            {
                return (false, $"Apartamento com ID {apartamentoId} não foi encontrado.");
            }

            // Verifica hospedagens existentes
            var hospedagemConflitante = await _context.Hospedagems
                .FirstOrDefaultAsync(h => h.ApartamentosId == apartamentoId &&
                              ((dataEntrada >= h.DataAbertura && dataEntrada < h.PrevisaoFechamento) ||
                               (dataSaida > h.DataAbertura && dataSaida <= h.PrevisaoFechamento) ||
                               (dataEntrada <= h.DataAbertura && dataSaida >= h.PrevisaoFechamento)));

            if (hospedagemConflitante != null)
            {
                return (false, $"Apartamento {apartamento.Codigo} ({apartamento.TipoApartamentos?.Descricao}) está ocupado " +
                              $"de {hospedagemConflitante.DataAbertura:dd/MM/yyyy} a {hospedagemConflitante.PrevisaoFechamento:dd/MM/yyyy}.");
            }

            // Verifica reservas existentes, EXCLUINDO a reserva atual se fornecida
            var query = _context.ApartamentosReservados
                .Include(ar => ar.Reservas)
                .Include(ar => ar.Clientes)
                .Where(ar => ar.ApartamentosId == apartamentoId &&
                       ar.IsActive &&
                       ((dataEntrada >= ar.DataEntrada && dataEntrada < ar.DataSaida) ||
                        (dataSaida > ar.DataEntrada && dataSaida <= ar.DataSaida) ||
                        (dataEntrada <= ar.DataEntrada && dataSaida >= ar.DataSaida)));

            // 🎯 EXCLUSÃO CRÍTICA: Se reservaIdExcluir for fornecido, exclui da verificação
            if (reservaIdExcluir.HasValue)
            {
                query = query.Where(ar => ar.ReservasId != reservaIdExcluir.Value);
            }

            var reservaConflitante = await query.FirstOrDefaultAsync();

            if (reservaConflitante != null)
            {
                var nomeCliente = reservaConflitante.Clientes?.Nome ?? "Cliente não identificado";
                return (false, $"Apartamento {apartamento.Codigo} ({apartamento.TipoApartamentos?.Descricao}) já está reservado para {nomeCliente} " +
                              $"de {reservaConflitante.DataEntrada:dd/MM/yyyy} a {reservaConflitante.DataSaida:dd/MM/yyyy} " +
                              $"(Reserva #{reservaConflitante.ReservasId}).");
            }

            return (true, string.Empty);
        }

        /// <summary>
        /// Obtém uma reserva com todos os apartamentos reservados
        /// </summary>
        /// <param name="reservaId">ID da reserva</param>
        /// <returns>Reserva com apartamentos carregados</returns>
        public async Task<Reserva> ObterReservaComApartamentosAsync(int reservaId)
        {
            return await _context.Reservas
                .Include(r => r.Empresas)
                //   .Include(r => r.Utilizadores)
                .Include(r => r.ApartamentosReservados.Where(ar => ar.IsActive))
                    .ThenInclude(ar => ar.Apartamentos).ThenInclude(ar => ar.TipoApartamentos)
                .Include(r => r.ApartamentosReservados.Where(ar => ar.IsActive))
                    .ThenInclude(ar => ar.Clientes)
                .Include(r => r.ApartamentosReservados.Where(ar => ar.IsActive))
                    .ThenInclude(ar => ar.TipoHospedagens)
                .FirstOrDefaultAsync(r => r.Id == reservaId && r.IsActive); // && r.IsActive
        }

        public async Task<IEnumerable<Reserva>> ObterTodasReservaComApartamentosAsync()
        {
            return await _context.Reservas
                .Include(r => r.Empresas)
                //   .Include(r => r.Utilizadores)
                .Include(r => r.ApartamentosReservados.Where(ar => ar.IsActive))
                    .ThenInclude(ar => ar.Apartamentos)
                .Include(r => r.ApartamentosReservados.Where(ar => ar.IsActive))
                    .ThenInclude(ar => ar.Clientes)
                .Include(r => r.ApartamentosReservados.Where(ar => ar.IsActive))
                    .ThenInclude(ar => ar.TipoHospedagens)
                    .Where(r => r.IsActive)
                .ToListAsync();
        }

        /// <summary>
        /// Cancela apartamentos reservados específicos
        /// </summary>
        /// <param name="apartamentosReservadosIds">IDs dos apartamentos reservados a cancelar</param>
        /// <returns>True se cancelado com sucesso</returns>
        public async Task<bool> CancelarApartamentosReservadosAsync(List<int> apartamentosReservadosIds)
        {
            var apartamentosReservados = await _context.ApartamentosReservados
                .Where(ar => apartamentosReservadosIds.Contains(ar.Id) && ar.IsActive)
                .ToListAsync();

            if (!apartamentosReservados.Any())
            {
                return false;
            }

            foreach (var apartamentoReservado in apartamentosReservados)
            {
                apartamentoReservado.IsActive = false;
                apartamentoReservado.LastModifiedDate = DateTime.Now;
            }

            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<IPaginatedList<Reserva>> GetFilteredReservaquery(Domain.Interface.Shared.PaginationFilter paginationFilter)
        {

            var query = _context.Reservas
                    .Include(p => p.Empresas)
                    .Include(r => r.ApartamentosReservados.Where(ar => ar.IsActive))
                        .ThenInclude(ar => ar.Apartamentos)
                    .Include(r => r.ApartamentosReservados.Where(ar => ar.IsActive))
                        .ThenInclude(ar => ar.Clientes)
                    .Include(r => r.ApartamentosReservados.Where(ar => ar.IsActive))
                        .ThenInclude(ar => ar.TipoHospedagens)
                    .Where(r => r.IsActive)
                    .AsNoTracking();

            // Normaliza filtro
            var filtro = paginationFilter.FieldFilter?.Trim().ToLower();

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                query = query.Where(r =>
                    // Código da reserva (pode usar r.Id ou r.NPX, dependendo do que usa como código)
                    r.Id.ToString().Contains(filtro) ||

                    // Nome do cliente
                    r.ApartamentosReservados.Any(ar =>
                        ar.Clientes != null &&
                        ar.Clientes.Nome.Trim().ToLower().Contains(filtro)
                    )
                );
            }

            return await IPaginatedList<Reserva>.ToPagedList(query, paginationFilter.PageNumber, paginationFilter.PageSize);
        }

     public async Task<IPaginatedList<ApartamentosReservado>> GetFilteredApartamentosReservadosquery(Domain.Interface.Shared.PaginationFilter paginationFilter)
{
    var filtro = paginationFilter.FieldFilter?.Trim().ToLower();

    var query = _context.ApartamentosReservados
        .Include(ar => ar.Reservas)
            .ThenInclude(r => r.Empresas)
        .Include(ar => ar.Reservas)
            .ThenInclude(r => r.Utilizadores)
        .Include(ar => ar.Clientes)
        .Include(ar => ar.Apartamentos)
        .Include(ar => ar.TipoHospedagens)
        .Where(ar => ar.IsActive)
        .AsNoTracking();

    if (!string.IsNullOrWhiteSpace(filtro))
    {
        query = query.Where(ar =>
            ar.Reservas.Id.ToString().Contains(filtro) || // Código da reserva
            (ar.Clientes != null && ar.Clientes.Nome.ToLower().Contains(filtro)) // Nome do cliente
        );
    }

    return await IPaginatedList<ApartamentosReservado>.ToPagedList(query, paginationFilter.PageNumber, paginationFilter.PageSize);
}

 public IQueryable GetFilteredAsync(Domain.Interface.Shared.PaginationFilter paginationFilter)
        {

    var filtro = paginationFilter.FieldFilter?.Trim().ToLower();
            IQueryable<ApartamentosReservado> query = Enumerable.Empty<ApartamentosReservado>().AsQueryable();
            query = (from apart in _context.ApartamentosReservados
        .Include(ar => ar.Reservas)
            .ThenInclude(r => r.Empresas)
        .Include(ar => ar.Reservas)
            .ThenInclude(r => r.Utilizadores)
        .Include(ar => ar.Clientes)
        .Include(ar => ar.Apartamentos)
        .Include(ar => ar.TipoHospedagens)
        .Where(ar => ar.IsActive)
        .AsNoTracking()       select apart);

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                query = query.Where(ar =>
                    ar.Reservas.Id.ToString().Contains(filtro) || // Código da reserva
                    (ar.Clientes != null && ar.Clientes.Nome.ToLower().Contains(filtro)) // Nome do cliente
                );
            }
            return query;
        }

        public async Task<bool> VerificarHospedagensAtivasAsync(int reservaId)
        {
            try
            {
                // Verifica se há hospedagens ativas através dos apartamentos da reserva
                var apartamentosReservados = await _context.ApartamentosReservados
                    .Where(ar => ar.ReservasId == reservaId && ar.IsActive)
                    .Select(ar => ar.ApartamentosId)
                    .ToListAsync();

                if (!apartamentosReservados.Any())
                    return false;

                var temHospedagensAtivas = await _context.Hospedagems
                    .AnyAsync(h => apartamentosReservados.Contains(h.ApartamentosId) && 
                                  h.IsActive && 
                                  h.DataAbertura != default(DateTime) && 
                                  h.DataFechamento == default(DateTime));

                return temHospedagensAtivas;
            }
            catch
            {
                // Em caso de erro, considera como não tendo hospedagens ativas
                return false; 
            }
        }


    }
}