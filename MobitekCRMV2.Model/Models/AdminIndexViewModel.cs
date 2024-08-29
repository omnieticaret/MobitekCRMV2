namespace MobitekCRMV2.Model.Models
{
    public class AdminIndexViewModel
    {

        public List<KeyValueModel> UserStatistics { get; set; }
        public List<KeyValueModel> KeywordValueStatistics { get; set; }
        public List<KeyValueModel> TodoStatistics { get; set; }

    }


    public class KeyValueModel
    {
        public string Key { get; set; }
        public string Value { get; set; }

    }


}
