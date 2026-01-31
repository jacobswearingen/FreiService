using FreiService.Web.Components;
using FreiService.Data;
using FreiService.Data.Repositories;
using FreiService.Data.Services;
using FreiService.Computus;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Configure SQLite database
var dbPath = Path.Combine(builder.Environment.ContentRootPath, "holydays.db");
builder.Services.AddDbContext<HolyDaysContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));

// Register services
builder.Services.AddScoped<IComputusService, ComputusService>();
builder.Services.AddScoped<IHolyDaysRepository, HolyDaysRepository>();
builder.Services.AddScoped<IHolyDayDefinitionsRepository, HolyDayDefinitionsRepository>();
builder.Services.AddScoped<IHolyDaysService, HolyDaysService>();

var app = builder.Build();

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<HolyDaysContext>();
    
    // In development, recreate the database to handle schema changes
    if (app.Environment.IsDevelopment())
    {
        dbContext.Database.EnsureDeleted();
    }
    
    dbContext.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
