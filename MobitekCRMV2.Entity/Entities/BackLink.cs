using System.ComponentModel.DataAnnotations;

namespace MobitekCRMV2.Entity.Entities
{
    public class BackLink : EntityBase<string>
    {

        public string Status { get; set; }
        public string UrlFrom { get; set; }
        public string LandingPage { get; set; }
        public string Anchor { get; set; }
        public string SelectDate { get; set; }
        public int Da { get; set; }
        public int Pa { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime LastUpdateDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ManualCheckDate { get; set; }

        #region Navigation Properties
        public string DomainId { get; set; }
        virtual public Domain Domain { get; set; }


        public string NewsSiteId { get; set; }
        virtual public NewsSite NewsSite { get; set; }

        #endregion
    }
}