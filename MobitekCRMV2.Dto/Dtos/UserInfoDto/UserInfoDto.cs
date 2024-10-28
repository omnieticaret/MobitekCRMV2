using MobitekCRMV2.Dto.Dtos.UsersDto;

namespace MobitekCRMV2.Dto.Dtos.UserInfoDto
{
    public class UserInfoDto
    {
        public string Id { get; set; }
        public string Iban { get; set; }
        public string BankName { get; set; }
        public string Tc { get; set; }
        public decimal Salary { get; set; }
        public bool Kdv { get; set; }

        public string UserId { get; set; }
        public UserSummaryDto User { get; set; }
    }

    public class UserInfoListDto
    {
        public List<UserInfoSummaryDto> UserInfos { get; set; }
        public int TotalCount { get; set; }
    }

    public class UserInfoSummaryDto
    {
        public string Id { get; set; }
        public string BankName { get; set; }
        public bool Kdv { get; set; }
        public UserBasicDto User { get; set; }
    }
    public class UserInfoCreateUpdateDto
    {
        public string Iban { get; set; }
        public string BankName { get; set; }
        public string Tc { get; set; }
        public decimal Salary { get; set; }
        public bool Kdv { get; set; }
        public string UserId { get; set; }
    }

    public class UserBasicDto
    {
        public string Id { get; set; }
        public string UserName { get; set; }
    }

    public class UserInfoFilterDto
    {
        public string BankName { get; set; }
        public bool? Kdv { get; set; }
        public string UserId { get; set; }
        public decimal? MinSalary { get; set; }
        public decimal? MaxSalary { get; set; }
    }
    public class UserInfoFinancialSummaryDto
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public FinancialDetailsDto FinancialDetails { get; set; }
    }

    public class FinancialDetailsDto
    {
        public decimal Salary { get; set; }
        public bool Kdv { get; set; }
        public decimal NetAmount { get; set; }
        public BankDetailsDto BankDetails { get; set; }
    }

    public class BankDetailsDto
    {
        public string BankName { get; set; }
        public string MaskedIban { get; set; }

        public class UserInfoBulkUpdateDto
        {
            public List<string> UserIds { get; set; }
            public decimal? SalaryAdjustment { get; set; }
            public bool? Kdv { get; set; }
        }
    }
}