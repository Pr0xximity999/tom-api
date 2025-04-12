using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TomApi.Data;
using TomApi.Interfaces;
using TomApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add config support and logging
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
builder.Logging.ClearProviders().AddConsole();

//Adding http content accessor to access user claims
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<IAuthenticationService, AuthenticationService>();

//Add the data repositories to the controllers
builder.Services.AddTransient<IRoomData, RoomData>();
builder.Services.AddTransient<IObjectData, ObjectData>();
builder.Services.AddTransient<IDataService, MySqlDataService>();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddAuthorization();
builder.Services.AddIdentityApiEndpoints<IdentityUser>(options =>
    {
        options.User.RequireUniqueEmail = true;
        options.Password.RequiredLength = 10;
    })
    .AddRoles<IdentityRole>()
    .AddDapperStores(options =>
    {
        options.ConnectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING_SQL") ?? builder.Configuration.GetConnectionString("azure");
    });
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
    Console.WriteLine(req.HttpContext.Connection.RemoteIpAddress);
    Console.WriteLine($"Method:{req.Method}\nEndpoint:{req.Path} | {context.GetEndpoint()?.DisplayName}");
    Console.WriteLine($"Authorization: {req.Headers.Authorization}");
    Console.WriteLine($"Params: {string.Join("\n\t", req.Query.Select(q => $"{q.Key}={q.Value}"))}");
    Console.WriteLine($"Body: {await new StreamReader(req.Body).ReadToEndAsync()}");
    req.Body.Position = 0;
    Console.WriteLine("-----------------");
    await next.Invoke();
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapGroup("/account").MapIdentityApi<IdentityUser>();

//Embed image and landing page
app.MapGet("/", () => Results.Content(
@"<html>
<head>
    <meta property=""og:type"" content=""avans yourself!!!!"">
    <meta property=""og:title"" content=""Title"">
    <meta property=""og:description"" content=""Google, show me this guys balls"">
    <meta property=""og:image"" content=""https://i.imgur.com/QrgxarN.jpeg"""">
    <meta property=""og:image:width"" content=""400"">
    <meta property=""og:image:height"" content=""400"">
</head>
<img style='width=100%;height=100%;' src='https://i.imgur.com/QrgxarN.jpeg'>
</html>
","text/html"));

app.MapPost("/account/logout",
    async (SignInManager<IdentityUser> signinManager, [FromBody] object empty) =>
    {
        if (empty != null)
        {
            await signinManager.SignOutAsync();
            return Results.Ok();
        }
        return Results.Unauthorized();
    }).RequireAuthorization();
app.MapGet("/account/checkAccessToken", [Authorize] () => Results.Content("{\"authorized\": true}", "application/json"));
app.MapGet("/account/id", [Authorize] (IAuthenticationService auth) => auth.GetCurrentUserId());
app.MapControllers();
app.Run();

