using API.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSingleton<IDbConnectionFactory>(_=>
    new MySqlDbConnectionFactory(builder.Configuration.GetConnectionString("MySqlConnection")!));  //["DbConnectionString]"]!));
//builder.Services.AddSingleton(_ => new DatabaseInitializer(builder.Configuration.GetConnectionString("MySqlConnection")!));
builder.Services.AddHostedService(_ => new DatabaseInitializer(builder.Configuration.GetConnectionString("MySqlConnection")!));
var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapControllers();

app.Run();
