using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Hephaestus.Extensions.Buffers.Json;
using Hephaestus.Extensions.Sandbox.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Hephaestus.Extensions.Sandbox
{
    public class ExampleService : IHostedService
    {
        private readonly ILogger<ExampleService> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly System.Timers.Timer _timer;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public ExampleService(ILogger<ExampleService> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;

            _jsonSerializerOptions = new JsonSerializerOptions
            {
                IncludeFields = true
            };

            _timer = new System.Timers.Timer
            {
                Interval = 1000 * 5,
                Enabled = false
            };

            _timer.Elapsed += TimerElapsed;
        }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                _timer.Enabled = false;

                Task.Run(async () =>
                {
                    await Task.CompletedTask;

                    using (var cancellationTokenSource = new CancellationTokenSource())
                    {
                        using (var scope = _serviceScopeFactory.CreateScope())
                        {
                            var values = new List<SampleA>();

                            for (var i = 0; i < 1000; i++)
                            {
                                values.Add(new SampleA
                                {
                                    Field01 = 1,
                                    Field02 = 2,
                                    Field03 = new SampleB
                                    {
                                        Field01 = 3,
                                        Field02 = 4,
                                        Field03 = new SampleC
                                        {
                                            Field01 = 5,
                                            Field02 = 6,
                                            Field03 = new SampleD
                                            {
                                                Field01 = 7,
                                                Field02 = 8
                                            }
                                        }
                                    }
                                });
                            }

                            using (var writer = new JsonChunkWriter())
                            {
                                writer.Serialize(values, _jsonSerializerOptions);

                                var values1 = writer.Deserialize<SampleA[]>(_jsonSerializerOptions);
                            }
                        }
                    }
                }).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while running the service");
            }
            finally
            {
                _timer.Enabled = true;
            }
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _timer.Enabled = true;

            await Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer.Enabled = false;

            return Task.CompletedTask;
        }
    }
}
