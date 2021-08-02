using _104Crawler.Model;
using _104Crawler.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        public CrawlerController(ICrawler crawler)
        {
            _crawler = crawler;
        }

        [HttpPut]
        public IActionResult Put()
        {
            _crawler.Process();

            return Ok();
        }
    }
}
