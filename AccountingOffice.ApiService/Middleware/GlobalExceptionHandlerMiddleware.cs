using System.Net;
using System.Text.Json;
using AccountingOffice.ApiService.Models;
using Serilog;

namespace AccountingOffice.ApiService.Middleware;

public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IHostEnvironment _environment;

    public GlobalExceptionHandlerMiddleware(RequestDelegate next, IHostEnvironment environment)
    {
        _next = next;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
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

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        var response = context.Response;

        var traceId = context.TraceIdentifier;
        var errorResponse = new ErrorResponse
        {
            StatusCode = (int)HttpStatusCode.InternalServerError,
            Message = "Ocorreu um erro interno no servidor.",
            Details = _environment.IsDevelopment() ? exception.ToString() : null,
            Timestamp = DateTime.UtcNow,
            TraceId = traceId
        };

        switch (exception)
        {
            case ArgumentNullException argNullEx:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.Message = $"Parâmetro obrigatório não fornecido: {argNullEx.ParamName}";
                Log.Warning(exception, "ArgumentNullException: {ParamName}", argNullEx.ParamName);
                break;

            case ArgumentException argEx:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.Message = argEx.Message;
                Log.Warning(exception, "ArgumentException: {Message}", argEx.Message);
                break;

            case InvalidOperationException invalidOpEx:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.Message = invalidOpEx.Message;
                Log.Warning(exception, "InvalidOperationException: {Message}", invalidOpEx.Message);
                break;

            case UnauthorizedAccessException:
                response.StatusCode = (int)HttpStatusCode.Unauthorized;
                errorResponse.StatusCode = (int)HttpStatusCode.Unauthorized;
                errorResponse.Message = "Acesso não autorizado.";
                Log.Warning(exception, "UnauthorizedAccessException");
                break;

            case KeyNotFoundException keyNotFoundEx:
                response.StatusCode = (int)HttpStatusCode.NotFound;
                errorResponse.StatusCode = (int)HttpStatusCode.NotFound;
                errorResponse.Message = keyNotFoundEx.Message;
                Log.Warning(exception, "KeyNotFoundException: {Message}", keyNotFoundEx.Message);
                break;

            case Microsoft.EntityFrameworkCore.DbUpdateException dbEx:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.Message = "Erro ao processar operação no banco de dados.";
                
                if (dbEx.InnerException != null)
                {
                    var innerMessage = dbEx.InnerException.Message;
                    if (innerMessage.Contains("UNIQUE") || innerMessage.Contains("duplicate"))
                    {
                        errorResponse.Message = "Registro duplicado. O item já existe no sistema.";
                    }
                    else if (innerMessage.Contains("FOREIGN KEY") || innerMessage.Contains("constraint"))
                    {
                        errorResponse.Message = "Não é possível realizar esta operação devido a restrições de integridade referencial.";
                    }
                }
                
                Log.Error(exception, "DbUpdateException: {Message}", dbEx.Message);
                break;

            case TimeoutException:
                response.StatusCode = (int)HttpStatusCode.RequestTimeout;
                errorResponse.StatusCode = (int)HttpStatusCode.RequestTimeout;
                errorResponse.Message = "A operação excedeu o tempo limite.";
                Log.Warning(exception, "TimeoutException");
                break;

            default:
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                errorResponse.StatusCode = (int)HttpStatusCode.InternalServerError;
                errorResponse.Message = "Ocorreu um erro interno no servidor.";
                Log.Error(exception, "Unhandled exception: {ExceptionType}", exception.GetType().Name);
                break;
        }

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = _environment.IsDevelopment()
        };

        var jsonResponse = JsonSerializer.Serialize(errorResponse, jsonOptions);
        await response.WriteAsync(jsonResponse);
    }
}

