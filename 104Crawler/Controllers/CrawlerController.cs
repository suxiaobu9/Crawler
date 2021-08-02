using _104Crawler.Model;
using _104Crawler.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _104Crawler.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CrawlerController : ControllerBase
    {
        private readonly ICrawler _crawler;
        private readonly ILogger<CrawlerController> _logger;

        public CrawlerController(ICrawler crawler, ILogger<CrawlerController> logger)
        {
            _crawler = crawler;
            _logger = logger;
        }

        [HttpPut]
        public IActionResult Put()
        {
            _logger.LogInformation("Start");
            _crawler.Process();
            _logger.LogInformation("Finish");
            return Ok();
        }
    }
}
