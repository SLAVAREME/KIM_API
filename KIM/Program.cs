using KIM.Api.Middlewares;
using KIM.BL.Extensions;
using KIM.BL.Shared.Responses;
using KIM.DAL.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendDev", policy =>
    {
        policy
            .SetIsOriginAllowed(origin =>
            {
                if (!Uri.TryCreate(origin, UriKind.Absolute, out var uri))
                {
                    return false;
                }

                return uri.Host is "localhost" or "127.0.0.1";
            })
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
builder.Services.AddKimDal(builder.Configuration);
builder.Services.AddKimBl();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "KIM API",
        Version = "v1"
    });
});
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(x => x.Value?.Errors.Count > 0)
            .SelectMany(x => x.Value!.Errors.Select(error => new ValidationErrorItem
            {
                Field = x.Key,
                Message = string.IsNullOrWhiteSpace(error.ErrorMessage) ? "Validation error" : error.ErrorMessage
            }))
            .ToList();

        var response = ApiResponse<object>.Failure(AppCode.ValidationError, "Validation failed", errors);
        return new BadRequestObjectResult(response);
    };
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "KIM API v1");
        options.RoutePrefix = "swagger";
    });
}

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseHttpsRedirection();
app.UseCors("FrontendDev");

app.UseAuthorization();

app.MapControllers();

app.Run();
