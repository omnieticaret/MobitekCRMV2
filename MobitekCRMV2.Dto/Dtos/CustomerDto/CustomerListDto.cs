using MobitekCRMV2.Entity.Entities;
using MobitekCRMV2.Entity.Enums;

namespace MobitekCRMV2.Dto.Dtos.CustomerDto
{
    public class CustomerListDto
    {
        public string Id { get; set; }
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyEmail { get; set; }
        public string CompanyPhone { get; set; }
        public string CompanyOfficialWebsite { get; set; }
        public string CustomerType { get; set; }
        public string CustomerRepresentative { get; set; }
        public string Projects { get; set; }


    }
}
