using System.Text;
using DealershipStockManagement.Application.Services;
using DealershipStockManagement.Domain.Interfaces;
using DealershipStockManagement.Infrastructure.Data;
using DealershipStockManagement.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// EF (DealershipContext lives in Infrastructure)
builder.Services.AddDbContext<DatabaseContext>(opts =>
    opts.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositories (infrastructure implementations)
builder.Services.AddScoped<IStockItemRepository, StockItemRepository>();
builder.Services.AddScoped<IImageRepository, ImageRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Application services
builder.Services.AddScoped<IStockService, StockService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddSingleton<JwtService>();

// JWT auth - ensure package version matches your target framework
var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "dev_key_change_me");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.RequireHttpsMetadata = false;
        o.SaveToken = true;
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddCors(p => p.AddPolicy("AllowAll", b => b.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

// Swagger with JWT support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Dealership API", Version = "v1" });
    var securitySchema = new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using Bearer scheme.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    };
    c.AddSecurityDefinition("Bearer", securitySchema);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement { { securitySchema, new[] { "Bearer" } } });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
    await db.Database.MigrateAsync();

    // Seed admin if none exists
    if (!db.Users.Any())
    {
        // Hash the password
        var (hash, salt) = PasswordHasher.HashPassword("P@ssw0rd");

        db.Users.Add(new DealershipStockManagement.Domain.Entities.User
        {
            Username = "admin",
            PasswordHash = hash,
            PasswordSalt = salt,
            DisplayName = "Administrator",
            Role = "Admin"
        });

        await db.SaveChangesAsync();
    }
}

app.UseCors("AllowAll");
if (app.Environment.IsDevelopment()) app.UseDeveloperExceptionPage();
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();