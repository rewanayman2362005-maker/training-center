using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TrainingCenter.Data;
using TrainingCenter.Models;
using TrainingCenter.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Default") ?? "Data Source=trainingcenter.db"));

builder.Services.AddScoped<PasswordHasher>();
builder.Services.AddScoped<TokenService>();

var jwtKey = builder.Configuration["Jwt:Key"] ?? "REPLACE_THIS_WITH_A_LONG_RANDOM_SECRET_AT_LEAST_32_CHARACTERS";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "TrainingCenterApi";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

var app = builder.Build();

// إنشاء قاعدة البيانات وزرع حساب الأدمن أول مرة فقط
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();

    if (!db.AdminUsers.Any())
    {
        var hasher = scope.ServiceProvider.GetRequiredService<PasswordHasher>();
        var seedUsername = app.Configuration["Admin:Username"] ?? "admin";
        var seedPassword = app.Configuration["Admin:Password"] ?? "ChangeMe123!";

        db.AdminUsers.Add(new AdminUser
        {
            Username = seedUsername,
            PasswordHash = hasher.Hash(seedPassword)
        });
        db.SaveChanges();
    }
}

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// أي رابط غير API يرجع لصفحة الموقع (Single Page App)
app.MapFallbackToFile("index.html");

app.Run();
