using Microsoft.AspNetCore.Mvc;
using TomApi.Data;
using TomApi.Interfaces;
using TomApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add config support and logging
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
builder.Logging.ClearProviders().AddConsole();

//Add the data repositories to the controllers
builder.Services.AddTransient<IDatabaseObject<Room_2D>, RoomData>();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();

