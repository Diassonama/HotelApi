using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Entities;
using Hotel.Domain.Interface;
using Hotel.Infrastruture.Persistence.Context;
using Hotel.Infrastruture.Persistence.Shared;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Infrastruture.Persistence.Repositories
{
    public class ApartamentosReservadoRepository : RepositoryBase<ApartamentosReservado>, IApartamentoReservadoRepository
    {
        private readonly GhotelDbContext _context;
        public ApartamentosReservadoRepository(GhotelDbContext dbContext) : base(dbContext)
        {
            _context = dbContext;
        }
        
        public async Task<bool> VerificarDisponibilidadeAsync(int roomId, DateTime startDate, DateTime endDate)
{
    // 1️⃣ Verifica se já existe uma hospedagem no período
    bool hospedagemExistente = await _context.Hospedagems
        .AnyAsync(h => h.ApartamentosId == roomId &&
                       ((startDate >= h.DataAbertura && startDate < h.PrevisaoFechamento) ||  
                        (endDate > h.DataAbertura && endDate <= h.PrevisaoFechamento) ||    
                        (startDate <= h.DataAbertura && endDate >= h.PrevisaoFechamento)));

    if (hospedagemExistente)
    {
        throw new Exception("Quarto já está ocupado no período informado.");
    }

    // 2️⃣ Verifica se já existe uma reserva no período
   bool reservaExistente = await _context.Reservas
    .Include(r => r.ApartamentosReservados)
    .AnyAsync(r => r.ApartamentosReservados.Any(a => 
        a.ApartamentosId == roomId &&
        ((startDate >= a.DataEntrada && startDate < a.DataSaida) ||  
         (endDate > a.DataEntrada && endDate <= a.DataSaida) ||    
         (startDate <= a.DataEntrada && endDate >= a.DataSaida))
    ));

    if (reservaExistente)
    {
        throw new Exception("Quarto já está reservado no período informado.");
    }

    return true; // Quarto disponível para reserva
}

        public async Task<bool> VerificarDisponibilidadeAsync(int roomId, DateTime startDate, DateTime endDate, int? excludeReservationId)
        {
            // 1️⃣ Verifica se já existe uma hospedagem no período
            bool hospedagemExistente = await _context.Hospedagems
                .AnyAsync(h => h.ApartamentosId == roomId &&
                               ((startDate >= h.DataAbertura && startDate < h.PrevisaoFechamento) ||  
                                (endDate > h.DataAbertura && endDate <= h.PrevisaoFechamento) ||    
                                (startDate <= h.DataAbertura && endDate >= h.PrevisaoFechamento)));

            if (hospedagemExistente)
            {
                return false; // Não disponível
            }

            // 2️⃣ Verifica se já existe uma reserva no período (excluindo a reserva atual se fornecida)
            bool reservaExistente = await _context.ApartamentosReservados
                .AnyAsync(ar => ar.ApartamentosId == roomId &&
                               !ar.ReservaNoShow && // Considera apenas reservas não canceladas
                               (excludeReservationId == null || ar.Id != excludeReservationId) &&
                               ((startDate >= ar.DataEntrada && startDate < ar.DataSaida) ||  
                                (endDate > ar.DataEntrada && endDate <= ar.DataSaida) ||    
                                (startDate <= ar.DataEntrada && endDate >= ar.DataSaida)));

            return !reservaExistente; // Retorna true se não há conflito
        }

        /// <summary>
        /// Obtém todas as reservas de apartamentos com dados completos da tabela Reserva
        /// </summary>
        /// <returns>Lista de apartamentos reservados com dados da reserva</returns>
        public async Task<IEnumerable<ApartamentosReservado>> ObterReservasComDadosCompletosAsync()
        {
            return await _context.ApartamentosReservados
                .Include(ar => ar.Reservas)
                    .ThenInclude(r => r.Empresas)
                .Include(ar => ar.Apartamentos)
                .Include(ar => ar.Clientes)
                .Include(ar => ar.TipoHospedagens)
                .Include(ar => ar.Utilizadores)
                .Where(ar => ar.IsActive)
                .OrderBy(ar => ar.DataEntrada)
                .ToListAsync();
        }

        /// <summary>
        /// Obtém reservas de apartamentos por período incluindo dados da tabela Reserva
        /// </summary>
        /// <param name="dataInicio">Data de início do período</param>
        /// <param name="dataFim">Data de fim do período</param>
        /// <returns>Lista de apartamentos reservados no período</returns>
        public async Task<IEnumerable<ApartamentosReservado>> ObterReservasPorPeriodoAsync(DateTime dataInicio, DateTime dataFim)
        {
            return await _context.ApartamentosReservados
                .Include(ar => ar.Reservas)
                    .ThenInclude(r => r.Empresas)
                .Include(ar => ar.Apartamentos)
                .Include(ar => ar.Clientes)
                .Include(ar => ar.TipoHospedagens)
                .Include(ar => ar.Utilizadores)
                .Where(ar => ar.IsActive && 
                           ar.DataEntrada <= dataFim && 
                           ar.DataSaida >= dataInicio)
                .OrderBy(ar => ar.DataEntrada)
                .ThenBy(ar => ar.ApartamentosId)
                .ToListAsync();
        }

        /// <summary>
        /// Obtém reservas de um apartamento específico incluindo dados da tabela Reserva
        /// </summary>
        /// <param name="apartamentoId">ID do apartamento</param>
        /// <returns>Lista de reservas do apartamento</returns>
        public async Task<IEnumerable<ApartamentosReservado>> ObterReservasPorApartamentoAsync(int apartamentoId)
        {
            return await _context.ApartamentosReservados
                .Include(ar => ar.Reservas)
                    .ThenInclude(r => r.Empresas)
                .Include(ar => ar.Apartamentos)
                .Include(ar => ar.Clientes)
                .Include(ar => ar.TipoHospedagens)
                .Include(ar => ar.Utilizadores)
                .Where(ar => ar.IsActive && ar.ApartamentosId == apartamentoId)
                .OrderBy(ar => ar.DataEntrada)
                .ToListAsync();
        }

        /// <summary>
        /// Obtém uma reserva específica com todos os dados relacionados
        /// </summary>
        /// <param name="reservaId">ID da reserva</param>
        /// <returns>Reserva com dados completos</returns>
        public async Task<Reserva> ObterReservaComDetalhesAsync(int reservaId)
        {
            return await _context.Reservas
                .Include(r => r.Empresas)
                .Include(r => r.Utilizadores)
                .Include(r => r.ApartamentosReservados)
                    .ThenInclude(ar => ar.Apartamentos)
                .Include(r => r.ApartamentosReservados)
                    .ThenInclude(ar => ar.Clientes)
                .Include(r => r.ApartamentosReservados)
                    .ThenInclude(ar => ar.TipoHospedagens)
                .Include(r => r.ApartamentosReservados)
                    .ThenInclude(ar => ar.Utilizadores)
                .FirstOrDefaultAsync(r => r.Id == reservaId && r.IsActive);
        }
    }
}