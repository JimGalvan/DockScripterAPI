using System.Text;
using System.Text.Json.Serialization;
using Amazon.S3;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using DockScripter.Core;
using DockScripter.Core.Aws;
using DockScripter.Repositories;
using DockScripter.Repositories.Contexts;
using DockScripter.Repositories.Interfaces;
using DockScripter.Services;
using DockScripter.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Global exception filter
builder.Services.AddControllers(options => { options.Filters.Add<GlobalExceptionFilter>(); });

// logger
builder.Services.AddLogging();

// Add controllers
builder.Services.AddAWSService<IAmazonS3>();
builder.Services.AddSingleton<IS3Service, S3Service>();
builder.Services.AddScoped<IDockerClient, DockerClient>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITokenBlacklistService, TokenBlacklistService>();
builder.Services.AddScoped<IDockerContainerService, DockerContainerService>();
builder.Services.AddScoped<DockerContainerRepository>();
builder.Services.AddScoped<IScriptService, ScriptService>();
builder.Services.AddScoped<ScriptRepository>();
builder.Services.AddScoped<IExecutionService, ExecutionService>();
builder.Services.AddScoped<ExecutionResultRepository>();
builder.Services.AddScoped<GlobalExceptionFilter>();
builder.Services.AddScoped<ScriptFileRepository>();
builder.Services.AddScoped<AwsSecretsManagerClient>();


// DB context
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// API versioning
builder.Services.AddApiVersioning(options =>
{
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
});

// Handle CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        policyBuilder =>
        {
            policyBuilder // or the actual URL of your React app
                .WithOrigins("http://localhost:3000") // Specify your allowed domain here
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});

// Configure JWT authentication
// Will be replaced with a more secure method in the future

var key = Array.Empty<byte>();
if (builder.Environment.IsDevelopment())
{
    var configuration = builder.Configuration;
    var keyUnencoded = configuration["Jwt:Key"];

    if (keyUnencoded == null)
    {
        throw new Exception("JWT key not found in configuration");
    }

    key = Encoding.ASCII.GetBytes(keyUnencoded);
}

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });


// Middleware
builder.Services.AddSingleton<ITokenBlacklistService, TokenBlacklistService>();

// Set host

builder.WebHost.UseUrls("http://0.0.0.0:80");

var app = builder.Build();
var scope = app.Services.CreateScope();
var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

// log the environment
logger.LogInformation("Environment: {Environment}", app.Environment.EnvironmentName);
logger.LogInformation("ASPNETCORE_ENVIRONMENT: {ASPNETCORE_ENVIRONMENT}",
    Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"));
logger.LogInformation("Connection string: {ConnectionString}",
    app.Configuration.GetConnectionString("DefaultConnection"));

// Apply pending migrations

var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();

try
{
    logger.LogInformation("Applying database migrations...");
    dbContext.Database.EnsureCreated();
    dbContext.Database.Migrate();
    logger.LogInformation("Database migrations applied successfully.");
}
catch (Exception ex)
{
    logger.LogError(ex, "An error occurred while applying database migrations.");
}


// Configure the HTTP request pipeline.
app.UseCors("AllowAllOrigins");
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseCors();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<TokenBlacklistMiddleware>();
app.UseHttpsRedirection();

app.Run();