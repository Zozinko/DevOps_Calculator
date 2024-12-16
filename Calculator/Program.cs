using OpenTelemetry.Metrics;
using static System.Net.WebRequestMethods;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

builder.Services.AddOpenTelemetry()
    .WithMetrics(meterProviderBuilder =>
    {
        meterProviderBuilder.AddPrometheusExporter();

        meterProviderBuilder.AddMeter("Microsoft.AspNetCore.Hosting",
			"Microsoft.AspNetCore.Server.Kestrel");

        // Status code 
        meterProviderBuilder.AddMeter("Microsoft.AspNetCore.Http.Connections");


        meterProviderBuilder.AddView("http.server.request.duration",
            new ExplicitBucketHistogramConfiguration
            {
                Boundaries =
                [
                    0,
                    0.005,
                    0.01,
                    0.025,
                    0.05,
                    0.075,
                    0.1,
                    0.25,
                    0.5,
                    0.75,
                    1,
                    2.5,
                    5,
                    7.5,
                    10
                ]
            });
    });


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.MapPrometheusScrapingEndpoint();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Calculator}/{action=Index}/{id?}");

app.Run();
