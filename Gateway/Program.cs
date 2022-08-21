using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

namespace Gateway;

public class Program
{
    public static void Main()
    {
        IConfiguration configuration = null; 
        new WebHostBuilder()
        .UseKestrel()
        .UseContentRoot(Directory.GetCurrentDirectory())
        .ConfigureAppConfiguration((hostingContext, config) =>
        {
            configuration = config
                .SetBasePath(hostingContext.HostingEnvironment.ContentRootPath)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile("ocelot.json")
                .AddEnvironmentVariables()
                .Build();                                                   
        })
        .ConfigureServices(s => {
            var jwtSettings = configuration?.GetSection("JwtBearerSettings").Get<JwtBearerSettings>();
            s.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(jwtSettings.Key, options =>
            {
                options.Authority = jwtSettings.Authority;
                options.Audience = jwtSettings.Audience;
            });
            s.AddAuthorization(); 
            s.AddOcelot();
        })
        .ConfigureLogging((_, _)  => { })
        .UseIISIntegration()
        .Configure(async app =>
        {
            var configuration = new OcelotPipelineConfiguration
            {
                PreAuthorizationMiddleware = SetupOcelotPipelineConfiguration.AddEmailToHeaderMiddleware
            };
            await app.UseOcelot(configuration);
        })
        .Build()
        .Run();
    }
}