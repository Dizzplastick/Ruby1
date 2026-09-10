using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services; 
using Refit;              
using Ruby.UI.Services;
using Ruby.UI.Services.Api;
using Ruby.UI.Services.HttpHandlers;
using Ruby.Web;
using Ruby.Web.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var apiUrl = "https://api.rubychaban.fun";

builder.Services.AddMudServices();

builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<ITokenStorageService, WebTokenStorageService>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();

builder.Services.AddTransient<AuthHeaderHandler>();

builder.Services.AddRefitClient<IAuthApi>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri(apiUrl));

builder.Services.AddRefitClient<IUserApi>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri(apiUrl))
    .AddHttpMessageHandler<AuthHeaderHandler>(); 

builder.Services.AddRefitClient<ITrackApi>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri(apiUrl))
    .AddHttpMessageHandler<AuthHeaderHandler>();

builder.Services.AddRefitClient<IPlaylistApi>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri(apiUrl))
    .AddHttpMessageHandler<AuthHeaderHandler>();

builder.Services.AddRefitClient<IStorageApi>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri(apiUrl))
    .AddHttpMessageHandler<AuthHeaderHandler>();

builder.Services.AddScoped<IAudioMetadataService, WebAudioMetadataService>();

builder.Services.AddSingleton<IAudioEngine, WebAudioEngine>();
builder.Services.AddSingleton<AudioPlayerService>();

await builder.Build().RunAsync();