using System.Reflection;
using AuthService.Application.Interfaces;
using AuthService.Application.UseCases;
using AuthService.Infrastructure.Persistence;
using AuthService.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Lee los comentarios XML (/// summary) para documentar los endpoints en Swagger
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

// --- Cableado de Onion Architecture (Api es la unica capa que conoce a todas) ---

// Persistencia: el DbContext es un detalle de Infrastructure.
builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AuthDB")));

// Inversion de dependencias: Application define el puerto (interfaz),
// Infrastructure la implementacion concreta. Api hace el registro.
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
builder.Services.AddScoped<IJwtService, JwtService>();

// Caso de uso (Application): orquesta Domain + puertos.
builder.Services.AddScoped<LoginUseCase>();

// CORS: permite que el frontend de React (Vite en :5173) llame a este servicio.
// Sin esto, el navegador bloquea las peticiones entre origenes distintos.
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirFrontend", policy =>
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Activa la politica CORS antes de la autenticacion/autorizacion.
app.UseCors("PermitirFrontend");

app.UseAuthorization();

app.MapControllers();

app.Run();