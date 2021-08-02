using _104Crawler.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace _104Crawler.Test
{
    [TestClass]
    public class CrawlerTest
    {
        [TestMethod]
        public void Method1()
        {
            var service = new Crawler(null);
            service.Get(1);
        }
    }
}
