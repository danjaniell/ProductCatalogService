using Asp.Versioning;
using Asp.Versioning.Builder;
using ProductCatalogService.Extensions;
using ProductCatalogService.Features.Startup;
using ProductCatalogService.Middleware;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddHealthChecks();

builder.Services.AddMemoryCacheWithFakes();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

builder.Services.AddHostedService<MemoryCacheExtensions.CacheInitializationService>();

builder.Services.AddProductCatalogServices(builder.Configuration);
builder.Services.AddFluentValidationService();

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1);
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),
        new HeaderApiVersionReader("X-Api-Version"));
})
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'V";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddEndpoints(typeof(Program).Assembly);

var app = builder.Build();
app.UseMiddleware<ExceptionMiddleware>();
app.UseSerilogRequestLogging();

app.MapHealthChecks("/health");

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapScalarApiReference(o =>
    o.WithTheme(ScalarTheme.DeepSpace)
     .WithModels(true)
     .WithSidebar(true)
     .WithLayout(ScalarLayout.Modern)
     .WithClientButton(true)
     .WithOperationSorter(OperationSorter.Method)
     .WithDotNetFlag(true)
     .OpenApiRoutePattern = "/openapi/v1.json"
    );
}

app.UseHttpsRedirection();

ApiVersionSet apiVersionSet = app.NewApiVersionSet()
    .HasApiVersion(new ApiVersion(1))
    .ReportApiVersions()
    .Build();

RouteGroupBuilder versionedGroup = app
    .MapGroup("api/v{version:apiVersion}/products")
    .WithApiVersionSet(apiVersionSet)
    .WithTags("Products");

app.MapEndpoints(versionedGroup);

app.Run();