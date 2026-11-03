using Microsoft.EntityFrameworkCore;
using TravelClaim.Web.Data;
using TravelClaim.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TravelClaimContext>(options =>
    options.UseSqlite("Data Source=travelclaims.db"));

// Add services to the container.
builder.Services.AddScoped<AllowanceCalculator>();
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
