using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Model;
using Model.StockCrawler.Entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
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

        public async Task ProcessAsync()
        {
            //await SyncCompanyAsync();
            await SyncHistoryStockAsync();
        }

        public async Task SyncCompanyAsync()
        {
            var companyCsv = new Dictionary<StockEnum, string>
            {
                { StockEnum.Listing, await GetAsync("https://mopsfin.twse.com.tw/opendata/t187ap03_L.csv") },
                { StockEnum.MainBoard, await GetAsync("http://mopsfin.twse.com.tw/opendata/t187ap03_O.csv") }
            };

            var sourceCompanys = companyCsv.Select(csv =>
            {
                var companysFromCsv = csv.Value.Split(Environment.NewLine).Skip(1)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(csvRow =>
                 {
                     var csvCompanyInfo = csvRow.Trim('\"').Split("\",\"");
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

        public async Task SyncHistoryStockAsync()
        {
            var allCompanies = await _db.Companies.Where(x => !x.IsDelete).ToListAsync();

            var listingStock = (int)StockEnum.Listing;
            var now = DateTime.Now;
            var listingDate = now.ToString("yyyyMMdd");
            var mainBoardDate = $"{now.Year - 1911}/{now.Month}";

            foreach (var company in allCompanies)
            {
                var skip = listingStock == 1 ? 2 : 5;
                var skipLast = listingStock == 1 ? 4 : 1;
                var url = company.Type == listingStock ?
                    $"https://www.twse.com.tw/exchangeReport/STOCK_DAY?response=csv&date={listingDate}&stockNo={company.Id}" :
                    $"https://www.tpex.org.tw/web/stock/aftertrading/daily_trading_info/st43_download.php?l=zh-tw&d={mainBoardDate}&stkno={company.Id}&s=0,asc,0";

                try
                {
                    var csv = await GetStreamAsync(url);
                    using var reader = new StreamReader(csv, Encoding.GetEncoding("Big5"));
                    var stockInfo = (await reader.ReadToEndAsync()).Split(Environment.NewLine);//.Where(x => !string.IsNullOrWhiteSpace(x)).Skip(skip).SkipLast(skipLast).Select(x=>x.Trim(new char[] { '\"' ,','}).Split("\",\""));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                }
            }


        }


        private static async Task<string> GetAsync(string url)
        {
            using var httpClient = new HttpClient();
            return await httpClient.GetStringAsync(url);
        }


        private static async Task<Stream> GetStreamAsync(string url)
        {
            using var httpClient = new HttpClient();
            return await httpClient.GetStreamAsync(url);
        }
    }
}
