using Microsoft.EntityFrameworkCore;
using LogLayer.Data;
using LogLayer.Services;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<RequestContext>();
builder.Services.AddScoped<LogService>();
builder.Services.AddScoped<EventService>();
builder.Services.AddScoped<ExportService>();

DotNetEnv.Env.Load();

var password = Environment.GetEnvironmentVariable("DB_PASSWORD");

if (string.IsNullOrWhiteSpace(password))
    throw new InvalidOperationException("DB_PASSWORD not set");

var template = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(template))
    throw new InvalidOperationException("DefaultConnection not set");

var connectionString = template.Replace("{PasswordPlaceholder}", password);
Console.WriteLine($"Using connection string: {connectionString}");

// Add DbContext with PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

app.UseHttpsRedirection();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();


app.UseMiddleware<RequestContextMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseHttpsRedirection();

app.Run();

