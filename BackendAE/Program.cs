using BackendAE.Data;
using BackendAE.Helpers;
using BackendAE.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Roles access management
builder.Services.AddSingleton<IAuthorizationHandler, DynamicRoleHandler>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
    options.AddPolicy("Empleado", policy => policy.RequireRole("Empleado"));
    options.AddPolicy("Bodeguero", policy => policy.RequireRole("Bodeguero"));
    options.AddPolicy("Contador", policy => policy.RequireRole("Contador"));
});

// Servicio de correo
builder.Services.AddScoped<EmailService>();
// Configuraci�n de AutoMapper
builder.Services.AddAutoMapper(typeof(AutoMapperProfiles));

// Configurar el servicio de CORS para permitir or�genes espec�ficos
builder.Services.AddCors(options =>
{
    // He cambiado el nombre de la pol�tica a algo m�s descriptivo
    options.AddPolicy("AllowSpecificOrigins",
        builder =>
        {
            builder.WithOrigins("http://localhost:4200", // Para desarrollo local
                                  "https://sistemae.netlify.app") // Para producci�n
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});

// Agregar servicios al contenedor de dependencias.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configuraci�n de Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "BackendAE", Version = "v1" });
    // Configuraci�n para que Swagger pueda usar JWT
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// Configuraci�n de JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "BackendAE v1");
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

// Aseg�rate de usar el nombre correcto de la pol�tica de CORS
app.UseCors("AllowSpecificOrigins");

// Middleware de Autenticaci�n y Autorizaci�n
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();