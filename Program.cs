using Clase_11_12.Repository;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
string connectionString = builder.Configuration.GetConnectionString("Default");
builder.Services.AddDbContext<IMDBContext>(config =>
{
    config.UseSqlServer(connectionString);
});
builder.Services.AddMvc()
                .AddJsonOptions(o => { 
                                o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve; });

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.MapControllers();
app.Run();
