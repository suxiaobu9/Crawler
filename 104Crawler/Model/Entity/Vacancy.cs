using System;
using System.Collections.Generic;

#nullable disable

namespace _104Crawler.Model.Entity
{
    public partial class Vacancy
    {
        public string CompanyNo { get; set; }
        public string No { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Required { get; set; }
        public int? Seniority { get; set; }
        public int SalaryMin { get; set; }
        public int SalaryMax { get; set; }
        public DateTime AppearDate { get; set; }
        public string Link { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsDelete { get; set; }

        public virtual Company CompanyNoNavigation { get; set; }
    }
}
