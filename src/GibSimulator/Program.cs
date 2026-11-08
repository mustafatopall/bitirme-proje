var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new { service = "GibSimulator", status = "up" }));

app.Run();
