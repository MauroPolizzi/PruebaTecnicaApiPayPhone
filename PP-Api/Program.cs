using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using PP_Api.Mappers;
using PP_Api.Modelos;
using PP_Dominio.Entidades;
using PP_Infraestructura;
using PP_Servicios;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// Leemos configuracion de JWT desde el appSetings.json
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]!);

// Agregamos la autenticacion a traves de JWT
builder.Services.AddAuthentication(options => 
{   options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = "localhost/",
        ValidAudience = "localhost/",
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

// Add services to the container.
builder.Services.AddDbContext<DataContext>();

builder.Services.AddTransient<IBilleteraServicio, BilleteraServicio>();
builder.Services.AddTransient<IRepositorio<Billetera>, Repositorio<Billetera>>();

builder.Services.AddTransient<IMovimientoServicio, MovimientoServicio>();
builder.Services.AddTransient<IRepositorio<Movimiento>, Repositorio<Movimiento>>();

builder.Services.AddAuthorization();
builder.Services.AddControllers();

// Agregamos middleware para FluentValidator
builder.Services.AddFluentValidationAutoValidation(); // buscará todos los validadores en el mismo ensamblado que BilleteraValidator.
builder.Services.AddValidatorsFromAssemblyContaining<UsuarioModelValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<BilleteraModelValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<MovimientoModelValidator>();

// Agregamos middleware para AutoMapper
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
