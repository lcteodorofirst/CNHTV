using System.Text.Json.Serialization;
using Cnhtv.Application.Common;
using Cnhtv.Application.DependencyInjection;
using Cnhtv.Infrastructure.DependencyInjection;
using Microsoft.AspNetCore.OData;
using Microsoft.OData.ModelBuilder;
using NLog;
using NLog.Web;
using Cnhtv.Api.Services;

LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

string[] allowedOrigins = [];

var builder = WebApplication.CreateBuilder(args);
{
    builder.Logging.ClearProviders();
    builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
    builder.Host.UseNLog();

    var systemConfig = builder
        .Configuration.GetSection(nameof(AppConfiguration))
        .Get<AppConfiguration>();

    if (systemConfig is null)
    {
        throw new InvalidOperationException(
            $"A configuração do '{nameof(AppConfiguration)}' está faltando e é necessária para iniciar a aplicação. Verifique o appsettings."
        );
    }

    builder.Services.Configure<AppConfiguration>(
        builder.Configuration.GetSection(nameof(AppConfiguration))
    );

    builder.Services.AddRazorPages();
    builder.Services.AddHealthChecks();
    builder.Services.AddAuthorization();
    builder.Services.AddHostedService<DevicePresenceWorker>();

    allowedOrigins = (builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? [])
        .Where(origin => !string.IsNullOrWhiteSpace(origin))
        .Select(origin => origin.Trim().TrimEnd('/'))
        .Distinct(StringComparer.OrdinalIgnoreCase)
        .ToArray();

    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(
            policy =>
            {
                policy
                    .SetIsOriginAllowed(origin =>
                        allowedOrigins.Contains(
                            origin.TrimEnd('/'),
                            StringComparer.OrdinalIgnoreCase
                        )
                    )
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            }
        );
        options.AddPolicy(
            "DisplayCors",
            policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()
        );
        options.AddPolicy(
            "AppCors",
            policy =>
            {
                policy
                    .SetIsOriginAllowed(origin =>
                        allowedOrigins.Contains(
                            origin.TrimEnd('/'),
                            StringComparer.OrdinalIgnoreCase
                        )
                    )
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            }
        );
    });

    var modelBuilder = new ODataConventionModelBuilder();

    builder.Services.AddApplication(modelBuilder).AddInfrastructure(systemConfig);

    builder
        .Services.AddControllers()
        .AddOData(options =>
        {
            options
                .AddRouteComponents("odata", modelBuilder.GetEdmModel())
                .Select()
                .Filter()
                .OrderBy()
                .Expand()
                .Count()
                .SetMaxTop(100);
        })
        .AddJsonOptions(o =>
        {
            o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            o.JsonSerializerOptions.PropertyNamingPolicy = null;
        });
}

var app = builder.Build();
{
    app.UseHttpsRedirection();

    app.UsePathBase(new PathString("/api"));
    app.UseStaticFiles();
    app.UseRouting();

    app.UseCors();

    app.UseAuthorization();

    app.MapRazorPages();
    app.MapHealthChecks("/healthz");
    app.MapControllers();
    app.Run();
}
