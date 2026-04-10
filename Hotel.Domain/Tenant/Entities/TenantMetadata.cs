using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Hotel.Domain.Tenant.Entities
{
    public class TenantMetadata
    {
        public string Region { get; set; } // Região do tenant
        public int MaxUsers { get; set; } // Limite de usuários
        public bool IsActive { get; set; } = true; // Status padrão é ativo
        public int Prazo { get; private set; }
        public string KeySerial { get; private set; }
        public Dictionary<string, string> CustomSettings { get; set; } = new(); // Armazena o JSON como objeto
        protected TenantMetadata() { }
        public TenantMetadata(string region, int maxUsers, bool isActive, Dictionary<string, string> customSettings = null)
        {
            Validate(region, maxUsers);
            Region = region;
            MaxUsers = maxUsers;
            IsActive = isActive;

            CustomSettings = customSettings ?? new Dictionary<string, string>();
        }
        private void Validate(string region, int maxUsers)
        {
            if (string.IsNullOrWhiteSpace(region))
                throw new ArgumentException("Region is required.");
            if (maxUsers <= 0)
                throw new ArgumentException("MaxUsers must be greater than zero.");
        }
       public void SetKeySerial( string keySerial)
        {
            KeySerial = KeySerial;
        }
         public void SetPrazo( int prazo = 365)
        {
            Prazo = prazo;
        }
        public void Deactivate()
        {
            IsActive = false;
        }
        public void Activate()
        {
            IsActive = true;
        }
      
    }
}