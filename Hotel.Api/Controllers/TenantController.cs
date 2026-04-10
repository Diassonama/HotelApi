using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Application.DTOs.Request;
using Hotel.Application.Helper;
using Hotel.Application.Interfaces;
using Hotel.Domain.Tenant.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;


namespace Hotel.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TenantController : ControllerBase
    {
        private readonly ITenantService _tenantService;

        public TenantController(ITenantService tenantService)
        {
            _tenantService = tenantService;
        }
        //    [HttpGet]
        /*   public async Task<IActionResult> GetTenant()
          {
              var tenantSubdomain = HttpContext.Items["Tenant"]?.ToString();

              if (string.IsNullOrEmpty(tenantSubdomain))
              {
                  return BadRequest(new { Error = "Subdomínio não encontrado" });
              }

              var tenant = _tenantService.GetCurrentTenant();  //    GetTenantBySubdomainAsync(tenantSubdomain);
              if (tenant == null)
              {
                  return NotFound(new { Error = "Inquilino não encontrado" });
              }

              return Ok(tenant);
          } */

        [HttpGet]
        public async Task<IActionResult> GetAll()

        {

            // Ok(await _tenantService.GetAllAsync());

            var tenant = await _tenantService.GetAllAsync();
            if (tenant == null)
                return NotFound();

            return Ok(tenant);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id) => Ok(await _tenantService.GetByIdAsync(id));

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTenantRequest request)
        {
            var metadata = new TenantMetadata(request.Metadata.Region, request.Metadata.MaxUsers, request.Metadata.IsActive, request.Metadata.CustomSettings); //, request.Metadata.CustomSettings
            var tenant = new Tenant(request.Id, request.Name, request.DatabaseServerName, request.UserID, request.Password, request.DatabaseName, metadata);
            await _tenantService.CreateTenantAsync(tenant);
            return CreatedAtAction(nameof(GetById), new { id = request.Id }, null);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] CreateTenantRequest request)
        {
            var metadata = new TenantMetadata(request.Metadata.Region, request.Metadata.MaxUsers, request.Metadata.IsActive, request.Metadata.CustomSettings); //, request.Metadata.CustomSettings
            var tenant = new Tenant(request.Id, request.Name, request.DatabaseServerName, request.UserID, request.Password, request.DatabaseName, metadata);

            await _tenantService.UpdateAsync(tenant);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _tenantService.DeleteAsync(id);
            return NoContent();
        }

    }
}