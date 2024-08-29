namespace MobitekCRMV2.Model.Models
{
    public class KeywordHistoryModel
    {
        public List<HistoryAndPosition> HistoryList { get; set; }


        public KeywordHistoryModel()
        {
            HistoryList = new List<HistoryAndPosition>();
        }
    }

    public class HistoryAndPosition
    {
        public int Position { get; set; }
        public string Date { get; set; }
    }
}
