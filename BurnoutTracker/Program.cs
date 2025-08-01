using BurnoutTracker.Application.Services;
using BurnoutTracker.Application.Workers;
using BurnoutTracker.Infrastructure;
using BurnoutTracker.Utilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder => {
        builder.WithOrigins("http://localhost:5173", "http://localhost:3000", "http://52.23.183.231:3000");
        builder.AllowAnyMethod();
        builder.AllowAnyHeader();
        builder.AllowCredentials();
    });
});

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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine("JWT Auth Failed: " + context.Exception.Message);
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                Console.WriteLine("JWT Challenge triggered");
                return Task.CompletedTask;
            }
        };
    });
builder.Services.AddAuthorization();

builder.Services.AddDbContext<BTDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 4, 0)),
        options => options.EnableRetryOnFailure()
    ));

Console.WriteLine($"Using connection string: {builder.Configuration.GetConnectionString("DefaultConnection")}");


builder.Services.AddScoped<JwtHelper>();
builder.Services.AddScoped<IGitHubRepositoryPlatformService, GitHubService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<SeedService>();
builder.Services.AddScoped<IRepositoryPlatformDispatcherService, RepositoryPlatformDispatcherService>();
builder.Services.AddScoped<IBurnoutDetectionService, BurnoutDetectionService>();
builder.Services.AddScoped<IRepoService, RepoService>();
builder.Services.AddScoped<DeveloperMetricsCalculator>();
builder.Services.AddHttpClient();

builder.Services.AddHostedService<BurnoutSyncService>();



var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BTDbContext>();
    db.Database.Migrate();
    Console.WriteLine("Migration completed");
}

using (var scope = app.Services.CreateScope())
{
    var seedService = scope.ServiceProvider.GetRequiredService<SeedService>();
    await seedService.SeedAsync();
}

app.UseHttpsRedirection();
app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
