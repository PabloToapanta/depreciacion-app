using System.Reflection;
using System.Text;
using AssetsService.Data;
using AssetsService.Services;
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

// Registra el DbContext con la connection string de AssetsDB (usuario assets_user).
builder.Services.AddDbContext<AssetsDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AssetsDB")));

// Registra el servicio de calculo de depreciacion.
builder.Services.AddScoped<DepreciacionService>();

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
