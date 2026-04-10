using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Application.Interfaces
{
    public interface ISerialService
    {
        Task<bool> ValidateLicenseAsync();
        Task<bool> ValidateLicenseComAsync(int prazo = 0);
        Task<int> fncTempoDeBloqueio(int intPrazo);
        Boolean fncRegistrado();
        void fncReiniciar();
        bool SetComercialFlag(bool valor);
        int prazoValidade();
        int prazoTrial();
        string GetDriveSerialNumber();
        string GenerateDriveKey(byte digitCount);
    }
}