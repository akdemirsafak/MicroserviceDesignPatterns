using ServiceA.API;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddHttpClient<ProductService>(opt =>
{
    opt.BaseAddress = new Uri("https://localhost:5001/api/products/");
})
// .AddTransientHttpErrorPolicy(_ => ResiliencyStrategies.GetRetryPolicy());
//.AddPolicyHandler(ResiliencyStrategies.GetCircuitBreakerPolicy());
.AddPolicyHandler(ResiliencyStrategies.GetAdvancedCircuitBreakerPolicy());




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
