using BackendAE.Data;
using BackendAE.Helpers;
using BackendAE.Services; // Asegúrate de que este using sea correcto para tu proyecto
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models; // Necesario para la configuración de Swagger
using System.ComponentModel;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configuración de formato de fecha global
builder.Services.Configure<JsonOptions>(options =>
{
    options.JsonSerializerOptions.Converters.Add(new DateTimeConverter("dd-MM-yyyy"));
});

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new DateTimeJsonConverter());
        options.JsonSerializerOptions.Converters.Add(new NullableDateTimeJsonConverter());
    });

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
// Configuración de AutoMapper
builder.Services.AddAutoMapper(typeof(AutoMapperProfiles));
// Configuración de la aplicación
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
// Configurar el servicio de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp",
        builder =>
        {
            builder.WithOrigins("http://localhost:4200") // ?? Reemplaza con la URL de tu frontend de Angular
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});

// Agregar servicios al contenedor de dependencias.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// >>>>>>>>>>> Aquí se agrega la configuración de Swagger <<<<<<<<<<<<
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    //c.SwaggerDoc("v1", new OpenApiInfo { Title = "BackendAE", Version = "v1" });
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "BackendAE", Version = "v1" });
    // Configuración para que Swagger pueda usar JWT
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
// >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

// >>>>>>>>>>> Configuración de JWT <<<<<<<<<<<<
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
    // >>>>>>>>>>> Aquí se habilita el middleware de Swagger <<<<<<<<<<<<
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "BackendAE v1");
    });
    // >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseCors("AllowAngularApp");

// >>>>>>>>>>> Middleware de Autenticación y Autorización <<<<<<<<<<<<
app.UseAuthentication();
app.UseAuthorization();
// >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

app.UseAuthorization();

app.MapControllers();

app.Run();