using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobitekCRMV2.Dto.Dtos.CustomerDto
{

    public class CustomerDetailDto
    {
        public string Id { get; set; }
        public string CompanyName { get; set; }
        public string CustomerRepresentativeId { get; set; }
        public string CustomerRepresentativeName { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyEmail { get; set; }
        public string CompanyPhone { get; set; }
        public string CompanyOfficialWebsite { get; set; }
        public string CustomerType { get; set; }
        public List<ProjectDto> Projects { get; set; }
    }

    public class ProjectDto
    {
        public string Id { get; set; }
        public string ProjectType { get; set; }
        public string Status { get; set; }
        public string Budget { get; set; }
        public string Contract { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class CustomerRepresentativeDto
    {
        public string Id { get; set; }
        public string UserName { get; set; }
    }


}
