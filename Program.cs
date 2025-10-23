﻿using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;

var builder = WebApplication.CreateBuilder(args);

// ✅ 1️⃣ Add MySQL Database Connection
var connectionString = Environment.GetEnvironmentVariable("MYSQL_CONNECTION_STRING") ?? 
                      builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        connectionString,
        new MySqlServerVersion(new Version(8, 0, 33)) // use your MySQL version
    )
);

// ✅ 2️⃣ Add Controller Support
builder.Services.AddControllers();

// ✅ 3️⃣ Add Swagger for API documentation/testing
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ✅ Add CORS support for frontend communication
builder.Services.AddCors(options =>
{
    var allowedOrigins = Environment.GetEnvironmentVariable("CORS_ALLOWED_ORIGINS");
    var origins = new List<string> { "http://localhost:4201" }; // Always allow localhost
    
    if (!string.IsNullOrEmpty(allowedOrigins))
    {
        origins.AddRange(allowedOrigins.Split(',', StringSplitOptions.RemoveEmptyEntries));
    }
    else
    {
        // Fallback to the Netlify app if no environment variable is set
        origins.Add("https://frontendangularapp.netlify.app");
    }

    options.AddPolicy("AllowAngularDev",
        policy =>
        {
            policy.WithOrigins(origins.ToArray())
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var app = builder.Build();

// ✅ 4️⃣ Enable Swagger only in development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ✅ 5️⃣ Middleware
// Removed app.UseHttpsRedirection() to avoid HTTPS redirection issues
app.UseAuthorization();
app.UseCors("AllowAngularDev"); // Enable CORS for frontend

// ✅ 6️⃣ Map Controller Endpoints
app.MapControllers();

// ✅ 7️⃣ Run the Application
app.Run();