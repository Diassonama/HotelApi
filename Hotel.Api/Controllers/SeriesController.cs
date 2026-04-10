using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Api.Controllers.Shared;
using Hotel.Domain.Interface;
using Hotel.Infrastruture.Persistence.Context;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SeriesController : ApiControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
       
        private readonly ISeriesRepository _repository;
        public SeriesController(IUnitOfWork unitOfWork, ISeriesRepository repository) : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<int>> GetSeries(string tipoDoc, int ano)
        {
            return await _repository.NumeradorAsync(tipoDoc, ano);  //  .SeriesRepository.GetSeriesAsync();
        }
        [HttpPost]  
        public  void PostSeries()
        {
            _repository.CriarSerieAsync();
        }



    }
}