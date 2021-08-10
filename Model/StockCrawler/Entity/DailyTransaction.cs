using System;
using System.Collections.Generic;

#nullable disable

namespace Model.StockCrawler.Entity
{
    public partial class DailyTransaction
    {
        public int Id { get; set; }
        public string CompanyId { get; set; }
        public long? TotalStock { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal? OpeningPrice { get; set; }
        public decimal? HighestPrice { get; set; }
        public decimal? LowestPrice { get; set; }
        public decimal? ClosingPrice { get; set; }
        public decimal? UpsDowns { get; set; }
        public int? TransactionNumbers { get; set; }

        public virtual Company Company { get; set; }
    }
}
