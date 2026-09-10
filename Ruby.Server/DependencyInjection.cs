using Amazon.Runtime;
using Amazon.S3;
using BBL.Options;
using BBL.Providers;
using BBL.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Ruby.BBL.Services;
using Ruby.DAL;
using System.Text;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<RubyDBContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        ConfigureStorage(services, configuration);

        services.AddScoped<IFileUrlProvider, FileUrlProvider>();
        services.AddScoped<IJwtTokenProvider, JwtTokenProvider>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IRefreshTokenProvider, RefreshTokenProvider>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ITrackService, TrackService>();
        services.AddScoped<IPlaylistService, PlaylistService>();
        

        return services;
    }

    private static void ConfigureStorage(IServiceCollection services, IConfiguration configuration)
    {
        var s3Options = configuration.GetSection("S3Storage").Get<S3StorageOptions>();
        var awsCredentials = new BasicAWSCredentials(s3Options.AccessKey, s3Options.SecretKey);

        bool isHttp = s3Options.ServiceUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase);

        var s3Config = new AmazonS3Config { ServiceURL = s3Options.ServiceUrl, UseHttp = isHttp, ForcePathStyle = true };

        services.AddSingleton<IAmazonS3>(new AmazonS3Client(awsCredentials, s3Config));
        services.AddScoped<IS3StorageService, S3StorageService>();
    }

    public static IServiceCollection AddAuthConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSecretKey = configuration["JwtOptions:SecretKey"];

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options => {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true, 
                    ValidIssuer = configuration["JwtOptions:Issuer"],

                    ValidateAudience = true, 
                    ValidAudience = configuration["JwtOptions:Audience"],

                    ValidateLifetime = true, 

                    ValidateIssuerSigningKey = true, 
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey))
                };
            });

        return services;
    }
}