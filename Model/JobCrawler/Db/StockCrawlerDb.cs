using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Model.Appsettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.StockCrawler.Entity
{
    public class StockCrawlerDb : StockCrawlerContext
    {
        private readonly DbConnections _dbConnection;
        public StockCrawlerDb(IOptions<DbConnections> dbConnection) : base()
        {
            _dbConnection = dbConnection.Value;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_dbConnection.StockCrawlerDb);
        }
    }
}
