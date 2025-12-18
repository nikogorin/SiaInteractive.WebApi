using Serilog;
using SiaInteractive.Application;
using SiaInteractive.Infraestructure;
using SiaInteractive.WebApi.Modules;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddIntraestructureServices();
builder.Services.AddApplicationServices();
builder.Services.AddLogServices(builder.Configuration);
builder.Services.AddSwagger();

builder.Host.UseSerilog();

try
{
    Log.Information("Starting SiaInteractive WebApi...");

    var app = builder.Build();

    app.UseMiddleware<ExceptionHandlingMiddleware>();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "SiaInteractive WebApi v1");
            options.RoutePrefix = "swagger";
            options.DisplayRequestDuration();
            options.ShowExtensions();
        });
    }

    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "SiaInteractive WebApi terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

public partial class Program { }