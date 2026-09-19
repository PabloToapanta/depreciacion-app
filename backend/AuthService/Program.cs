using System.Reflection;
using AuthService.Data;
using AuthService.Services;
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

// Registra el DbContext con la connection string de AuthDB (usuario auth_user).
builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AuthDB")));

// Registra el servicio de generacion de JWT.
builder.Services.AddScoped<JwtService>();

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
