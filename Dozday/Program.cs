//using Dozday.Client.Pages;

using Dozday.Components;
using Dozday.Components.Layout;
using Dozday.Core.Interfaces;
using Dozday.Core.Models;
using Dozday.Data;
using Dozday.Data.Repositories;
using Dozday.Services.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using System.Text;
using _Imports = Dozday.Client._Imports;

Console.OutputEncoding = Encoding.UTF8;

var builder = WebApplication.CreateBuilder(args);

// Configure JwtSettings
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings"));
builder.Services.Configure<GoogleAuthOptions>(
    builder.Configuration.GetSection("GoogleAuth"));

// Register Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IQualityAssuranceRepository, QualityAssuranceRepository>();
builder.Services.AddScoped<IUserSubscriptionRepository, UserSubscriptionRepository>();

// Register Services
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IQualityAssuranceService, QualityAssuranceService>();
builder.Services.AddScoped<GlobalExceptionModalState>();

builder.Services.AddAutoMapper(cfg => { cfg.AddMaps(AppDomain.CurrentDomain.GetAssemblies()); });

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped(sp =>
{
    var navigationManager = sp.GetRequiredService<NavigationManager>();
    return new HttpClient { BaseAddress = new Uri(navigationManager.BaseUri) };
});

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddDbContext<DozdayDbContext>(options =>
{
    options.UseNpgsql(
        "Host=localhost;Port=5432;Database=Dozday;Username=postgres;Password=Pavlik1107"
    );
});

var app = builder.Build();

//using (var scope = app.Services.CreateScope())
//{
//    var db = scope.ServiceProvider.GetRequiredService<DozdayDbContext>();

//    try
//    {
//        var canConnect = await db.Database.CanConnectAsync();

//        if (canConnect)
//        {
//            Console.WriteLine("PostgreSQL: підключення успішне");
//            var simpleQueryResult = await db.Database.ExecuteSqlRawAsync("SELECT * FROM ");
//            Console.WriteLine(simpleQueryResult);
//        }
//        else
//        {
//            Console.WriteLine("PostgreSQL: НЕ вдалося підключитися");
//        }
//    }
//    catch (Exception ex)
//    {
//        Console.WriteLine("PostgreSQL: помилка підключення");
//        Console.WriteLine(ex.Message);
//    }
//}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Error", true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.Use(async (context, next) =>
{
    context.Response.Headers["Cross-Origin-Opener-Policy"] = "same-origin-allow-popups";
    await next();
});

app.UseAntiforgery();

app.MapControllers();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(_Imports).Assembly);

app.Run();