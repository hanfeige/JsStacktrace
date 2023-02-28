using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Playwright;
using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Services;

namespace JsStacktrace.Worker
{
    [Dependency(ServiceLifetime.Singleton)]
    public class TestService:DomainService
    {
        public const string initScript =@"
            Object.defineProperties(navigator, {webdriver:{get:()=>undefined}});
        ";
        private IFrame pageFrame = null;
        public async Task InitAsync(IFrame frame)
        {
            try
            {
                Logger.LogInformation($"url：{frame.Url}");
                if (frame.Url.Contains("playwright.dev"))
                {
                    await frame.AddScriptTagAsync(new FrameAddScriptTagOptions
                    {
                        Content = initScript
                    });
                    pageFrame = frame;
                    Logger.LogInformation($"conected:{pageFrame.Page.Url}");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"init exception：{ex.Message}");
            }
        }
        private string CMD_GET_Data = @"() => {
            var result=[];
            for(var i=0;i<10000;i++)
            {
                result.push({a:i+'ddsfjasjfdsjfjdsfjdsjfsjfsd',b:i+123445,c:'&%$#$%^&*(*&^',d:'mmsas8343jd'});
            }
            return result;
        }";
        public async Task PrintAsync()
        {
            if (pageFrame == null) return;

            var result = (await pageFrame.EvaluateAsync(CMD_GET_Data)).Value;

            Logger.LogInformation($"data length：{result.ToString().Length}");
        }
    }
}
