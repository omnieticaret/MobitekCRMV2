namespace MobitekCRMV2.Model.Models
{
    public class SetKeywordValueModel
    {
        public DateTime TargetDate { get; set; }
        public DateTime SourceDate { get; set; }

        public string Message { get; set; }


        public List<DateCountModel> RecordList { get; set; }
    }


    public class DateCountModel
    {
        public DateTime Date { get; set; }
        public int Count { get; set; }
    }
}
