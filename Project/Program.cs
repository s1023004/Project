using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession(
    (options) =>
    {
        //最大間隔時間為30分鐘
        options.IdleTimeout = TimeSpan.FromMinutes(30);
    }
);

ConfigurationManager manager = builder.Configuration;
String connectionString = manager.GetConnectionString("nor");
Console.WriteLine(connectionString);

builder.Services.AddDbContext<Project.Models.NorthwindDB>(
        (options) =>
        {
            options.UseSqlServer(connectionString);
        }
        );
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
//使用Session Middleware
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
