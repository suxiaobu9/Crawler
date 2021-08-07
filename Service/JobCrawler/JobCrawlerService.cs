using Microsoft.Extensions.Logging;
using Model;
using Model.JobCrawler;
using Model.JobCrawler.Entity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Service.JobCrawler
{
    public class JobCrawlerService : ICrawlerService
    {
        /// <summary>
        /// 爬蟲類型
        /// </summary>
        public CrawlerEnum CrawlerType => CrawlerEnum.Job;

        private readonly JobCrawlerDb _dbContext;
        private readonly ILogger<JobCrawlerService> _logger;
        private readonly object compLocker = new();
        private readonly object jobLocker = new();
        public JobCrawlerService(JobCrawlerDb context, ILogger<JobCrawlerService> logger)
        {
            _dbContext = context;
            _logger = logger;
        }

        /// <summary>
        /// 執行爬蟲
        /// </summary>
        public async Task ProcessAsync()
        {
            var counter = 1;

            var initData = await GetAsync(1);

            if (initData == null || !initData.data.list.Any())
                return;

            var taskList = new List<Task<JobResponseModel.List[]>>();

            for (var i = 0; i <= initData.data.totalPage; i++)
            {
                taskList.Add(Task.Run(async () =>
                {
                    var jobResponsData = await GetAsync(counter++);
                    return jobResponsData.data.list;
                }));
            }

            var dirtyJobInfos = (await Task.WhenAll(taskList)).SelectMany(x => x);

            var companies = new List<JobResponseModel.List>();
            var hash = new HashSet<string>();
            // 處理重複資料
            foreach (var item in dirtyJobInfos.GroupBy(x => x.custNo).Select(x => x.Key))
            {
                if (hash.Contains(item))
                    continue;

                hash.Add(item);
                companies.Add(dirtyJobInfos.FirstOrDefault(x => x.custNo == item));
            }

            // 同步公司資料
            await SyncCompAsync(companies);

            hash = new HashSet<string>();

            var jobs = new List<JobResponseModel.List>();
            // 處理重複職缺
            foreach (var item in dirtyJobInfos.GroupBy(x => x.jobNo).Select(x => x.Key))
            {
                if (hash.Contains(item))
                    continue;
                hash.Add(item);
                jobs.Add(dirtyJobInfos.FirstOrDefault(x => x.jobNo == item));
            }
            // 同步職缺資料
            await SyncVacancyAsync(jobs);
        }

        /// <summary>
        /// 取得104回應
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private static async Task<string> Get104ResponseAsync(string url)
        {
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Referrer = new Uri("https://www.104.com.tw");

            return await httpClient.GetStringAsync(url);


        }

        // 取得104職缺資料(頁)
        private async Task<JobResponseModel> GetAsync(int page)
        {
            //var url = $"https://www.104.com.tw/jobs/search/list?ro=1&jobcat=2007001000&kwop=7&keyword=C%23%20.net%20core&expansionType=area%2Cspec%2Ccom%2Cjob%2Cwf%2Cwktm&area=6001001000%2C6001002000&order=14&asc=0&s9=1&s5=0&wktm=1&page={page}&mode=l&jobsource=2018indexpoc&searchTempExclude=2";
            var url = $"https://www.104.com.tw/jobs/search/list?ro=1&jobcat=2007001000&isnew=0&kwop=7&keyword=C%23%20.net%20core&expansionType=area%2Cspec%2Ccom%2Cjob%2Cwf%2Cwktm&area=6001001000%2C6001002000&order=14&asc=0&s9=1&s5=0&wktm=1&page={page}&mode=l&jobsource=2018indexpoc&searchTempExclude=2";
            var responseBody = await Get104ResponseAsync(url);

            var body = JsonConvert.DeserializeObject<JobResponseModel>(responseBody);

            return body;
        }

        /// <summary>
        /// 同步公司資料
        /// </summary>
        /// <param name="companys"></param>
        private async Task SyncCompAsync(IEnumerable<JobResponseModel.List> companys)
        {
            var now = DateTime.Now;
            var dbCompanies = await Task.Run(() => { return _dbContext.Companies.Where(x => companys.Select(y => y.custNo).Contains(x.No)).ToList(); });

            var taskList = companys.Select(item => Task.Run(async () =>
            {
                var companyInfo = await GetDetailDataAsync<CompanyResponseModel>(item.link.cust, "company");

                if (dbCompanies.Any(x => x.No == item.custNo))
                {
                    var existCompany = dbCompanies.FirstOrDefault(x => x.No == item.custNo);
                    if (now.Date.Equals(existCompany.LatestCheckDate.Date))
                        return;

                    existCompany.Welfare = companyInfo.data.welfare;
                    return;
                }

                var newCompany = new Company
                {
                    No = item.custNo,
                    Name = item.custName,
                    SourceFrom = "104",
                    Link = item.link.cust,
                    Welfare = companyInfo.data.welfare
                };
                lock (compLocker)
                {
                    _dbContext.Companies.Add(newCompany);
                }
            }));

            await Task.WhenAll(taskList);

            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// 同步職缺資料
        /// </summary>
        /// <param name="jobs"></param>
        private async Task SyncVacancyAsync(IEnumerable<JobResponseModel.List> jobs)
        {
            var dbVacancies = await Task.Run(() => { return _dbContext.Vacancies.Where(x => !x.IsDelete && jobs.Select(y => y.jobNo).Contains(x.No)).ToList(); });

            var taskList = jobs.Select(item => Task.Run(async () =>
               {
                   // 取得職缺詳細資料
                   var jobInfo = await GetDetailDataAsync<VacancyResponseModel>(item.link.job, "job");

                   var expIdx = jobInfo.data.condition.workExp.IndexOf("年");
                   int? workExp = expIdx < 0 ? null : int.Parse(jobInfo.data.condition.workExp.Substring(0, expIdx));

                   var newVacancy = new Vacancy
                   {
                       CompanyNo = item.custNo,
                       No = item.jobNo,
                       Name = item.jobName,
                       Description = jobInfo.data.jobDetail.jobDescription,
                       Required = jobInfo.data.condition.other,
                       Seniority = workExp,
                       SalaryMin = jobInfo.data.jobDetail.salaryMin,
                       SalaryMax = jobInfo.data.jobDetail.salaryMax,
                       AppearDate = DateTime.Parse(jobInfo.data.header.appearDate),
                       Link = item.link.job
                   };

                   if (dbVacancies.Any(x => x.No == item.jobNo))
                   {
                       var existVacancy = dbVacancies.FirstOrDefault(x => !x.IsDelete && x.No == item.jobNo);
                       if (existVacancy.AppearDate.Date.Equals(newVacancy.AppearDate.Date))
                           return;

                       existVacancy.Name = newVacancy.Name;
                       existVacancy.Description = newVacancy.Description;
                       existVacancy.Required = newVacancy.Required;
                       existVacancy.Seniority = newVacancy.Seniority;
                       existVacancy.SalaryMin = newVacancy.SalaryMin;
                       existVacancy.SalaryMax = newVacancy.SalaryMax;
                       existVacancy.AppearDate = newVacancy.AppearDate;
                       return;
                   }
                   lock (jobLocker)
                   {
                       _dbContext.Vacancies.Add(newVacancy);
                   }
               }));

            await Task.WhenAll(taskList);

            _dbContext.SaveChanges();
        }

        /// <summary>
        /// 取得職缺詳細資料
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private async Task<T> GetDetailDataAsync<T>(string url, string type)
        {
            var codeSpliteTmp = url.Split('?')[0].Split('/');
            var code = codeSpliteTmp[codeSpliteTmp.Length - 1];
            // https://www.104.com.tw/job/ajax/content/79pg1
            // https://www.104.com.tw/{type}/ajax/content/{code}
            var detailInfoUrl = $"https://www.104.com.tw/{type}/ajax/content/{code}";
            var response = await Get104ResponseAsync(detailInfoUrl);
            var info = JsonConvert.DeserializeObject<T>(response);
            return info;
        }

    }
}
