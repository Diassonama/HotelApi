//using System;
//using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
//using System.Linq;
//using System.Threading.Tasks;
using Hotel.Application.Interfaces;
using Hotel.Domain.Entities;
using Hotel.Domain.Interface;
using Hotel.Domain.Tenant.Entities;
using Hotel.Infrastruture.Persistence.Context;
using Hotel.Infrastruture.Persistence.Shared;
using Microsoft.EntityFrameworkCore;

//using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Hotel.Infrastruture.Persistence.Repositories
{
    public class SerialRepository : RepositoryBase<Serial>, ISerialRepository
    {
        private readonly IConfiguration _configuration;
        private readonly GhotelDbContext _context;
        private readonly TenantContext _tenantContext;

        private readonly ITenantService _tenant;

        public SerialRepository(IConfiguration configuration, GhotelDbContext context, ITenantService tenant, TenantContext tenantContext) : base(context)
        {
            _configuration = configuration;
            _context = context;
            _tenant = tenant;
            _tenantContext = tenantContext;
        }

        public async Task<Serial> GetByIdAsync(int id)
        {

            try
            {
                return await _context.Serial.AsNoTracking().FirstOrDefaultAsync(p => p.NumSerial == id);
                //   return await _context.Serial.FindAsync(id);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar Serial com Id={id}: {ex.Message}");
                throw;
            }
        }
        public void UpdateSerialSistema4(string Valor)
        {
            try
            {
                var tenantExist = _tenantContext.Tenants.AsNoTracking().FirstOrDefault(p => p.Id == _tenant.GetCurrentTenant().Id);

                if (tenantExist == null)
                    throw new ArgumentException("Dados não encontrado.");

                tenantExist.Metadata.SetKeySerial(Valor);

                // Attach the entity and mark it as modified
                _tenantContext.Attach(tenantExist);
                _tenantContext.Entry(tenantExist).State = EntityState.Modified;

                // Save changes to the database
                _tenantContext.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao atualizar Serial: {ex.Message}");
                throw;
            }
        }

        public async void UpdateSerialSistema0(string Valor)
        {
            try
            {
                // var tenantId = _tenant.GetCurrentTenant();
                var tenantExist = await _tenantContext.Tenants.FirstOrDefaultAsync(p => p.Id == _tenant.GetCurrentTenant().Id);

                if (tenantExist == null)
                    throw new ArgumentException("Dados não encontrado.");

                //   var metadata = new TenantMetadata(tenantExist.Metadata.Region, tenantExist.Metadata.MaxUsers, tenantExist.Metadata.IsActive, tenantExist.Metadata.CustomSettings); //, tenantExist.Metadata.CustomSettings
                tenantExist.Metadata.SetKeySerial(Valor);
                //   var tenant = new Tenant(tenantExist.Id, tenantExist.Name, tenantExist.DatabaseServerName, tenantExist.UserID, tenantExist.Password, tenantExist.DatabaseName, metadata);
                _tenantContext.Entry(tenantExist).State = EntityState.Modified;
                await _tenantContext.SaveChangesAsync();
                // _context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao atualizar Serial: {ex.Message}");
                throw;
            }

        }

        public void UpdateSerialSistema(string valor)
        {
            // Valida o valor para evitar problemas
            if (string.IsNullOrWhiteSpace(valor))
                throw new ArgumentException("O valor do serial não pode ser nulo ou vazio.", nameof(valor));

            // Conexão com o banco de dados
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("UPDATE Tenant SET Metadata_KeySerial = @Valor where Id =@Id", conn))
                {
                    // Define o comando como texto
                    cmd.CommandType = CommandType.Text;

                    // Adiciona o parâmetro com segurança
                    cmd.Parameters.Add("@Valor", SqlDbType.NVarChar).Value = valor;
                    cmd.Parameters.Add("@Id", SqlDbType.NVarChar).Value = _tenant.GetCurrentTenant().Id;

                    try
                    {
                        // Abre a conexão
                        conn.Open();

                        // Executa o comando
                        int rowsAffected = cmd.ExecuteNonQuery();

                        // Verifica se algum registro foi atualizado
                        if (rowsAffected == 0)
                            Console.WriteLine("Nenhum registro foi atualizado. Verifique os critérios da consulta.");
                    }
                    catch (SqlException ex)
                    {
                        // Log do erro para análise
                        Console.WriteLine($"Erro ao atualizar o serial: {ex.Message}");
                        throw; // Re-lança a exceção para ser tratada em outro nível, se necessário
                    }
                }
            }
        }
public int Prazo (){
    return _tenant.GetCurrentTenant().Metadata.Prazo;
}
public int MaxUsers (){
    return _tenant.GetCurrentTenant().Metadata.MaxUsers;
}

public Dictionary<string,string> CustomSettings (){
    return _tenant.GetCurrentTenant().Metadata.CustomSettings;
}

 
        public string GetKeySerial()
        {
            string serial = string.Empty;
            var tenantId = _tenant.GetCurrentTenant();
            if (string.IsNullOrEmpty(tenantId.ToString()))
            {
                throw new InvalidCastException("Tenant não pode ser null.");
            }
            serial = tenantId.Metadata.KeySerial;
            return serial;

        }

        public void Apagar()
        {
            _context.Serial.RemoveRange(_context.Serial);
            _context.SaveChanges();
            //   _context.Database.ExecuteSqlRawAsync("DELETE FROM Serial");
        }

        public async Task AddAsync(Serial serial)
        {
            try
            {
                // Adiciona a entidade ao contexto
                await _context.Serial.AddAsync(serial);

                // Salva as alterações no banco de dados
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Tratar erros
                Console.WriteLine($"Erro ao adicionar a entidade: {ex.Message}");
                throw;
            }
        }
    }
}
