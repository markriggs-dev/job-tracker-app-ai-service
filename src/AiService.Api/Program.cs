using Anthropic.SDK;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Minio;
using AiService.Api.Middleware;
using AiService.Core.Interfaces;
using AiService.Core.Services;
using AiService.Core.Clients;
using AiService.Infrastructure.Data;
using AiService.Infrastructure.Repositories;
using AiService.Infrastructure.Storage;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Auth0
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = $"https://{builder.Configuration["Auth0:Domain"]}/";
        options.Audience = builder.Configuration["Auth0:Audience"];
    });

builder.Services.AddAuthorization();

// EF Core
builder.Services.AddDbContext<AiServiceDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Storage settings + MinIO
builder.Services.Configure<StorageSettings>(builder.Configuration.GetSection("Storage"));
builder.Services.AddSingleton<IMinioClient>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<StorageSettings>>().Value;
    return new MinioClient()
        .WithEndpoint(settings.Endpoint)
        .WithCredentials(settings.AccessKey, settings.SecretKey)
        .WithSSL(settings.UseSSL)
        .Build();
});

// Anthropic Claude client
builder.Services.AddSingleton(new AnthropicClient(builder.Configuration["Anthropic:ApiKey"]!));

// Internal service HTTP clients
builder.Services.AddHttpClient<JobServiceClient>(c =>
    c.BaseAddress = new Uri(builder.Configuration["Services:JobService"]!));

builder.Services.AddHttpClient<ExperienceServiceClient>(c =>
    c.BaseAddress = new Uri(builder.Configuration["Services:ExperienceService"]!));

// Repositories and services
builder.Services.AddScoped<IStorageService, MinioStorageService>();
builder.Services.AddScoped<IAiProfileRepository, AiProfileRepository>();
builder.Services.AddScoped<IGeneratedResumeRepository, GeneratedResumeRepository>();
builder.Services.AddScoped<AiProfileService>();
builder.Services.AddScoped<ResumeGenerationService>();

// CORS
builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy =>
        policy.WithOrigins(builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? ["http://localhost:5173"])
              .AllowAnyHeader()
              .AllowAnyMethod()));

var app = builder.Build();

// Auto-migrate
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AiServiceDbContext>();
    db.Database.Migrate();
}

// Ensure MinIO bucket exists
using (var scope = app.Services.CreateScope())
{
    var storage = scope.ServiceProvider.GetRequiredService<IStorageService>();
    await storage.EnsureBucketExistsAsync();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseRouting();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
