using System;
using System.Collections.Generic;

#nullable disable

namespace Model.JobCrawler.Entity
{
    public partial class Company
    {
        public Company()
        {
            Vacancies = new HashSet<Vacancy>();
        }

        public string No { get; set; }
        public string Name { get; set; }
        public string SourceFrom { get; set; }
        public string Link { get; set; }
        public string Welfare { get; set; }
        public DateTime LatestCheckDate { get; set; }

        public virtual ICollection<Vacancy> Vacancies { get; set; }
    }
}
