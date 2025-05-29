using Microsoft.EntityFrameworkCore;
using kutuphane;
using kutuphane.Models;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();




builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=kutuphanem.db"));


builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Home/Login";
        options.LogoutPath = "/Home/Logout";
        options.AccessDeniedPath = "/Home/AccessDenied";
        options.Cookie.HttpOnly = true; 
        options.ExpireTimeSpan = TimeSpan.FromDays(30); 
        options.SlidingExpiration = true; 
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always; 
        options.Cookie.SameSite = SameSiteMode.Strict; 
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}



app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    if (!context.Kategoriler.Any())
    {
        var roman = new CategoryModel { Name = "Roman", ParentCategoryId = null };
        var siir = new CategoryModel { Name = "Şiir", ParentCategoryId = null };
        var manga = new CategoryModel { Name = "Manga", ParentCategoryId = null };
        context.Kategoriler.AddRange(roman, siir, manga);
        context.SaveChanges();

        var altKategoriler = new List<CategoryModel>
        {
            new CategoryModel { Name = "Tarihî Roman", ParentCategoryId = roman.Id },
            new CategoryModel { Name = "Polisiye Roman", ParentCategoryId = roman.Id },
            new CategoryModel { Name = "Modern Şiir", ParentCategoryId = siir.Id },
            new CategoryModel { Name = "Klasik Şiir", ParentCategoryId = siir.Id },
            new CategoryModel { Name = "Shonen", ParentCategoryId = manga.Id },
            new CategoryModel { Name = "Shoujo", ParentCategoryId = manga.Id }
        };
        context.Kategoriler.AddRange(altKategoriler);
        context.SaveChanges();
    }
}

app.Run();