using MobitekCRMV2.Entity.Enums;

namespace MobitekCRMV2.Entity.Entities
{
    /// <summary>
    /// //Tanıtım Classı
    /// </summary>
    public class Promotion : EntityBase<string>
    {

        public string PromotionURL { get; set; }
        public string LandingPage { get; set; }
        public DateTime Date { get; set; }

        public Status Status { get; set; }
        public Status GoogleIndex { get; set; }

        #region Navigation Properties
        public List<NewsSite> NewsSites { get; set; }
        public List<Keyword> Keywords { get; set; }
        public string ProjectId { get; set; }
        public Project Project { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public string CustomerId { get; set; }
        public Customer Customer { get; set; }
        #endregion
    }
}
