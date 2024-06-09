using Microsoft.EntityFrameworkCore;
using FetWaveWWW.Data;
using FetWaveWWW.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using FetWaveWWW.Services;
using Ixnas.AltchaNet;
var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("FetWaveWWWContextConnection") ?? throw new InvalidOperationException("Connection string 'FetWaveWWWContextConnection' not found.");

builder.Services.AddDbContext<FetWaveWWWContext>(options => options.UseSqlServer(connectionString));

builder.Services
    .AddIdentity<FetWaveWWWUser,FetWaveWWWRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<FetWaveWWWContext>()
    .AddDefaultUI();

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();


builder.Services.AddSingleton<AltchaPageService>();
builder.Services.AddSingleton<IAltchaChallengeStore, AltchaCache>();

var app = builder.Build();

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

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
