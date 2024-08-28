namespace MobitekCRMV2.Entity.Entities
{
    public class KeywordInfo : EntityBase<string>
    {
        public int position { get; set; }
        public int page { get; set; }
        public string domain { get; set; }
        public string link { get; set; }
        public string title { get; set; }
        public string description { get; set; }



        #region Navigation Properties
        public string KeywordId { get; set; }
        public Keyword Keyword { get; set; }
        #endregion
    }
}
