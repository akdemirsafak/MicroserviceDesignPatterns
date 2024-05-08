using System.Reflection;
using EventSourcing.API.Commands;
using EventSourcing.API.DbContexts;
using EventSourcing.API.EventStores;
using MediatR;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddMediatR(Assembly.GetExecutingAssembly());
//builder.Services.AddMediatR(typeof(CreateProductCommand).Assembly);
//builder.Services.AddMediatR(typeof(Program).Assembly);

builder.Services.AddEventStore(builder.Configuration); //panel : http://localhost:2113/
builder.Services.AddSingleton<ProductStream>();


builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
