using MobitekCRMV2.Entity.Enums;
using System;

namespace MobitekCRMV2.Entity.Entities
{
    public class KeywordValue : EntityBase<string>
    {
        public int Position { get; set; }// apiden gelen veri
        public int Page { get; set; }//boş kalacak
        public string Domain { get; set; }// dbden gelen proje domain
        public string Link { get; set; }//apiden gelen link
        public string Title { get; set; }//boş kalacak
        public string Description { get; set; }//boş kalacak

        public string CountryCode { get; set; }// birden çok  olanlar için
        public DateTime CreatedDate { get; set; }// oto dolduruluyor nesne oluşturulurken
        public DateTime UpdatedDate { get; set; }//gerekli yerde doldurulacak

        public TargetCheckType TargetCheckType { get; set; }

        #region Navigation Properties
        public string KeywordId { get; set; }
        public Keyword Keyword { get; set; }
        #endregion


        public KeywordValue()
        {
            CreatedDate = DateTime.Now;

        }
    }
}

