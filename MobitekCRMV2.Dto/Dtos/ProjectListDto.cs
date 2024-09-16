using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobitekCRMV2.Dto.Dtos
{
    public class ProjectListDto
    {
        public string Id { get; set; } 
        public string Url { get; set; }
        public string Budget { get; set; }

        public string ExpertName { get; set; }
        public string ExpertId { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ReportDate { get; set; }
        public string Quantity { get; set; }


    }
}
