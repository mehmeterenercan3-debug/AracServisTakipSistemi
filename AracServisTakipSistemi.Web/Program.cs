using Microsoft.EntityFrameworkCore;
using AracServisTakipSistemi.Application.Interfaces;
using AracServisTakipSistemi.Application.Options;
using AracServisTakipSistemi.Application.Services;
using AracServisTakipSistemi.Infrastructure.Data;
using AracServisTakipSistemi.Infrastructure.Repositories;
using AracServisTakipSistemi.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))));

builder.Services.Configure<RotaAyarlari>(builder.Configuration.GetSection("RotaAyarlari"));

// Repository'ler
builder.Services.AddScoped<IPersonelRepository, PersonelRepository>();
builder.Services.AddScoped<IPersonelAdresRepository, PersonelAdresRepository>();
builder.Services.AddScoped<IAracRepository, AracRepository>();
builder.Services.AddScoped<IAracAtamaRepository, AracAtamaRepository>();
builder.Services.AddScoped<ISemtReferansRepository, SemtReferansRepository>();
builder.Services.AddScoped<IVardiyaRepository, VardiyaRepository>();

// Servisler
builder.Services.AddScoped<IMesafeHesaplayici, HaversineMesafeHesaplayici>();
builder.Services.AddHttpClient<IGeocodingServisi, NominatimGeocodingServisi>();
builder.Services.AddScoped<RotaHesaplamaServisi>();
builder.Services.AddScoped<AracAtamaServisi>();
builder.Services.AddScoped<PersonelServisi>();
builder.Services.AddScoped<AracServisi>();
builder.Services.AddScoped<BakimRiskServisi>();
builder.Services.AddScoped<SemtReferansServisi>();
builder.Services.AddScoped<RotaYenidenHesaplamaOrkestraServisi>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();