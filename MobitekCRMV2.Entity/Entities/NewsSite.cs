using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MobitekCRMV2.Entity.Entities;

namespace MobitekCRMV2.Entity.Entities
{
    public class NewsSite : EntityBase<string>
    {
        /// <summary>
        /// şu an herhangi bi tabloyla ilişkisi yok
        /// ilerde tanıtım ile ilişkisi olabilir
        /// </summary>
        public string Name { get; set; }
        public int Price { get; set; }
        public string GoogleNews { get; set; }
        public int DRScore { get; set; }
        public int LinkedDomains { get; set; }
        public int TotalBacklinks { get; set; }
        public int OrganicTraffic { get; set; }

        public int AllTraffic { get; set; }
        public int PAScore { get; set; }
        public int DAScore { get; set; }
        public int SpamScore { get; set; }
        public string EditorMail { get; set; } //Editor mail
        public string EditorPhone { get; set; } //Editor telefon numarası
        public int MozDA { get; set; } //Domain Authority // int or string ?

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime LastUpdateDate { get; set; }

        public string OldData { get; set; }

        public string Note { get; set; }

        #region Navigation Properties
        public string PromotionId { get; set; }
        public Promotion Promotion { get; set; }
        public string UserId { get; set; }//editor (User )ile bağlanıcak 
        virtual public User User { get; set; }


        virtual public List<BackLink> BackLinks { get; set; }

        #endregion
    }
}
