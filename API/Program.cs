using API.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSingleton<IDbConnectionFactory>(_=>
    new MySqlDbConnectionFactory(builder.Configuration.GetConnectionString("MySqlConnection")!));
builder.Services.AddHostedService(_ => new DatabaseInitializer(builder.Configuration.GetConnectionString("MySqlConnection")!));
builder.Services.AddCors();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseCors(x => x
    .AllowAnyHeader()
    .AllowAnyMethod()
    .WithOrigins("http://localhost:4200", "https://localhost:4200"));

app.MapControllers();

app.Run();
