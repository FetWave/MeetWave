using MeetWave.Data;
using MeetWave.Services;
using Ixnas.AltchaNet;
using MeetWave.Services;
using MeetWave.Services.Interfaces;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Radzen;
using Stripe;
var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("MeetWaveContextConnection") ?? throw new InvalidOperationException("Connection string 'MeetWaveContextConnection' not found.");

builder.Services.AddDbContext<MeetWaveContext>(
    options => options.UseSqlServer(connectionString),
    contextLifetime: ServiceLifetime.Transient,
    optionsLifetime: ServiceLifetime.Transient);

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<MeetWaveContext>();


// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddMemoryCache();

builder.Services.AddRadzenComponents();

builder.Services.AddSingleton<AltchaPageService>();
builder.Services.AddSingleton<IAltchaChallengeStore, AltchaCache>();

builder.Services.AddSingleton<GoogleService>();
builder.Services.AddSingleton<IEmailSender, GoogleService>();

builder.Services.AddScoped<SeedDataService>();

builder.Services.AddSingleton<IPaymentsService, StripePaymentsService>();

builder.Services.AddTransient<EventsService>();
builder.Services.AddTransient<MessagesService>();
builder.Services.AddTransient<ProfilesService>();
builder.Services.AddTransient<AuthHelperService>();

builder.Services.AddAuthentication()
    .AddGoogle(googleOptions =>
    {
        googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"]!;
        googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!;
    })
    .AddDiscord(discordOptions =>
    {
        discordOptions.ClientId = builder.Configuration["Authentication:Discord:ClientId"]!;
        discordOptions.ClientSecret = builder.Configuration["Authentication:Discord:ClientSecret"]!;
    });

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

var app = builder.Build();

app.UseForwardedHeaders();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

//Data Seeding Scope
using var scope = app.Services.CreateScope();
var seedService = scope.ServiceProvider.GetService<SeedDataService>();
await seedService?.SeedEventInfra();
await seedService?.SeedProfileInfra();

switch ((app.Configuration["PaymentProcessor"]?? string.Empty).ToLower())
{
    case "stripe":
        var privateKey = app.Configuration["Authentication:Stripe:PrivateApiKey"];
        var publicKey = app.Configuration["Authentication:Stripe:PublicApiKey"];
        var apiKey = string.IsNullOrEmpty(privateKey) ? publicKey : privateKey;
        StripeConfiguration.ApiKey = apiKey;
        break;
    default:
        break;
}

app.Run();
