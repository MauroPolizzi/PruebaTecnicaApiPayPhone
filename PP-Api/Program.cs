using FluentValidation;
using FluentValidation.AspNetCore;
using PP_Api.Modelos;
using PP_Dominio.Entidades;
using PP_Infraestructura;
using PP_Servicios;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<DataContext>();

builder.Services.AddTransient<IBilleteraServicio, BilleteraServicio>();
builder.Services.AddTransient<IRepositorio<Billetera>, Repositorio<Billetera>>();

builder.Services.AddTransient<IMovimientoServicio, MovimientoServicio>();
builder.Services.AddTransient<IRepositorio<Movimiento>, Repositorio<Movimiento>>();

builder.Services.AddControllers();

// Agregamos middleware para FluentValidator
builder.Services.AddFluentValidationAutoValidation(); // buscará todos los validadores en el mismo ensamblado que BilleteraValidator.
builder.Services.AddValidatorsFromAssemblyContaining<BilleteraModelValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<MovimientoModelValidator>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
