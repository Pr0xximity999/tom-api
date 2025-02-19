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

//Connection attempt logging
app.Use(async (context, next) =>
{
    var req = context.Request;
    req.EnableBuffering();
    Console.WriteLine("-----------------");
    Console.WriteLine(DateTime.Now);
    Console.WriteLine(req.Headers["X-Forwarded-For"]); // thanks nginx
    Console.WriteLine($"Method:{req.Method}\nEndpoint:{req.Path} | {context.GetEndpoint()?.DisplayName}");
    Console.WriteLine($"Authorization: {req.Headers.Authorization}");
    Console.WriteLine($"Params: {string.Join("\n\t", req.Query.Select(q => $"{q.Key}={q.Value}"))}");
    Console.WriteLine($"Body: {await new StreamReader(req.Body).ReadToEndAsync()}");
    req.Body.Position = 0; //brother what the fuck
    Console.WriteLine("-----------------");
    await next.Invoke();
});

app.UseHttpsRedirection();

app.MapGet("/", () => Results.Content(
@"<html>
<img style='width=100%;height=100%;' src='https://i.imgur.com/QrgxarN.jpeg'>
</html>
","text/html"));
app.MapControllers();
app.Run();

