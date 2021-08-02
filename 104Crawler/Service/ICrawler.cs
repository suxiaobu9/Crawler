using _104Crawler.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _104Crawler.Service
{
    public interface ICrawler
    {
        public void Process();

        public JobResponseModel Get(int page);

        public void SyncComp(IEnumerable<JobResponseModel.List> jobs);

        public void SyncVacancy(IEnumerable<JobResponseModel.List> jobs);

    }
}
