using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public interface ICrawlerService
    {
        /// <summary>
        /// 爬蟲類型
        /// </summary>
        public CrawlerEnum CrawlerType { get; }

        /// <summary>
        /// 執行爬蟲
        /// </summary>
        public void Process();
    }
}
