using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    /// <summary>
    /// 爬蟲類型
    /// </summary>
    public enum CrawlerEnum
    {
        Job,
        Stock
    }

    public enum StockEnum
    {
        /// <summary>
        /// 上市
        /// </summary>
        Listing = 1,
        /// <summary>
        /// 上櫃
        /// </summary>
        MainBoard = 2
    }
}
