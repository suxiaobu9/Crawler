using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace _104Crawler.Model
{
    public class CompanyResponseModel
    {
        public Data data { get; set; }
        public Metadata metadata { get; set; }

        public class Data
        {
            public string custSwitch { get; set; }
            public string custName { get; set; }
            public long custNo { get; set; }
            public string industryDesc { get; set; }
            public string indcat { get; set; }
            public string empNo { get; set; }
            public string capital { get; set; }
            public string address { get; set; }
            public string custLink { get; set; }
            public string profile { get; set; }
            public string product { get; set; }
            public string welfare { get; set; }
            public string management { get; set; }
            public string phone { get; set; }
            public string fax { get; set; }
            public string hrName { get; set; }
            public string lat { get; set; }
            public string lon { get; set; }
            public string logo { get; set; }
            public string news { get; set; }
            public string newsLink { get; set; }
            public Zone zone { get; set; }
            public Linkmore linkMore { get; set; }
            public string corpImage2 { get; set; }
            public string corpImage1 { get; set; }
            public string corpImage3 { get; set; }
            public string corpLink2 { get; set; }
            public string corpLink1 { get; set; }
            public string corpLink3 { get; set; }
            public object[] productPictures { get; set; }
            public Envpicture[] envPictures { get; set; }
            public string[] tagNames { get; set; }
            public string[] legalTagNames { get; set; }
            public object[] historys { get; set; }
            public bool isSaved { get; set; }
            public bool isTracked { get; set; }
            public string addrNoDesc { get; set; }
            public string reportUrl { get; set; }
            public int? postalCode { get; set; }
        }

        public class Zone
        {
        }

        public class Linkmore
        {
        }

        public class Envpicture
        {
            public string file { get; set; }
            public string description { get; set; }
            public int is_cover { get; set; }
            public string link_l { get; set; }
            public string link_s { get; set; }
        }

        public class Metadata
        {
        }

    }
}
