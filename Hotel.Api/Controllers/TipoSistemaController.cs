using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Hotel.Api.Controllers.Shared;
using Hotel.Application.Responses;
using Hotel.Domain.Interface;
using Hotel.Application.TaxAccountingBasis.Queries;




namespace Hotel.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TipoSistemaController : ApiControllerBase
    {
        public TipoSistemaController(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        [HttpGet]
        public async Task<BaseCommandResponse> Get()
        {
            return await Mediator.Send(new GetTaxAccountingBasisQuery());
        }
    }
}