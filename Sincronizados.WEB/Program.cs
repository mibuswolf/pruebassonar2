using Blazored.Modal;
using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Radzen;
using Sincronizados.Shared.Models;
using Sincronizados.WEB;
using Sincronizados.WEB.Auth;
using Sincronizados.WEB.Helpers;
using Sincronizados.WEB.HttpHanddlers;


var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

//builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddSingleton(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7050/") }); //apintamos al backend
//builder.Services.AddSingleton(sp => new HttpClient { BaseAddress = new Uri("https://web-apl-ob:5557/SincronizadosWebBackPD/") }); //apintamos al backend
//builder.Services.AddSingleton(sp => new HttpClient { BaseAddress = new Uri("https://crc.obgroup.systems:5558/") }); //apintamos al backend
//builder.Services.AddSingleton(sp => new HttpClient { BaseAddress = new Uri("https://sincronizados.obgroup.systems:5558/") }); 

builder.Services.AddLocalization(); //Servicios de localizacion

builder.Services.AddScoped<IRepository, Repository>();
builder.Services.AddScoped<ISuscriptionServices, SuscriptionServices>();

//builder.Services.AddOptions();
builder.Services.AddAuthorizationCore();
Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1NHaF1cWWhIfEx1RHxQdld5ZFRHallYTnNWUj0eQnxTdEZiWH1ccH1UQ2RbUEZ0XQ==");

//builder.Services.AddSyncfusionBlazor();
builder.Services.AddSweetAlert2();
builder.Services.AddBlazoredModal();
builder.Services.AddRadzenComponents();
builder.Services.AddScoped<DialogService>();
builder.Services.AddScoped<NotificationService>();

builder.Services.AddScoped<AuthenticationProviderJWT>();
builder.Services.AddScoped<AuthenticationStateProvider, AuthenticationProviderJWT>(x => x.GetRequiredService<AuthenticationProviderJWT>());
builder.Services.AddScoped<ILoginService, AuthenticationProviderJWT>(x => x.GetRequiredService<AuthenticationProviderJWT>());
builder.Services.AddSingleton<IPublicUserDto, PublicUserDto>();

await builder.Build().RunAsync();
