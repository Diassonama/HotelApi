using System;
using System.Collections.Generic;
using System.Data;
//using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Application.Interfaces;
using Hotel.Domain.Entities;
using Hotel.Domain.Interface;
using Hotel.Domain.Interface.Shared;
using Hotel.Infrastruture.Persistence.Context;
using Hotel.Infrastruture.Persistence.Shared;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Hotel.Infrastruture.Persistence.Repositories
{
    public class SeriesRepository : RepositoryBase<Series>, ISeriesRepository
    {
        private readonly GhotelDbContext _context;

      //  private readonly IConfiguration _configuration;
      //  private readonly TenantContext _tenantContext;

        private readonly ITenantService _tenant;
        public SeriesRepository(GhotelDbContext context) : base(context)
        {
            _context = context;
        //    _configuration = configuration;
        //    _tenantContext = tenantContext;
          //  _tenant = tenant;
        }
        public async Task<IEnumerable<Series>> GetAsync()
        {
            return await _context.Series
                                              .AsNoTracking()
                              .ToListAsync();
        }
        public async Task<Series> GetByIdAsync(int Id)
        {
            return await _context.Series
                                                                  .AsNoTracking()
                              .FirstOrDefaultAsync(p => p.Id == Id);

        }

        public async Task<IPaginatedList<Series>> GetFilteredquery(Domain.Interface.Shared.PaginationFilter paginationFilter)
        {
            var aux = await IPaginatedList<Series>.ToPagedList(
             _context.Series
                     .AsNoTracking()
                     .Where(r => r.Id.ToString().ToLower().Contains(!string.IsNullOrWhiteSpace(paginationFilter.FieldFilter) ? paginationFilter.FieldFilter.ToLower() : "")

                                 )
           //      .ToListAsync();
           , paginationFilter.PageNumber, paginationFilter.PageSize);

            return aux;
        }

        public IQueryable GetFilteredAsync(Domain.Interface.Shared.PaginationFilter paginationFilter)
        {
            IQueryable<Series> query = Enumerable.Empty<Series>().AsQueryable();
            query = (from apart in _context.Series
                                           .AsNoTracking()
                                     .Where(r => r.Id.ToString().ToLower().Contains(!string.IsNullOrWhiteSpace(paginationFilter.FieldFilter) ? paginationFilter.FieldFilter.ToLower() : ""))
                     select apart);
            return query;
        }

        public async Task<int> NumeradorAsync(string tipoDoc, int ano)
        {
            // Localiza a série correspondente ao tipo de documento e ano fornecidos
            var clienteConsultado = await _context.Series
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.TipoDoc == tipoDoc && p.Ano == ano);

            if (clienteConsultado == null)
            {
                // Lança exceção ou retorna valor padrão caso o cliente não seja encontrado
                throw new InvalidOperationException("Nenhuma série encontrada para o tipo de documento e ano especificados.");
            }

            // Incrementa o numerador
            clienteConsultado.Numerador += 1;

            // Usa uma transação para garantir consistência em caso de falhas
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Atualiza o estado da entidade
                    _context.Entry(clienteConsultado).State = EntityState.Modified;

                    // Salva as alterações no banco de dados
                    await _context.SaveChangesAsync();

                    // Confirma a transação
                    await transaction.CommitAsync();
                }
                catch
                {
                    // Reverte a transação em caso de erro
                    await transaction.RollbackAsync();
                    throw; // Repassa a exceção
                }
            }

            // Retorna o novo valor do numerador
            return clienteConsultado.Numerador;
        }


        public void CriarSerieAsync()
        {
            _context.Database.ExecuteSqlRaw("EXEC CriaSerie");
            /* var connectionString = _tenant.GetCurrentTenant().ConnectionString; //_configuration.GetConnectionString("DefaultConnection");
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("CriaSerie", conn))
                {
                    // Define o comando como texto
                    cmd.CommandType = CommandType.StoredProcedure;

                    try
                    {
                        // Abre a conexão
                        conn.Open();

                        // Executa o comando
                        int rowsAffected = cmd.ExecuteNonQuery();

                        // Verifica se algum registro foi atualizado
                    //    if (rowsAffected == 0)
                    //        Console.WriteLine("Nenhum registro foi atualizado. Verifique os critérios da consulta.");
                    }
                    catch (SqlException ex)
                    {
                        // Log do erro para análise
                        Console.WriteLine($"Erro ao atualizar o serie: {ex.Message}");
                        throw; // Re-lança a exceção para ser tratada em outro nível, se necessário
                    } */
              //  }
           // }
        }


        public async Task CriarSerieAsync2()
        {
            // Obtém o ano letivo da tabela Param
            //    var anoExercicio = await _context.Params.FirstOrDefaultAsync();
            /*  .Where(p => p.Id == 1)
             .Select(p => p.DataInicio.Date.Year)
             .FirstOrDefaultAsync(); */
            var ano = 2024; //anoExercicio.DataInicio.Date.Year;


            /*       if (anoExercicio.ToString() == null)
                  {
                      throw new InvalidOperationException("Ano letivo não encontrado na tabela Param.");
                  } */

            // Seleciona os documentos que não possuem uma entrada correspondente na tabela Series para o ano atual
            var documentosFaltantes = await _context.DocumentosVendas
                .Where(dv => !_context.Series.Any(s => s.TipoDoc == dv.Documento && s.Ano == ano))
                .Select(dv => dv.Documento)
                .ToListAsync();

            if (!documentosFaltantes.Any())
            {
                // Caso não existam documentos faltantes, retorna sem realizar operações
                return;
            }

            // Cria uma lista de novas séries para inserir
            var novasSeries = documentosFaltantes.Select(documento => new Series
            {
                TipoDoc = documento,
                Serie = ano.ToString(),//anoExercicio.ToString(),
                Descricao = $"SERIE {ano}",
                Numerador = 0,
                Ano = ano, //anoExercicio.DataInicio.Date.Year,
                NumVias = "1",
                AutoFacturacao = true
            }).ToList();

            // Adiciona as novas séries ao contexto
            await _context.Series.AddRangeAsync(novasSeries);

            // Salva as alterações no banco de dados
            await _context.SaveChangesAsync();
        }

    }
}