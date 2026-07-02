using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using backend;
using backend.Data;

// Load .env from project root (one level up from backend/)
Env.TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

// Read connection from KEYFILE (.mglc) or DATABASE_URL
var keyFilePath = builder.Configuration["KEYFILE"] ?? Environment.GetEnvironmentVariable("KEYFILE");
if (!string.IsNullOrWhiteSpace(keyFilePath))
    KeyFileReader.Read(keyFilePath);

var connectionString = KeyFileReader.ConnectionString.Length > 0
    ? KeyFileReader.ConnectionString
    : builder.Configuration["DATABASE_URL"]
      ?? Environment.GetEnvironmentVariable("DATABASE_URL")
      ?? throw new InvalidOperationException("Either KEYFILE or DATABASE_URL must be set.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        var nuxtOrigin = builder.Configuration["NUXT_ORIGIN"]
            ?? Environment.GetEnvironmentVariable("NUXT_ORIGIN")
            ?? "http://localhost:3000";

        // Support comma-separated list: "http://localhost:3000,http://localhost:3001"
        var origins = nuxtOrigin.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        policy.WithOrigins(origins).AllowAnyHeader().AllowAnyMethod();
    });
});

var app = builder.Build();

// OpenAPI + Scalar: always enabled (API endpoints are auth-protected)
app.MapOpenApi();
app.MapScalarApiReference(options =>
{
    options.Title  = "HR System API";
    options.Theme  = ScalarTheme.BluePlanet;
    options.DefaultHttpClient = new(ScalarTarget.CSharp, ScalarClient.HttpClient);
    options.Authentication = new ScalarAuthenticationOptions
    {
        PreferredSecurityScheme = "MangoAuth",
    };
});

app.UseCors();
if (app.Environment.IsDevelopment())
    app.UseHttpsRedirection();
app.MapControllers();
app.Run();
