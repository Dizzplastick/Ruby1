using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using Refit;
using Ruby.Client.Services;
using Ruby.Client.Services.Api;
using Ruby.Client.Services.Api.HttpHandlers;
using Ruby.UI.Services;
using Ruby.UI.Services.HttpHandlers;
using Ruby.UI.Services.Api;
using System.Net.Http;
using System.Windows;

namespace Ruby.Client
{
    public partial class App : Application
    {
        public IServiceProvider Services { get; }

        public App()
        {
            var services = new ServiceCollection();

            services.AddWpfBlazorWebView();

            services.AddMudServices();

            string baseApiUrl = "https://api.rubychaban.fun";

            services.AddSingleton<ITokenStorageService, TokenStorageService>();

            services.AddSingleton<IAudioEngine, DesktopBassEngine>();
            services.AddSingleton<AudioPlayerService>();

            services.AddTransient<AuthHeaderHandler>();

            services.AddAuthorizationCore(); 
            services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();

            services.AddScoped<IAudioMetadataService, DesktopAudioMetadataService>();

            services.AddRefitClient<IAuthApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseApiUrl));

            services.AddRefitClient<IStorageApi>()
        .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseApiUrl))
        .AddHttpMessageHandler<AuthHeaderHandler>();

            services.AddRefitClient<ITrackApi>()
        .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseApiUrl))
        .AddHttpMessageHandler<AuthHeaderHandler>();

            services.AddRefitClient<IPlaylistApi>()
        .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseApiUrl))
        .AddHttpMessageHandler<AuthHeaderHandler>();

            services.AddRefitClient<IUserApi>()
        .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseApiUrl))
        .AddHttpMessageHandler<AuthHeaderHandler>();

            Services = services.BuildServiceProvider();
        }
    }
}