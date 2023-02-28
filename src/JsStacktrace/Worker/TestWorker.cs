using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Threading;

namespace JsStacktrace.Worker
{
    public class TestWorker : AsyncPeriodicBackgroundWorkerBase
    {
        public TestWorker(AbpAsyncTimer timer, IServiceScopeFactory serviceScopeFactory) : base(timer, serviceScopeFactory)
        {
            Timer.Period = 1000;//1s
        }
        protected override async Task DoWorkAsync(PeriodicBackgroundWorkerContext workerContext)
        {
            using (var scope = workerContext.ServiceProvider.CreateScope())
            {
                var testService = scope.ServiceProvider.GetService<TestService>();
                await testService.PrintAsync();
            }
        }
    }
}
