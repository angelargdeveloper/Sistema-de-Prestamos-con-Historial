using Microsoft.EntityFrameworkCore;
using AlMaximo.Infrastructure.Data;
using AlMaximo.Core.Interfaces;
using AlMaximo.Infrastructure.Repositories;
using AlMaximo.Application.Services;
using AlMaximo.Application.Mappings;
using AlMaximo.Application.Validators;
using FluentValidation;
using AlMaximo.Core.DTOs;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configurar Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/almaximo-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.WriteIndented = true;
    });

// Configurar Entity Framework
builder.Services.AddDbContext<AlMaximoDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configurar AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Configurar FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<CreatePrestamoValidator>();

// Registrar repositorios
builder.Services.AddScoped<IEmpleadoRepository, CatalogosRepository>();
builder.Services.AddScoped<IEmpleadoAutorizadorRepository, CatalogosRepository>();
builder.Services.AddScoped<ITipoPagoAbonoRepository, CatalogosRepository>();
builder.Services.AddScoped<IPrestamoRepository, PrestamoRepository>();

// Registrar servicios
builder.Services.AddScoped<ICatalogosService, CatalogosService>();
builder.Services.AddScoped<IPrestamoService, PrestamoService>();

// Configurar Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "AlMaximo Loans API",
        Version = "v1",
        Description = "API para el sistema de gestión de préstamos AlMaximo"
    });
    
    // Incluir comentarios XML si los hay
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// Configurar CORS para el frontend Angular
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200", "https://localhost:4200", 
                          "http://localhost:4201", "https://localhost:4201")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "AlMaximo Loans API v1");
    });
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseCors("AllowAngular");

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Mapear controladores API
app.MapControllers();

// Endpoint de salud
app.MapGet("/health", () => "OK");

app.Run();
