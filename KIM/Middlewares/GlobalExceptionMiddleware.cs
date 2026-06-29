using FluentValidation;
using KIM.BL.Shared.Exceptions;
using KIM.BL.Shared.Responses;
using System.Text.Json;

namespace KIM.Api.Middlewares;

public class GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException ex)
        {
            var errors = ex.Errors
                .Select(error => new ValidationErrorItem
                {
                    Field = error.PropertyName,
                    Message = error.ErrorMessage
                })
                .ToList();

            await WriteAsync(
                context,
                StatusCodes.Status400BadRequest,
                ApiResponse<object>.Failure(AppCode.ValidationError, "Validation failed", errors));
        }
        catch (NotFoundException ex)
        {
            await WriteAsync(
                context,
                StatusCodes.Status404NotFound,
                ApiResponse<object>.Failure(AppCode.NotFound, ex.Message));
        }
        catch (ConflictException ex)
        {
            await WriteAsync(
                context,
                StatusCodes.Status409Conflict,
                ApiResponse<object>.Failure(AppCode.Conflict, ex.Message));
        }
        catch (ForbiddenOperationException ex)
        {
            await WriteAsync(
                context,
                StatusCodes.Status400BadRequest,
                ApiResponse<object>.Failure(AppCode.ForbiddenOperation, ex.Message));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception");
            await WriteAsync(
                context,
                StatusCodes.Status500InternalServerError,
                ApiResponse<object>.Failure(AppCode.InternalError, "Something went wrong"));
        }
    }

    private static async Task WriteAsync(HttpContext context, int statusCode, ApiResponse<object> response)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}