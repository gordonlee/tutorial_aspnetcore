using Microsoft.Extensions.Hosting;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace Scheduler
{
    public class RandomStringProvider
    {
        private const string RandomStringUri =
            "https://www.random.org/strings/?num=1&len=8&digits=on&upperalpha=on&loweralpha=on&unique=on&format=plain&rnd=new";

        private readonly HttpClient _httpClient;

        public RandomStringProvider()
        {
            _httpClient = new HttpClient();
        }
        public async Task UpdateString(CancellationToken cancellationToken)
        {
            try
            {
                var response = await _httpClient.GetAsync(RandomStringUri, cancellationToken);
                if (response.IsSuccessStatusCode)
                {
                    RandomString = await response.Content.ReadAsStringAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        public string RandomString { get; private set; } = string.Empty;
    }

    public abstract class HostedService : IHostedService
    {
        private Task _executingTask;
        private CancellationTokenSource _cts;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _executingTask = ExecuteAsync(_cts.Token);
            return _executingTask.IsCompleted ? _executingTask : Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_executingTask == null)
            {
                return;
            }

            _cts.Cancel();

            await Task.WhenAny(_executingTask, Task.Delay(-1, cancellationToken));

            cancellationToken.ThrowIfCancellationRequested();
        }

        protected abstract Task ExecuteAsync(CancellationToken cancellationToken);
    }

    public class DataRefreshService : HostedService
    {
        private readonly RandomStringProvider _randomStringProvider;

        public DataRefreshService(RandomStringProvider provider)
        {
            _randomStringProvider = provider;
        }

        protected override async Task ExecuteAsync(CancellationToken cancelltaionToken)
        {
            while (!cancelltaionToken.IsCancellationRequested)
            {
                await _randomStringProvider.UpdateString(cancelltaionToken);
                await Task.Delay(TimeSpan.FromSeconds(5), cancelltaionToken);
            }
        }
    }
    
}
