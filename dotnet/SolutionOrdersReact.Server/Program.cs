using Microsoft.EntityFrameworkCore;
using SolutionOrdersReact.Server.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// 1. Controllers
builder.Services.AddControllers();

// 2. Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() 
    { 
        Title = "SolutionOrdersReact API", 
        Version = "v1",
        Description = "API dla systemu zamówień z CQRS"
    });
});

// 3. Entity Framework Core - SQL Server
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Server=localhost,1433;Database=TestReactDb;User=sa;Password=YourStrong@Password123;TrustServerCertificate=True;MultipleActiveResultSets=true";

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// 4. MediatR - automatyczne skanowanie handlerów
builder.Services.AddMediatR(cfg => 
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

// 5. CORS - dla React Native
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.

// Swagger UI (w development i production dla łatwiejszego testowania)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "SolutionOrdersReact API v1");
    c.RoutePrefix = "swagger";
});

app.UseHttpsRedirection();

// CORS
app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

// Auto-migracja bazy danych przy starcie (tylko DEV!)
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        try
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            
            // Migracja bazy danych
            dbContext.Database.Migrate();
            logger.LogInformation("✅ Baza danych zmigrowana pomyślnie!");
        }
        catch (Exception ex)
        {
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "⚠️ Błąd podczas migracji bazy danych");
            Console.WriteLine($"⚠️ Błąd połączenia z bazą: {ex.Message}");
            Console.WriteLine("ℹ️ Uruchom: docker-compose -f docker-compose-db.yml up -d");
        }
    }
}

app.Run();