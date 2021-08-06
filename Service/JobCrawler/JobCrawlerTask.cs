using Microsoft.Extensions.Logging;
using Model;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.JobCrawler
{
    [DisallowConcurrentExecution]
    public class JobCrawlerTask : IJob
    {
        private readonly ILogger<JobCrawlerTask> _logger;
        private readonly ICrawlerService _jobService;

        public JobCrawlerTask(ILogger<JobCrawlerTask> logger, IEnumerable<ICrawlerService> jobService)
        {
            _logger = logger;
            _jobService = jobService.FirstOrDefault(x => x.CrawlerType == CrawlerEnum.Job);
        }

        public Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation($"{DateTime.Now:yyyy/MM/dd HH:mm:ss} : JobCrawler Start");

            _jobService.Process();

            _logger.LogInformation($"{DateTime.Now:yyyy/MM/dd HH:mm:ss} : JobCrawler Finish");

            return Task.CompletedTask;
        }
    }
}
