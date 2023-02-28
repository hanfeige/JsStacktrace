using JsStacktrace.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Services;


namespace JsStacktrace
{
    [Dependency(ServiceLifetime.Singleton)]
    public class PageManager : DomainService
    {
        string[] browserArgs = new string[] { "--start-maximized", "--disable-site-isolation-trials", "--disable-blink-features", "--disable-blink-features=AutomationControlled" };
        string pageInitScript = @"
            Object.defineProperties(navigator, {webdriver:{get:()=>undefined}});
            console.log = function() {};
            console.warn = function() {};
            console.info = function() {};
            console.debug = function() {};
            console.dir = function() {};
            console.error = function() {};
        ";
        private IBrowserContext Context;
        private TestService _testService;
        public PageManager(TestService testService)
        {
            _testService = testService;
        }
        /// <summary>
        /// close BrowserContext and return open urls.
        /// </summary>
        /// <returns></returns>
        public async Task<ICollection<string>> CloseAsync()
        {
            var urls = Context.Pages.Where(p => p.Url.Contains("http")).Select(x => x.Url).ToList();
            urls.ForEach(x => Console.WriteLine($"全部关闭：{x}"));
            await Context.CloseAsync();
            await Context.DisposeAsync();
            if (Context.Browser != null)
            {
                await Context.Browser.CloseAsync();
                await Context.Browser.DisposeAsync();
            }
            Context = null;
            return urls;
        }
        public async Task<IPage> OpenPageAsync(string url = "about:blank")
        {
            if (Context == null || Context.Pages.Count == 0)
            {
                var path = System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                path = Path.Combine(path, "mycache");
                var playwright = await Playwright.CreateAsync();
                Context = await playwright.Chromium.LaunchPersistentContextAsync(path, new()
                {
                    Channel = "chrome",
                    Headless = false,
                    Args = browserArgs,
                    IgnoreDefaultArgs = new string[] { "--disable-extensions", "--no-sandbox", "--no-initial-navigation", "--enable-automation", "--no-default-browser-check" },
                    ViewportSize = ViewportSize.NoViewport
                });
                Context.Page += async (sender, newPage) =>
                {
                    // Console.WriteLine($"page：{newPage.Url}");
                    await newPage.AddInitScriptAsync(pageInitScript);
                    newPage.FrameNavigated += async (a, b) =>
                    {
                        await _testService.InitAsync(b);
                    };
                };
            }
            var page = await Context.NewPageAsync();
            await page.GotoAsync(url);
            return page;
        }
    }
}
