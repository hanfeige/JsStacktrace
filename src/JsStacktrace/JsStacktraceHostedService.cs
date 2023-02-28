using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Volo.Abp;

namespace JsStacktrace;

public class JsStacktraceHostedService : IHostedService
{
    private readonly IAbpApplicationWithExternalServiceProvider _abpApplication;
    private readonly HelloWorldService _helloWorldService;
    private PageManager _pageManager;

    public JsStacktraceHostedService(HelloWorldService helloWorldService, IAbpApplicationWithExternalServiceProvider abpApplication, PageManager pageManager)
    {
        _helloWorldService = helloWorldService;
        _abpApplication = abpApplication;
        _pageManager= pageManager;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        //await _helloWorldService.SayHelloAsync();
        await _pageManager.OpenPageAsync("https://playwright.dev/dotnet/");
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _abpApplication.ShutdownAsync();
    }
}
