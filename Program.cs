var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Add request logging middleware
app.Use(async (context, next) =>
{
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("Request: {Method} {Path} at {Time}", 
        context.Request.Method, 
        context.Request.Path, 
        DateTime.UtcNow);
    
    await next();
    
    logger.LogInformation("Response: {StatusCode} for {Method} {Path}", 
        context.Response.StatusCode,
        context.Request.Method, 
        context.Request.Path);
});

app.MapControllers();

app.Run();
