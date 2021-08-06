using Model;
using Model.JobCrawler;
using Model.JobCrawler.Entity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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

        public JobCrawlerService(JobCrawlerDb context)
        {
            _dbContext = context;
        }

        /// <summary>
        /// 執行爬蟲
        /// </summary>
        public void Process()
        {
            var counter = 1;

            var dirtyJobInfos = new List<JobResponseModel.List>();

            while (true)
            {
                // 取得當頁職缺內容
                var jobResponsData = this.Get(counter++);

                if (jobResponsData == null || !jobResponsData.data.list.Any())
                    break;

                dirtyJobInfos.AddRange(jobResponsData.data.list);
            }

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
            this.SyncComp(companies);

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
            this.SyncVacancy(jobs);
        }

        /// <summary>
        /// 取得104回應
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private string Get104Response(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            // 躲驗證
            request.Headers.Add("Referer", "https://www.104.com.tw");
            using var response = (HttpWebResponse)(request.GetResponse());

            if (response.StatusCode != HttpStatusCode.OK)
                return null;

            using var sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            var responseBody = sr.ReadToEnd();
            return responseBody;
        }

        // 取得104職缺資料(頁)
        public JobResponseModel Get(int page)
        {
            //var url = $"https://www.104.com.tw/jobs/search/list?ro=1&jobcat=2007001000&kwop=7&keyword=C%23%20.net%20core&expansionType=area%2Cspec%2Ccom%2Cjob%2Cwf%2Cwktm&area=6001001000%2C6001002000&order=14&asc=0&s9=1&s5=0&wktm=1&page={page}&mode=l&jobsource=2018indexpoc&searchTempExclude=2";
            var url = $"https://www.104.com.tw/jobs/search/list?ro=1&jobcat=2007001000&isnew=0&kwop=7&keyword=C%23%20.net%20core&expansionType=area%2Cspec%2Ccom%2Cjob%2Cwf%2Cwktm&area=6001001000%2C6001002000&order=14&asc=0&s9=1&s5=0&wktm=1&page={page}&mode=l&jobsource=2018indexpoc&searchTempExclude=2";
            var responseBody = Get104Response(url);

            var body = JsonConvert.DeserializeObject<JobResponseModel>(responseBody);

            return body;
        }

        /// <summary>
        /// 同步公司資料
        /// </summary>
        /// <param name="companys"></param>
        public void SyncComp(IEnumerable<JobResponseModel.List> companys)
        {
            var now = DateTime.Now;
            var dbCompanies = _dbContext.Companies.Where(x => companys.Select(y => y.custNo).Contains(x.No)).ToList();
            foreach (var item in companys)
            {
                //取得公司詳細資料
                var companyInfo = GetDetailData<CompanyResponseModel>(item.link.cust, "company");

                if (dbCompanies.Any(x => x.No == item.custNo))
                {
                    var existCompany = dbCompanies.FirstOrDefault(x => x.No == item.custNo);
                    if (now.Date.Equals(existCompany.LatestCheckDate.Date))
                        continue;

                    existCompany.Welfare = companyInfo.data.welfare;
                    continue;
                }

                var newCompany = new Company
                {
                    No = item.custNo,
                    Name = item.custName,
                    SourceFrom = "104",
                    Link = item.link.cust,
                    Welfare = companyInfo.data.welfare
                };

                _dbContext.Companies.Add(newCompany);
            }

            _dbContext.SaveChanges();
        }

        /// <summary>
        /// 同步職缺資料
        /// </summary>
        /// <param name="jobs"></param>
        public void SyncVacancy(IEnumerable<JobResponseModel.List> jobs)
        {
            var dbVacancies = _dbContext.Vacancies.Where(x => !x.IsDelete && jobs.Select(y => y.jobNo).Contains(x.No)).ToList();
            foreach (var item in jobs)
            {
                // 取得職缺詳細資料
                var jobInfo = GetDetailData<VacancyResponseModel>(item.link.job, "job");

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
                        continue;

                    existVacancy.Name = newVacancy.Name;
                    existVacancy.Description = newVacancy.Description;
                    existVacancy.Required = newVacancy.Required;
                    existVacancy.Seniority = newVacancy.Seniority;
                    existVacancy.SalaryMin = newVacancy.SalaryMin;
                    existVacancy.SalaryMax = newVacancy.SalaryMax;
                    existVacancy.AppearDate = newVacancy.AppearDate;
                    continue;
                }
                _dbContext.Vacancies.Add(newVacancy);
            }
            _dbContext.SaveChanges();
        }

        /// <summary>
        /// 取得職缺詳細資料
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public T GetDetailData<T>(string url, string type)
        {
            var codeSpliteTmp = url.Split('?')[0].Split('/');
            var code = codeSpliteTmp[codeSpliteTmp.Length - 1];
            // https://www.104.com.tw/job/ajax/content/79pg1
            // https://www.104.com.tw/{type}/ajax/content/{code}
            var detailInfoUrl = $"https://www.104.com.tw/{type}/ajax/content/{code}";
            var response = Get104Response(detailInfoUrl);
            var info = JsonConvert.DeserializeObject<T>(response);
            return info;
        }


    }
}
