using System.ComponentModel.DataAnnotations;

namespace MobitekCRMV2.Dto.Dtos
{
    public class ProjectListDto
    {
        public string Id { get; set; }
        public string Url { get; set; }
        public string Budget { get; set; }
        public string ProjectType { get; set; }

        public string Contracts { get; set; }
        public string ExpertName { get; set; }
        public string ExpertId { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ReportDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime StatusUpdateDate { get; set; }
        public string ElapsedTime { get; set; }


    }
}
