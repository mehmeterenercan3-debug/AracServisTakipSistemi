using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using AracServisTakipSistemi.BLL.Interfaces;
using AracServisTakipSistemi.BLL.Services;
using AracServisTakipSistemi.DAL.Data;
using AracServisTakipSistemi.DAL.Repositories;
using AracServisTakipSistemi.DAL.Services;
using AracServisTakipSistemi.Entities.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))));

// Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole<int>>(options =>
    {
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 6;
        options.Password.RequireUppercase = false;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Hesap/Giris";
    options.AccessDeniedPath = "/Hesap/Erisimyok";
});
// Repository'ler
builder.Services.AddScoped<IPersonelRepository, PersonelRepository>();
builder.Services.AddScoped<IPersonelAdresRepository, PersonelAdresRepository>();
builder.Services.AddScoped<IAracRepository, AracRepository>();
builder.Services.AddScoped<ISemtReferansRepository, SemtReferansRepository>();
builder.Services.AddScoped<IVardiyaRepository, VardiyaRepository>();
builder.Services.AddScoped<IBolgeRepository, BolgeRepository>();
builder.Services.AddScoped<IRotaRepository, RotaRepository>();
builder.Services.AddScoped<IAracArizaKaydiRepository, AracArizaKaydiRepository>();
builder.Services.AddScoped<IBakimKaydiRepository, BakimKaydiRepository>();

// Servisler
builder.Services.AddScoped<IMesafeHesaplayici, HaversineMesafeHesaplayici>();
builder.Services.AddHttpClient<IGeocodingServisi, NominatimGeocodingServisi>();
builder.Services.AddScoped<RotaHesaplamaServisi>();
builder.Services.AddScoped<PersonelServisi>();
builder.Services.AddScoped<AracServisi>();
builder.Services.AddScoped<BakimRiskServisi>();
builder.Services.AddScoped<SemtReferansServisi>();
builder.Services.AddScoped<BolgeServisi>();
builder.Services.AddScoped<RotaYenidenHesaplamaOrkestraServisi>();
builder.Services.AddScoped<VardiyaServisi>();
builder.Services.AddScoped<RotaServisi>();
builder.Services.AddScoped<AracArizaKaydiServisi>();
builder.Services.AddScoped<BakimKaydiServisi>();

// Sunucu her zaman nokta = ondalık ayıracı kullansın (tr-TR virgül bekleyip veri bozulmasın diye)
var desteklenenKulturler = new[] { new CultureInfo("en-US") };
builder.Services.Configure<Microsoft.AspNetCore.Builder.RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("en-US");
    options.SupportedCultures = desteklenenKulturler;
    options.SupportedUICultures = desteklenenKulturler;
});

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

app.UseRequestLocalization();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Rolleri ilk çalıştırmada otomatik oluştur
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
    foreach (var rol in new[] { "Admin", "Sofor", "Personel" })
    {
        if (!await roleManager.RoleExistsAsync(rol))
            await roleManager.CreateAsync(new IdentityRole<int>(rol));
    }
}

// İlk admin kullanıcısını oluştur (yoksa)
using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var adminKullaniciAdi = "admin";

    if (await userManager.FindByNameAsync(adminKullaniciAdi) == null)
    {
        var admin = new ApplicationUser { UserName = adminKullaniciAdi, Email = "admin@aracservis.local" };
        var sonuc = await userManager.CreateAsync(admin, "Admin123!");
        if (sonuc.Succeeded)
            await userManager.AddToRoleAsync(admin, "Admin");
    }
}

app.Run();