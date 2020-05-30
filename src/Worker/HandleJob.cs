using System;
using System.Threading;
using System.Threading.Tasks;
using Contracts;
using Microsoft.Extensions.Logging;

namespace Worker
{
    public class HandleJob : IHandleJob
    {
        private readonly ILogger<HandleJob> _logger;

        public HandleJob(ILogger<HandleJob> logger)
        {
            _logger = logger;
        }

        public async Task HandleAsync(string jobId, CancellationToken ct)
        {
            try
            {
                _logger.LogInformation($"Starting {nameof(HandleAsync)} with {jobId}");

                await Task.Delay(TimeSpan.FromSeconds(30), ct);

                _logger.LogInformation($"Finishing {nameof(HandleAsync)} with {jobId}");
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation($"Cancelled {nameof(HandleAsync)} with {jobId}");
                throw;
            }
        }
    }
}