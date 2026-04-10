using System;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Api.Controllers.Shared;
using Hotel.Domain.Entities;
using Hotel.Domain.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Hotel.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProductTypeController : ApiControllerBase
    {
        private readonly ILogger<ProductTypeController> _logger;

        public ProductTypeController(IUnitOfWork unitOfWork, ILogger<ProductTypeController> logger) : base(unitOfWork)
        {
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var productTypes = await _unitOfWork.GetRepository<ProductType>().GetAllAsync();
                return Ok(productTypes.OrderBy(x => x.ProductTypeCode));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar ProductTypes");
                return StatusCode(500, new { message = "Erro interno ao listar ProductTypes." });
            }
        }

        [HttpGet("{code}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByCode(string code)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(code))
                {
                    return BadRequest(new { message = "ProductTypeCode é obrigatório." });
                }

                var normalizedCode = code.Trim().ToUpperInvariant();
                var productType = (await _unitOfWork.GetRepository<ProductType>().GetAllAsync())
                    .FirstOrDefault(x => x.ProductTypeCode == normalizedCode);

                if (productType == null)
                {
                    return NotFound(new { message = $"ProductType com código '{normalizedCode}' não encontrado." });
                }

                return Ok(productType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar ProductType por código {Code}", code);
                return StatusCode(500, new { message = "Erro interno ao buscar ProductType." });
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] CreateProductTypeRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new { message = "Dados da requisição são obrigatórios." });
                }

                if (!IsValidCode(request.ProductTypeCode, out var normalizedCode, out var codeError))
                {
                    return BadRequest(new { message = codeError });
                }

                if (string.IsNullOrWhiteSpace(request.ProductTypeDescription))
                {
                    return BadRequest(new { message = "ProductTypeDescription é obrigatório." });
                }

                var repository = _unitOfWork.GetRepository<ProductType>();
                var exists = (await repository.GetAllAsync()).Any(x => x.ProductTypeCode == normalizedCode);
                if (exists)
                {
                    return Conflict(new { message = $"Já existe ProductType com código '{normalizedCode}'." });
                }

                var entity = new ProductType
                {
                    ProductTypeCode = normalizedCode,
                    ProductTypeDescription = request.ProductTypeDescription.Trim(),
                    CaminhoImagem = request.CaminhoImagem?.Trim() ?? string.Empty
                };

                await repository.Add(entity);
                await _unitOfWork.Save();

                return CreatedAtAction(nameof(GetByCode), new { code = entity.ProductTypeCode }, entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar ProductType");
                return StatusCode(500, new { message = "Erro interno ao criar ProductType." });
            }
        }

        [HttpPut("{code}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(string code, [FromBody] UpdateProductTypeRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new { message = "Dados da requisição são obrigatórios." });
                }

                if (!IsValidCode(code, out var normalizedCode, out var codeError))
                {
                    return BadRequest(new { message = codeError });
                }

                if (string.IsNullOrWhiteSpace(request.ProductTypeDescription))
                {
                    return BadRequest(new { message = "ProductTypeDescription é obrigatório." });
                }

                var repository = _unitOfWork.GetRepository<ProductType>();
                var entity = (await repository.GetAllAsync()).FirstOrDefault(x => x.ProductTypeCode == normalizedCode);
                if (entity == null)
                {
                    return NotFound(new { message = $"ProductType com código '{normalizedCode}' não encontrado." });
                }

                entity.ProductTypeDescription = request.ProductTypeDescription.Trim();
                entity.CaminhoImagem = request.CaminhoImagem?.Trim() ?? string.Empty;

                await repository.Update(entity);
                await _unitOfWork.Save();

                return Ok(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar ProductType com código {Code}", code);
                return StatusCode(500, new { message = "Erro interno ao atualizar ProductType." });
            }
        }

        [HttpDelete("{code}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(string code)
        {
            try
            {
                if (!IsValidCode(code, out var normalizedCode, out var codeError))
                {
                    return BadRequest(new { message = codeError });
                }

                var repository = _unitOfWork.GetRepository<ProductType>();
                var entity = (await repository.GetAllAsync()).FirstOrDefault(x => x.ProductTypeCode == normalizedCode);
                if (entity == null)
                {
                    return NotFound(new { message = $"ProductType com código '{normalizedCode}' não encontrado." });
                }

                await repository.Delete(entity);
                await _unitOfWork.Save();

                return Ok(new { message = "ProductType removido com sucesso." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao remover ProductType com código {Code}", code);
                return StatusCode(500, new { message = "Erro interno ao remover ProductType." });
            }
        }

        private static bool IsValidCode(string rawCode, out string normalizedCode, out string error)
        {
            normalizedCode = null;
            error = null;

            if (string.IsNullOrWhiteSpace(rawCode))
            {
                error = "ProductTypeCode é obrigatório.";
                return false;
            }

            normalizedCode = rawCode.Trim().ToUpperInvariant();
            if (normalizedCode.Length != 1)
            {
                error = "ProductTypeCode deve ter exatamente 1 caractere.";
                return false;
            }

            return true;
        }

        public class CreateProductTypeRequest
        {
            public string ProductTypeCode { get; set; }
            public string ProductTypeDescription { get; set; }
            public string CaminhoImagem { get; set; }
        }

        public class UpdateProductTypeRequest
        {
            public string ProductTypeDescription { get; set; }
            public string CaminhoImagem { get; set; }
        }
    }
}