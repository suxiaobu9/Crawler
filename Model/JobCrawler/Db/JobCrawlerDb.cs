using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Model.Appsettings;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.JobCrawler.Entity
{
    public class JobCrawlerDb : JobCrawlerContext
    {
        private readonly DbConnections _dbConnection;
        public JobCrawlerDb(IOptions<DbConnections> dbConnection) : base()
        {
            _dbConnection = dbConnection.Value;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        { 
                optionsBuilder.UseSqlServer(_dbConnection.JobCrawlerDb);
        }
    }
}
