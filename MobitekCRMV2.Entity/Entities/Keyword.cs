using System.Collections.Generic;

namespace MobitekCRMV2.Entity.Entities
{
    /// <summary>
    /// Bu kullanıcının input olarak gireceği, ve geri dönüş olarak KeywordResponse alacağı bir class.
    /// </summary>
    public class Keyword : EntityBase<string>
    {

        public string KeywordName { get; set; }
        public string TargetURL { get; set; }
        public string TargetStatus { get; set; }

        public bool IsStarred { get; set; }

        public string MetaTitle { get; set; }

        public string MetaDescription { get; set; }

        public string H1 { get; set; }

        #region Navigation Properties
        public string ProjectId { get; set; }
        public Project Project { get; set; }
        public List<KeywordResponse> KeywordResponses { get; set; }
        public List<KeywordValue> KeywordValues { get; set; }
        public KeywordInfo KeywordInfo { get; set; }
        public List<Promotion> Promotions { get; set; }
        #endregion

    }
}
