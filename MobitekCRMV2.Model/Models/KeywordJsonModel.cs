using System.Collections.Generic;

namespace MobitekCRMV2.Model.Models
{
    public class KeywordJsonModel
    {
        public int position { get; set; }
        public int page { get; set; }
        public string domain { get; set; }
        public string link { get; set; }
        public string title { get; set; }
        public string description { get; set; }

        public string CountryCode { get; set; }
    }

    public class Root
    {
        public List<KeywordJsonModel> organic_results { get; set; }
    }
}
