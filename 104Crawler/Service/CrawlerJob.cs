using Microsoft.Extensions.Logging;
using Quartz;
using System.Threading.Tasks;

namespace _104Crawler.Service
{
    [DisallowConcurrentExecution]
    public class CrawlerJob : IJob
    {
        private readonly ILogger<CrawlerJob> _logger;
        private readonly ICrawler _crawler;

        public CrawlerJob(ILogger<CrawlerJob> logger, ICrawler crawler)
        {
            _logger = logger;
            _crawler = crawler;
        }

        public Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("Crawler Start");

            //_crawler.Process();

            _logger.LogInformation("Crawler Finish");

            return Task.CompletedTask;
        }
    }
}
