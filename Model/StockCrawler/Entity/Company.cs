using System;
using System.Collections.Generic;

#nullable disable

namespace Model.StockCrawler.Entity
{
    public partial class Company
    {
        public Company()
        {
            DailyTransactions = new HashSet<DailyTransaction>();
        }

        public string Id { get; set; }
        public int Type { get; set; }
        public string Name { get; set; }
        public string Nickname { get; set; }
        public string ForeignCompanyRegistration { get; set; }
        public string Industry { get; set; }
        public string UniformCode { get; set; }
        public string ChairmanOfBoard { get; set; }
        public string GeneralManager { get; set; }
        public DateTime EstablishmentDate { get; set; }
        public DateTime ListingDate { get; set; }
        public string EnglishNickname { get; set; }
        public string EnglishAddress { get; set; }
        public string Email { get; set; }
        public string Url { get; set; }
        public bool IsDelete { get; set; }

        public virtual ICollection<DailyTransaction> DailyTransactions { get; set; }
    }
}
