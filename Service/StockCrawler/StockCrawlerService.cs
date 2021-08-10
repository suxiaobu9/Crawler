using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Model;
using Model.StockCrawler.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Service.StockCrawler
{
    public class StockCrawlerService : ICrawlerService
    {
        public CrawlerEnum CrawlerType => CrawlerEnum.Stock;
        private readonly StockCrawlerDb _db;
        private readonly ILogger<StockCrawlerService> _logger;
        public StockCrawlerService(StockCrawlerDb db, ILogger<StockCrawlerService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public Task ProcessAsync()
        {
            return SyncCompanyAsync();
        }

        public async Task SyncCompanyAsync()
        {
            var companyCsvs = new Dictionary<StockEnum, string>
            {
                { StockEnum.Listing, await GetAsync("https://mopsfin.twse.com.tw/opendata/t187ap03_L.csv") },
                { StockEnum.MainBoard, await GetAsync("http://mopsfin.twse.com.tw/opendata/t187ap03_O.csv") }
            };

            var sourceCompanys = companyCsvs.Select(csv =>
            {
                var companysFromCsv = csv.Value.Split(Environment.NewLine).Skip(1)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(csvRow =>
                 {
                     var csvCompanyInfo = csvRow.Split("\",\"");
                     return new Company
                     {
                         Id = csvCompanyInfo[1],
                         Name = csvCompanyInfo[2],
                         Type = (int)csv.Key,
                         Nickname = csvCompanyInfo[3],
                         ForeignCompanyRegistration = csvCompanyInfo[4].Length > 2 ? csvCompanyInfo[4] : null,
                         Industry = csvCompanyInfo[5],
                         UniformCode = csvCompanyInfo[7],
                         ChairmanOfBoard = csvCompanyInfo[8],
                         GeneralManager = csvCompanyInfo[9],
                         EstablishmentDate = new DateTime(Convert.ToInt32(csvCompanyInfo[14].Substring(0, 4)), Convert.ToInt32(csvCompanyInfo[14].Substring(4, 2)), Convert.ToInt32(csvCompanyInfo[14].Substring(6, 2))),
                         ListingDate = new DateTime(Convert.ToInt32(csvCompanyInfo[15].Substring(0, 4)), Convert.ToInt32(csvCompanyInfo[15].Substring(4, 2)), Convert.ToInt32(csvCompanyInfo[15].Substring(6, 2))),
                         EnglishNickname = csvCompanyInfo[27],
                         EnglishAddress = csvCompanyInfo[28],
                         Email = csvCompanyInfo[30],
                         Url = csvCompanyInfo[31]
                     };
                 });
                return companysFromCsv;
            }).SelectMany(x => x).ToDictionary(x => x.Id, x => x);

            var allCompany = (await _db.Companies.ToListAsync()).ToDictionary(x => x.Id, x => x);

            foreach (var item in sourceCompanys)
            {
                if (!allCompany.Any(x => x.Key == item.Key))
                    await _db.Companies.AddAsync(item.Value);
                if (allCompany.Any(x => x.Key == item.Key && allCompany.First(x => x.Key == item.Key).Value.IsDelete))
                    allCompany.First(x => x.Key == item.Key).Value.IsDelete = false;
            }

            foreach (var item in allCompany)
            {
                if (!sourceCompanys.Any(x => x.Key == item.Key))
                    item.Value.IsDelete = true;
            }

            await _db.SaveChangesAsync();
        }


        private async Task<string> GetAsync(string url)
        {
            using var httpClient = new HttpClient();
            return await httpClient.GetStringAsync(url);
        }
    }
}
