using MobitekCRMV2.Dto.Dtos.ProjectsDto;
using MobitekCRMV2.Dto.Dtos.UsersDto;
using MobitekCRMV2.Dto.Dtos.UsersDto;
using MobitekCRMV2.Entity.Enums;

namespace MobitekCRMV2.Dto.Dtos.CustomersDto
{
    public class CustomerDto
    {
        public string Id { get; set; }
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyEmail { get; set; }
        public string CompanyPhone { get; set; }
        public string CompanyOfficialWebsite { get; set; }
        public CustomerType CustomerType { get; set; }

        public string CustomerRepresentativeId { get; set; }
        public UserSummaryDto CustomerRepresentative { get; set; }
        public List<ProjectSummaryDto> Projects { get; set; }
    }
    public class CustomerListDto
    {
        public List<CustomerSummaryDto> Customers { get; set; }
        public int TotalCount { get; set; }
    }

    public class CustomerSummaryDto
    {
        public string Id { get; set; }
        public string CompanyName { get; set; }
        public string CompanyEmail { get; set; }
        public string CompanyPhone { get; set; }
        public CustomerType CustomerType { get; set; }
    }

    public class CustomerCreateUpdateDto
    {
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyEmail { get; set; }
        public string CompanyPhone { get; set; }
        public string CompanyOfficialWebsite { get; set; }
        public CustomerType CustomerType { get; set; }
        public string CustomerRepresentativeId { get; set; }
    }

    public class CustomerFilterDto
    {
        public string CompanyName { get; set; }
        public string CompanyEmail { get; set; }
        public CustomerType? CustomerType { get; set; }
        public string CustomerRepresentativeId { get; set; }
    }
}