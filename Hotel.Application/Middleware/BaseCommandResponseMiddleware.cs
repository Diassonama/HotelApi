
using System.ComponentModel.DataAnnotations;
using System.Data;
using Hotel.Application.Common.Exceptions;
using Hotel.Application.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


namespace Hotel.Application.Middleware
{
    public class BaseCommandResponseMiddleware
    {
        /* 


        private readonly RequestDelegate _next;
        private readonly IHostEnvironment _environment;

            public BaseCommandResponseMiddleware(RequestDelegate next, IHostEnvironment environment)
            {
                _next = next;
                _environment = environment;

            }

            public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var statusCode = exception switch
            {
                UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
                NotFoundException => StatusCodes.Status404NotFound,
                DBConcurrencyException => StatusCodes.Status409Conflict,
                ApplicationException => StatusCodes.Status400BadRequest,
                ValidationException => StatusCodes.Status400BadRequest,
                ArgumentNullException => StatusCodes.Status400BadRequest,
                FormatException => StatusCodes.Status400BadRequest,
                TimeoutException => StatusCodes.Status408RequestTimeout,
                AccessViolationException => StatusCodes.Status500InternalServerError,
                _ => StatusCodes.Status500InternalServerError
            };

            context.Response.StatusCode = statusCode;

            var showDetails = true; //_environment.IsDevelopment(); // Exibe detalhes apenas em ambiente de desenvolvimento


            // Cria a resposta com BaseCommandResponse
            var response = new BaseCommandResponse
            {
                Success = false,
                Message = GetFriendlyMessage(statusCode),
                Errors = new List<string> { exception.Message },
                Data = null,
                StackTrace = showDetails ? exception.StackTrace : null, // Adiciona StackTrace somente se ShowDetails for true
                ShowDetails = showDetails
            };

            var responseJson = System.Text.Json.JsonSerializer.Serialize(response);

            return context.Response.WriteAsync(responseJson);
        }

        private string GetFriendlyMessage(int statusCode)
        {
            return statusCode switch
            {
                StatusCodes.Status401Unauthorized => "Você não está autorizado a realizar esta operação.",
                StatusCodes.Status404NotFound => "O recurso solicitado não foi encontrado.",
                StatusCodes.Status409Conflict => "Ocorreu um conflito ao acessar os dados. Tente novamente.",
                StatusCodes.Status400BadRequest => "Houve um problema com a solicitação enviada.",
                StatusCodes.Status408RequestTimeout => "A operação demorou muito tempo e foi encerrada.",
                StatusCodes.Status500InternalServerError => "Ocorreu um erro inesperado no servidor.",
                _ => "Erro desconhecido."
            };
        }
    }

    } */

        private readonly RequestDelegate _next;
        private readonly IHostEnvironment _environment;
        private readonly ILogger<BaseCommandResponseMiddleware> _logger;

        public BaseCommandResponseMiddleware(RequestDelegate next, IHostEnvironment environment, ILogger<BaseCommandResponseMiddleware> logger)
        {
            _next = next;
            _environment = environment;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado capturado no middleware.");
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var statusCode = exception switch
            {
                UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
                NotFoundException => StatusCodes.Status404NotFound,
                ValidationException => StatusCodes.Status422UnprocessableEntity,
                TimeoutException => StatusCodes.Status408RequestTimeout,
                ApplicationException => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status500InternalServerError
            };

            context.Response.StatusCode = statusCode;

            var showDetails = true; //_environment.IsDevelopment();

            var response = new BaseCommandResponse
            {
                Success = false,
                Message = GetFriendlyMessage(statusCode),
                Errors = new List<string> { exception.Message },
                Data = null,
                StackTrace = showDetails ? exception.StackTrace : null,
                ShowDetails = showDetails
            };

            var options = new System.Text.Json.JsonSerializerOptions
            {
                PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            };

            var responseJson = System.Text.Json.JsonSerializer.Serialize(response, options);

            return context.Response.WriteAsync(responseJson);
        }

        private string GetFriendlyMessage(int statusCode)
        {
            return statusCode switch
            {
                StatusCodes.Status401Unauthorized => "Você não está autorizado a realizar esta operação.",
                StatusCodes.Status404NotFound => "O recurso solicitado não foi encontrado.",
                StatusCodes.Status422UnprocessableEntity => "Os dados enviados são inválidos.",
                StatusCodes.Status408RequestTimeout => "A operação demorou muito tempo e foi encerrada.",
                StatusCodes.Status500InternalServerError => "Ocorreu um erro inesperado no servidor.",
                _ => "Erro desconhecido."
            };
        }
    }
}