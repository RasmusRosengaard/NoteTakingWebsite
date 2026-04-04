using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using webapi.Data;
using webapi.Repositories;

var builder = WebApplication.CreateBuilder(args);

// PostgreSQL connection
// Uses Railway DATABASE_URL in production, falls back to local database for development
var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL") 
                       ?? builder.Configuration.GetConnectionString("Default");

// JWT settings
var jwtSecret = builder.Configuration["JwtSettings:SecretKey"];
var jwtExpiryHours = int.Parse(builder.Configuration["JwtSettings:ExpiryHours"] ?? "24");

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure PostgreSQL DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString)
);

// Register repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICanvasRepository, CanvasRepository>();

// CORS setup (only needed for local dev)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
    };
});

var app = builder.Build();

// Apply EF migrations automatically on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate(); // Creates tables in PostgreSQL
}

// Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Serve Vue frontend from wwwroot
app.UseDefaultFiles();   // serves index.html by default
app.UseStaticFiles();    // serves JS/CSS/assets

// Fallback for Vue Router history mode
app.Use(async (context, next) =>
{
    await next();
    if (context.Response.StatusCode == 404 &&
        !Path.HasExtension(context.Request.Path.Value))
    {
        context.Request.Path = "/index.html";
        await next();
    }
});

app.UseHttpsRedirection();
app.UseCors("AllowFrontend"); // Optional in prod
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();