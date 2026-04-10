using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Application.DTOs.Response;
using Hotel.Application.Interfaces;
using Hotel.Domain.Entities;
using Hotel.Domain.Interface;
using Hotel.Infrastruture.Persistence.Context;
using Hotel.Infrastruture.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SerialController : ControllerBase
    {
        private readonly ISerialService _serialService;
        private GhotelDbContext _context;
        private readonly ISerialRepository _serialRepository;

        public SerialController(ISerialService serialService, GhotelDbContext context, ISerialRepository serialRepository)
        {
            _serialService = serialService;
            _context = context;
            _serialRepository = serialRepository;
        }

        [HttpGet]
        public async Task<SerialResponse> ObterSerialSistema() {

            int Contador = 0;
            int Prazo = 0;

            if (_serialService.fncRegistrado())
            {
               
                if (await _serialService.ValidateLicenseComAsync(_serialService.prazoValidade()))
                {
                    Prazo = _serialService.prazoValidade();
                }
            }
            else
            {
                
                if (await _serialService.ValidateLicenseAsync())
                {
                    Prazo = _serialService.prazoTrial();                   
                }
            }

            Contador = await _serialService.fncTempoDeBloqueio(Prazo);

            return new SerialResponse()
            {
                contador = Contador,
                Prazo = Prazo

            };
        }

        [HttpGet("senha")]
        public void Get(string senha)
        {
            if (senha == "DFASOFT")
            {

            _serialRepository.Apagar();
            int cont = 0;
            var serial = new Serial(int.Parse(_serialService.GetDriveSerialNumber()), string.Concat(cont.ToString(), ",", DateTime.Now.Date.ToShortDateString()), DateTime.Now.Date.ToShortDateString(), _serialService.prazoTrial());
            serial.SetChave (_serialService.GenerateDriveKey(4));
             _context.Add(serial);
             _context.SaveChanges();

                _serialService.fncReiniciar();
            }
        } 
    }
}