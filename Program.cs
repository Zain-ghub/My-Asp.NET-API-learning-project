using Microsoft.EntityFrameworkCore;
using RepoApi.Data;
using RepoApi.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddOpenApi();
builder.Services.AddControllers().AddJsonOptions(x =>
        x.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles); ;
builder.Services.AddDbContext<RepoContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("RepoConnection")));

var app = builder.Build();
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


