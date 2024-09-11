using dotenv.net;
using Microsoft.EntityFrameworkCore;
using MilkStore.API;
using MilkStore.Contract.Repositories;
using MilkStore.Contract.Services.Interface;
using MilkStore.Repositories.Context;
using MilkStore.Repositories.UOW;
using MilkStore.Services.Service;

DotEnv.Load();
var builder = WebApplication.CreateBuilder(args);

// config appsettings by env
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("MilkStore.Repositories")));



builder.Services.AddControllers();
builder.Services.AddScoped<IOrderService, OrderService>();
//builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddConfig(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
