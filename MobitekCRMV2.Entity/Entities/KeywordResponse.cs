using System;

namespace MobitekCRMV2.Entity.Entities
{
    /// <summary>
    /// Keyword inputuna karşılık olarak geri dönülecek olan class
    /// </summary>
    public class KeywordResponse : EntityBase<string>
    {
        public string SerpURL { get; set; } //Serp URL
        public DateTime Date { get; set; } //Tarih
        public string Position { get; set; } //Konum

        #region Navigation Properties
        public string KeywordId { get; set; }
        public Keyword Keyword { get; set; }
        #endregion

    }
}
