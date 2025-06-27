using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SolucionDA.DatabaseAccess;
using SolucionDA.UnitOfWork;


var builder = WebApplication.CreateBuilder(args);

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Controllers
builder.Services.AddControllers();

// 🔌 Inyección del factory que resuelve múltiples motores
builder.Services.AddSingleton<IDbConnectionFactory, MultiDbConnectionFactory>();

// 💼 Inyección de UnitOfWork
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

var app = builder.Build();

// Swagger UI en desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// HTTPS + Controllers
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();