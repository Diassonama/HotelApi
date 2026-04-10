using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Api.Controllers.Shared;
using Hotel.Application.Common.PagedResult;
using Hotel.Application.FacturaEmpresa.Queries;
using Hotel.Application.Responses;
using Hotel.Domain.Dtos;
using Hotel.Domain.DTOs;
using Hotel.Domain.Entities;
using Hotel.Domain.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FacturaEmpresaController : ApiControllerBase
    {
        public FacturaEmpresaController(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
        [HttpGet]
        public async Task<BaseCommandResponse> Get()
        {
            return await Mediator.Send(new GetFacturaEmpresaQuery());
        }

        [HttpGet("get-with-pagination")]
        public async Task<ActionResult<PagedList<FacturaEmpresa>>> GetAllFiltro([FromQuery] GetFilteredQuery query)
        {
            return await Mediator.Send(query);
        }
        [HttpGet("EmpresasComDividas")]
        public async Task<ActionResult<PagedList<FacturaEmpresaDto>>> GetEmpresaComDividaFiltro([FromQuery] GetFilteredEmpresasComDividasQuery query)
        {
            return await Mediator.Send(query);
        }
        [HttpGet("DetalhesDeDividasEmpresa")]
        public async Task<ActionResult<PagedList<FacturaEmpresa>>> GetDetalheFiltro([FromQuery] Domain.Interface.Shared.PaginationFilter paginationFilter)
        {
            return await Mediator.Send(new GetFilteredDetalhesDeDividasEmpresaQuery
            {
                paginationFilter = paginationFilter,
                //  Id = empresaId
            });

            /*   var query = new GetFilteredDetalhesDeDividasEmpresaQuery
             {
                 paginationFilter = paginationFilter,
                 Id = empresaId
             };

             var result = await Mediator.Send(query);

             return Ok(result); */
        }

        [HttpGet("DetalhesDeDividasEmpresaV2")]
        public async Task<ActionResult<PagedList<FacturaEmpresaDetalhesDto>>> GetDetalheFiltroV2([FromQuery] Domain.Interface.Shared.PaginationFilter paginationFilter)
        {
            return await Mediator.Send(new GetFilteredDetalhesDeDividasEmpresaQueryV2
            {
                paginationFilter = paginationFilter,
                //  Id = empresaId
            });


        }



        [HttpGet("DetalhesDeDividasEmpresaComTotal")]
        public async Task<ActionResult<PagedList<FacturaEmpresa>>> GetDetalheTotalFiltro([FromQuery] Domain.Interface.Shared.PaginationFilter paginationFilter)
        {
            var (paginatedData, valorTotal) = await Mediator.Send(new GetFilteredDetalhesDeDividasEmpresaComTotalQuery
            {
                paginationFilter = paginationFilter,
                //  EmpresaId = empresaId
            });

            return Ok(new
            {
                paginatedData,
                ValorTotal = valorTotal
            });
        }

        [HttpGet("{id}")]
        public async Task<BaseCommandResponse> get(int id)
        {
            return await Mediator.Send(new GetFacturaEmpresaByIdQuery { Id = id });

        }

        [HttpGet("CheckinId")]
        public async Task<BaseCommandResponse> getbycheckin(int id)
        {
            return await Mediator.Send(new GetFacturaEmpresaByCheckinIdQuery { Id = id });

        }

        [HttpGet("EmpresaId")]
        public async Task<BaseCommandResponse> getbyEmpresaId(int id)
        {
            return await Mediator.Send(new GetFacturaEmpresaByEmpresaIdQuery { Id = id });

        }

    }
}