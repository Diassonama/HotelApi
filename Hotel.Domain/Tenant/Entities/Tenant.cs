using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Domain.Tenant.Entities
{
    public class Tenant
    {
        public string Id { get; private set; } // Identificador do tenant (e.g., "tenant1")
        public string Name { get; private set; } // Nome do tenant
        public string DatabaseServerName { get; private set; }
        public string UserID { get; private set; }
        public string Password { get; private set; }
        public string DatabaseName { get; private set; }
        public string ConnectionString { get; private set; }
        public TenantMetadata Metadata { get; private set; } 

        public Tenant() { } // Construtor privado para impedir instanciação sem validação

        public Tenant(string id, string name, string databaseServerName, string userId, string password, string databaseName, TenantMetadata metadata)
        {
            Validate(id, name, databaseServerName, userId, password, databaseName, metadata);

            Id = id;
            Name = name;
            DatabaseServerName = databaseServerName;
            UserID = userId;
            Password = password;
            DatabaseName = databaseName;
            Metadata = metadata;
            ConnectionString = BuildConnectionString();
        }

        private void Validate(string id, string name, string databaseServerName, string userId, string password, string databaseName, TenantMetadata metadata)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("ID é obrigatório.");
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Nome é obrigatório.");
            if (string.IsNullOrWhiteSpace(databaseServerName))
                throw new ArgumentException("Database server name é obrigatório.");
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("User ID é obrigatório.");
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password é obrigatório.");
            if (string.IsNullOrWhiteSpace(databaseName))
                throw new ArgumentException("Database name é obrigatório.");
            if (metadata == null)
                throw new ArgumentNullException(nameof(metadata));
        }

        private string BuildConnectionString()
        {
            return $"Data Source={DatabaseServerName};Initial Catalog={DatabaseName};User Id={UserID};Password={Password};Encrypt=false";
        }

        public void UpdateMetadata(TenantMetadata newMetadata)
        {
            if (newMetadata == null)
                throw new ArgumentNullException(nameof(newMetadata));

            Metadata = newMetadata;
        }
    }
}