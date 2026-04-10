
using System.Data;
using System.Net;
using System.Text.Json;
using Hotel.Application.Common.Exceptions;
using Hotel.Application.Responses;
using Microsoft.AspNetCore.Http;

namespace Hotel.Application.Middleware
{
    public class GlobalErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public GlobalErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
           /*  await _next(context);
            HttpStatusCode status;
            string stackTrace = string.Empty;
            string message = string.Empty;
            var excetionType = exception.GetType(); */

             try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
               // await HandlerExceptionAsync(context, ex);
               await HandleExceptionAsync(context, ex);
            } 

            /*
                   string resposta = string.Empty;
                        try
                        {
                            await _next(context);
                        }

                        catch (NotFoundException ex)
                        {
                            context.Response.StatusCode = StatusCodes.Status404NotFound;
                          //  message = exception.Message;
                          //  await  context.Response.WriteAsync(new BaseCommandResponse { Success = false, Message = ex.Message });
                               resposta = JsonSerializer.Serialize(new BaseCommandResponse { Success = false, Message = ex.Message });
                        }
                        catch (DBConcurrencyException ex)
                        {
                          //  status = HttpStatusCode.BadRequest;
                          //  message = exception.Message;
                            resposta = JsonSerializer.Serialize(new BaseCommandResponse { Success = false, Message = ex.Message });

                        }
                        catch (UnauthorizedAccessException)
                        {
                            // Handle unauthorized access
                          //  context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                          //  message = "Acesso não Autorizado";
                            resposta = JsonSerializer.Serialize(new BaseCommandResponse { Success = false, Message = "Acesso não Autorizado" });
                        }
                        catch (Exception ex)
                        {
                            // Handle other unhandled exceptions
                            // Log the exception for debugging purposes.
                            Console.WriteLine($"Unhandled Exception: {ex}");

                            // Customize the error response as needed.
                            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                         //   message = exception.Message;
                            resposta = JsonSerializer.Serialize(new BaseCommandResponse { Success = false, Message = ex.Message });

                            //  context.Response.WriteAsync(new BaseCommandResponse { Success = false, Message = "An error occurred while processing your request." });
                        }
                       // var response = JsonSerializer.Serialize(new BaseCommandResponse { Success = false, Message = message });
                        context.Response.ContentType = "Application/json";
                        // context.Response.StatusCode = (int)status;

                        return context.Response.WriteAsync(resposta);*/
                        
        }


private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
     //   _logger.LogError(exception, "An unexpected error occurred.");

        //More log stuff        
         string resposta = string.Empty;

        BaseCommandResponse response = exception switch
        {
            ApplicationException  _ => resposta = JsonSerializer.Serialize(new BaseCommandResponse { Success = false, Message = $"{HttpStatusCode.BadRequest} - The request key not found." }),
            NotFoundException _ => resposta = JsonSerializer.Serialize(new BaseCommandResponse { Success = false, Message = $"{HttpStatusCode.NotFound} - Registro não encontrado." }),
            DBConcurrencyException  _ => resposta = JsonSerializer.Serialize(new BaseCommandResponse { Success = false, Message = $"{HttpStatusCode.BadRequest} - Erro encontrado" }),
            NotImplementedException _ => resposta = JsonSerializer.Serialize(new BaseCommandResponse { Success = false, Message = $"{HttpStatusCode.BadRequest} - Erro encontrado" }),


            KeyNotFoundException _ =>  resposta = JsonSerializer.Serialize(new BaseCommandResponse { Success = false, Message = "The request key not found." }), //new BaseCommandResponse(HttpStatusCode.NotFound, "The request key not found."),
            UnauthorizedAccessException _ =>  resposta = JsonSerializer.Serialize(new BaseCommandResponse { Success = false, Message = $"{HttpStatusCode.Unauthorized} - Unauthorized." }), //new BaseCommandResponse(HttpStatusCode.Unauthorized, "Unauthorized."),
            _ =>  resposta = JsonSerializer.Serialize(new BaseCommandResponse { Success = false, Message = $"{HttpStatusCode.InternalServerError} - Internal server error. Please retry later." }) //new BaseCommandResponse(HttpStatusCode.Unauthorized, "Unauthorized."),
            // BaseCommandResponse(HttpStatusCode.InternalServerError, "Internal server error. Please retry later.")
        };

        context.Response.ContentType = "application/json";
      //  context.Response.StatusCode = (int)response.StatusCode;
        await context.Response.WriteAsync(resposta);
    }

        private static Task HandlerExceptionAsync(HttpContext context, Exception exception)
        {
            HttpStatusCode status;
            string stackTrace = string.Empty;
            string message = string.Empty;
            var excetionType = exception.GetType();

            if (excetionType == typeof(DBConcurrencyException))
            {
                status = HttpStatusCode.BadRequest;
                message = exception.Message;
                stackTrace = exception.StackTrace;
            }
            else if (excetionType == typeof(NotFoundException))
            {
                status = HttpStatusCode.NotFound;
                message = exception.Message;
                stackTrace = exception.StackTrace;
            }
            else
            {
                status = HttpStatusCode.InternalServerError;
                message = exception.Message;
                stackTrace = exception.StackTrace;
            }


            var response = JsonSerializer.Serialize(new BaseCommandResponse { Success = false, Message = message });
            context.Response.ContentType = "Application/json";
            context.Response.StatusCode = (int)status;

            return context.Response.WriteAsync(response);

            /*
                        try

                        catch (NotFoundException)
                        {
                            context.Response.StatusCode = StatusCodes.Status404NotFound;
                            message = exception.Message;
                            //  context.Response.WriteAsJsonAsync(new BaseCommandResponse { Success = false, Message = ex.Message });
                            //  var resposta = JsonSerializer.Serialize(new BaseCommandResponse { Success = false, Message = ex.Message });
                        }
                        catch (DBConcurrencyException)
                        {
                            status = HttpStatusCode.BadRequest;
                            message = exception.Message;
                        }
                        catch (UnauthorizedAccessException)
                        {
                            // Handle unauthorized access
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            message = "Acesso não Autorizado";
                            //    context.Response.WriteAsJsonAsync(new BaseCommandResponse { Success = false, Message = "Unauthorized access." });
                        }
                        catch (Exception ex)
                        {
                            // Handle other unhandled exceptions
                            // Log the exception for debugging purposes.
                            Console.WriteLine($"Unhandled Exception: {ex}");

                            // Customize the error response as needed.
                            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                            message = exception.Message;
                            //  context.Response.WriteAsync(new BaseCommandResponse { Success = false, Message = "An error occurred while processing your request." });
                        }
                        var response = JsonSerializer.Serialize(new BaseCommandResponse { Success = false, Message = message });
                        context.Response.ContentType = "Application/json";
                        // context.Response.StatusCode = (int)status;

                        return context.Response.WriteAsync(response);

             */
        }

    }
}