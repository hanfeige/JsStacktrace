using System.Threading.Tasks;
using JsStacktrace.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Domain;
using Volo.Abp.Modularity;

namespace JsStacktrace;

[DependsOn(
    typeof(AbpDddDomainModule),
    typeof(AbpBackgroundWorkersModule),
    typeof(AbpAutofacModule)
)]
public class JsStacktraceModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        base.ConfigureServices(context);
    }
    public override Task OnApplicationInitializationAsync(ApplicationInitializationContext context)
    {
        context.AddBackgroundWorkerAsync<TestWorker>();
        var logger = context.ServiceProvider.GetRequiredService<ILogger<JsStacktraceModule>>();
        var configuration = context.ServiceProvider.GetRequiredService<IConfiguration>();
        logger.LogInformation($"MySettingName => {configuration["MySettingName"]}");

        var hostEnvironment = context.ServiceProvider.GetRequiredService<IHostEnvironment>();
        logger.LogInformation($"EnvironmentName => {hostEnvironment.EnvironmentName}");

        return Task.CompletedTask;
    }
}
