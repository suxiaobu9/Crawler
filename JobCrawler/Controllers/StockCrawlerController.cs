using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    public class StockCrawlerController : ControllerBase
    {
        private readonly ICrawlerService _stockCrawlerService;
        public StockCrawlerController(IEnumerable<ICrawlerService> crawlerService)
        {
            _stockCrawlerService = crawlerService.FirstOrDefault(x => x.CrawlerType == CrawlerEnum.Stock);
        }

        [HttpPut]
        public async Task<IActionResult> PutAsync()
        {
            await _stockCrawlerService.ProcessAsync();
            return Ok();
        }
    }
}
