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
        public CrawlerEnum CrawlerType { get; }

        public void Process();
    }
}
