using System.Reflection;
using AssetsService.Data;
using AssetsService.Services;
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

// Registra el DbContext con la connection string de AssetsDB (usuario assets_user).
builder.Services.AddDbContext<AssetsDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AssetsDB")));

// Registra el servicio de calculo de depreciacion.
builder.Services.AddScoped<DepreciacionService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
