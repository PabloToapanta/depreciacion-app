using System.Reflection;
using System.Text;
using AssetsService.Application.Interfaces;
using AssetsService.Application.UseCases;
using AssetsService.Domain.Services;
using AssetsService.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

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
builder.Services.AddDbContext<AssetsDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AssetsDB")));

// Inversion de dependencias: Application define el puerto (interfaz),
// Infrastructure la implementacion concreta. Api hace el registro.
builder.Services.AddScoped<IActivoRepository, ActivoRepository>();
builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();

// Regla de negocio pura (Domain).
builder.Services.AddScoped<DepreciacionService>();

// Casos de uso (Application): orquestan Domain + puertos.
builder.Services.AddScoped<CrearActivoUseCase>();
builder.Services.AddScoped<ListarActivosUseCase>();
builder.Services.AddScoped<ObtenerActivoUseCase>();
builder.Services.AddScoped<CalcularDepreciacionUseCase>();
builder.Services.AddScoped<ListarCategoriasUseCase>();

// CORS: permite que el frontend de React (Vite en :5173) llame a este servicio.
// Sin esto, el navegador bloquea las peticiones entre origenes distintos.
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirFrontend", policy =>
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

// Autenticacion con JWT: valida los tokens que emite AuthService (misma clave,
// mismo issuer y misma audience). Sin token valido -> 401 automatico.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
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

// UseAuthentication debe ir ANTES de UseAuthorization: primero se identifica
// quien llama (JWT), luego se decide si puede pasar ([Authorize]).
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();