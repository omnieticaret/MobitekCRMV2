using MobitekCRMV2.Entity.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace MobitekCRMV2.Entity.Entities
{
    public class Todo : EntityBase<string>
    {
        public string Title { get; set; }
        public string Description { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DueDate { get; set; }
        public string Response { get; set; }
        public TodoStatus Status { get; set; }


        #region Navigation Properties
        public string OwnerId { get; set; }
        public User Owner { get; set; }

        public string ProjectId { get; set; }
        public Project Project { get; set; }

        public string TodoGroupId { get; set; }
        public TodoGroup TodoGroup { get; set; }

        public string TemplateItemId { get; set; }
        public TemplateItem TemplateItem { get; set; }

        #endregion


    }
}
