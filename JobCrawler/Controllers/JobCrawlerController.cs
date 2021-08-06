using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Model;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobCrawler.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobCrawlerController : ControllerBase
    {
        private readonly ILogger<JobCrawlerController> _logger;
        private readonly ICrawlerService _jobService;

        public JobCrawlerController(ILogger<JobCrawlerController> logger, IEnumerable<ICrawlerService> jobService)
        {
            _logger = logger;
            _jobService = jobService.First(x => x.CrawlerType == CrawlerEnum.Job);
        }

        /// <summary>
        /// API同步公司、職缺資料
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        public IActionResult Put()
        {
            _logger.LogInformation($"{DateTime.Now:yyyy/MM/dd HH:mm:ss} : Start");
            _jobService.Process();
            _logger.LogInformation($"{DateTime.Now:yyyy/MM/dd HH:mm:ss} : Finish");
            return Ok();
        }

    }
}
