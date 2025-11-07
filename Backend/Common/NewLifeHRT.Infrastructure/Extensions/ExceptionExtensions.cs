using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NewLifeHRT.Infrastructure.Models.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NewLifeHRT.Infrastructure.Extensions
{
    public static class ExceptionExtensions
    {
        public static GlobalAPIException ToGlobalApiException(this Exception ex)
        {
            return ex switch
            {
                // 400 - Client input error
                ArgumentException => new GlobalAPIException(
                    ex.Message,
                    StatusCodes.Status400BadRequest,
                    ex
                ),
                FormatException => new GlobalAPIException(
                    "Invalid data format.",
                    StatusCodes.Status400BadRequest,
                    ex
                ),
                InvalidOperationException => new GlobalAPIException(
                    ex.Message,
                    StatusCodes.Status400BadRequest,
                    ex
                ),

                // 401 - Authentication
                SecurityTokenException => new GlobalAPIException(
                    ex.Message,
                    StatusCodes.Status401Unauthorized,
                    ex
                ),
                UnauthorizedAccessException => new GlobalAPIException(
                    ex.Message,
                    StatusCodes.Status401Unauthorized,
                    ex
                ),

                // 403 - Forbidden
                AccessViolationException => new GlobalAPIException(
                    "Access violation error.",
                    StatusCodes.Status403Forbidden,
                    ex
                ),

                // 404 - Not Found
                KeyNotFoundException => new GlobalAPIException(
                    ex.Message,
                    StatusCodes.Status404NotFound,
                    ex
                ),

                // 409 - Conflict (DB constraint / unique key)
                DbUpdateException => new GlobalAPIException(
                    ex.InnerException?.Message ?? "Database conflict occurred.",
                    StatusCodes.Status409Conflict,
                    ex
                ),

                // 422 - Validation error
                ValidationException valEx => new GlobalAPIException(
                    valEx.Message,
                    StatusCodes.Status422UnprocessableEntity,
                    valEx
                ),

                // Default → 500 Internal Server Error
                _ => new GlobalAPIException(
                    string.IsNullOrWhiteSpace(ex.Message)
                        ? "An unexpected error occurred."
                        : ex.Message,
                    StatusCodes.Status500InternalServerError,
                    ex
                )
            };
        }
    }
}
