using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Entities;
using Hotel.Domain.Interface.Shared;

namespace Hotel.Domain.Interface
{
    public interface ISerialRepository: IRepositoryBase<Serial>
    {
        Task<Serial> GetByIdAsync(int Id);
        void UpdateSerialSistema(string Valor);
        string GetKeySerial();
        void Apagar();
        Task AddAsync(Serial serial);
        Dictionary<string,string> CustomSettings ();
        int Prazo();
        int MaxUsers();
    }
}