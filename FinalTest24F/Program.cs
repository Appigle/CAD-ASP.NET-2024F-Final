using FinalTest24F.Data;
using FinalTest24F.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<MvcBookContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("MvcBookContext") ?? throw new InvalidOperationException("Connection string 'MvcBookContext' not found.")));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Remove the duplicate AddDefaultIdentity and combine the configuration
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
  options.SignIn.RequireConfirmedAccount = false;
  options.SignIn.RequireConfirmedEmail = false;
  options.Password.RequireDigit = false;
  options.Password.RequiredLength = 6;
  options.Password.RequireNonAlphanumeric = false;
  options.Password.RequireUppercase = false;
  options.Password.RequireLowercase = false;
})
.AddEntityFrameworkStores<MvcBookContext>();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages(); // Add this line

// Configure session with more specific options
builder.Services.AddSession(options =>
{
  options.IdleTimeout = TimeSpan.FromMinutes(30);
  options.Cookie.HttpOnly = true;
  options.Cookie.IsEssential = true;
});
builder.Services.AddScoped<IBook, Librarian>();
builder.Services.AddMemoryCache();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseMigrationsEndPoint();
}
else
{
  app.UseExceptionHandler("/Home/Error");
  app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Add session middleware BEFORE routing endpoints
app.UseSession();

// Add custom session variables middleware
app.Use(async (context, next) =>
{
  if (!context.Session.Keys.Contains("MachineName"))
  {
    context.Session.SetString("MachineName", System.Environment.MachineName);
  }
  context.Session.SetString("PageTimestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
  await next();
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();