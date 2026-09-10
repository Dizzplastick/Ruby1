using Amazon.Runtime;
using Amazon.S3;
using BBL.Options;
using BBL.Providers;
using BBL.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Ruby.DAL;
using System.Text;



var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddAuthConfiguration(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Введите 'Bearer' [пробел] и затем ваш токен.\n\nПример: 'Bearer eyJhbGciOiJIUzI1Ni...'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowWebClient", policy =>
    {
        policy.WithOrigins(
                "http://localhost:5000",
                "https://localhost:5001",
                "https://localhost:7137",
                "https://rubychaban.fun",
                "https://www.rubychaban.fun")
              .AllowAnyMethod()
              .AllowAnyHeader(); 
    });
});


builder.Services.Configure<StorageOptions>(builder.Configuration.GetSection("StorageOptions")); //ссылка на blob storage в appsettings.json
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("JwtOptions")); //настройки для генерации (для провайдера) JWT токена
builder.Services.Configure<S3StorageOptions>(builder.Configuration.GetSection("S3Storage"));// настройки для подключения к MinIO (S3 совместимое хранилище)


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.UseCors("AllowWebClient");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();


app.Run();
