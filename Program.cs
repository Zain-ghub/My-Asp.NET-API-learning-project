using Microsoft.EntityFrameworkCore;
using RepoApi.Data;
using RepoApi.Middleware;
using RepoApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddOpenApi();
builder.Services.AddControllers().AddJsonOptions(x =>
        x.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles); ;
builder.Services.AddDbContext<RepoContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("RepoConnection"), sqlOptions =>
    sqlOptions.EnableRetryOnFailure(
        maxRetryCount: 5,
        maxRetryDelay: TimeSpan.FromSeconds(10),
        errorNumbersToAdd: null
    )));
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddSingleton(new RabbitMQPublisher("rabbitmq"));
builder.Services.AddScoped<ICustomerService, CustomerService>();
var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<RepoContext>();
    db.Database.Migrate();
}
// adding middleware in order
app.UseMiddleware<ExeptionHandlingMiddleware>();
app.UseMiddleware<LoggerMiddleware>();
app.UseHttpsRedirection();
app.MapControllers();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.Run();


