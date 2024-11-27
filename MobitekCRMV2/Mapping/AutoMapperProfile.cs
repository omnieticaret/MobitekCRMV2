using AutoMapper;
using MobitekCRMV2.Dto.Dtos.BackLinskDto;
using MobitekCRMV2.Dto.Dtos.ContentsBudgetDto;
using MobitekCRMV2.Dto.Dtos.CustomersDto;
using MobitekCRMV2.Dto.Dtos.DomainDto;
using MobitekCRMV2.Dto.Dtos.KeywordsDto;
using MobitekCRMV2.Dto.Dtos.KeywordsInfoDto;
using MobitekCRMV2.Dto.Dtos.KeywordsResponseDto;
using MobitekCRMV2.Dto.Dtos.KeywordsValueDto;
using MobitekCRMV2.Dto.Dtos.LogsMessageDto;
using MobitekCRMV2.Dto.Dtos.NewsSitesDto;
using MobitekCRMV2.Dto.Dtos.PlatformsDto;
using MobitekCRMV2.Dto.Dtos.ProjectsDto;
using MobitekCRMV2.Dto.Dtos.PromotionsDto;
using MobitekCRMV2.Dto.Dtos.SmPlatformsDto;
using MobitekCRMV2.Dto.Dtos.UserInfoDto;
using MobitekCRMV2.Dto.Dtos.UsersDto;
using MobitekCRMV2.Entity.Entities;

namespace MobitekCRMV2.Mapping
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // Project Mappings
            CreateMap<Project, ProjectDto>()
                .ForMember(dest => dest.StartDate,
                    opt => opt.MapFrom(src => src.StartDate.ToString("dd/MM/yyyy")))
                .ForMember(dest => dest.ReportDate,
                    opt => opt.MapFrom(src => src.ReportDate.ToString("dd/MM/yyyy")))
                .ForMember(dest => dest.MeetingDate,
                    opt => opt.MapFrom(src => src.MeetingDate.ToString("dd/MM/yyyy")))
                .ForMember(dest => dest.StatusUpdateDate,
                    opt => opt.MapFrom(src => src.StatusUpdateDate.ToString("dd/MM/yyyy")))
                .ForMember(dest => dest.CompletedKeywordsCount,
                    opt => opt.MapFrom(src => src.Keywords.Count(k => k.IsStarred)))
                .ForMember(dest => dest.TotalKeywordsCount,
                    opt => opt.MapFrom(src => src.ContractKeywordCount))
                .ForMember(dest => dest.CompletionRate,
                    opt => opt.MapFrom(src => src.ContractKeywordCount > 0
                        ? (decimal)src.Keywords.Count(k => k.IsStarred) / src.ContractKeywordCount * 100
                        : 0));

            CreateMap<ProjectDto, Project>();

            CreateMap<Project, ProjectSummaryDto>()
                .ForMember(dest => dest.CustomerName,
                    opt => opt.MapFrom(src => src.Customer.CompanyName))
                .ForMember(dest => dest.ExpertName,
                    opt => opt.MapFrom(src => src.Expert.UserName))
                .ForMember(dest => dest.KeywordsCount,
                    opt => opt.MapFrom(src => src.Keywords.Count))
                .ForMember(dest => dest.ReportDate,
                    opt => opt.MapFrom(src => src.ReportDate.ToString("dd/MM/yyyy")));

            CreateMap<ProjectSummaryDto, Project>();
            CreateMap<ProjectCreateUpdateDto, Project>();
            CreateMap<Project, ProjectCreateUpdateDto>();

            // User Mappings
            CreateMap<User, UserDto>();
            CreateMap<UserDto, User>();

            CreateMap<User, UserSummaryDto>();
            CreateMap<UserSummaryDto, User>();

            CreateMap<UserDto, UserSummaryDto>();
            CreateMap<UserSummaryDto, UserDto>();

            CreateMap<UserCreateUpdateDto, User>();
            CreateMap<User, UserCreateUpdateDto>();

            // Customer Mappings
            CreateMap<Customer, CustomerDto>();
            CreateMap<CustomerDto, Customer>();

            CreateMap<Customer, CustomerSummaryDto>();
            CreateMap<CustomerSummaryDto, Customer>();

            CreateMap<CustomerCreateUpdateDto, Customer>();
            CreateMap<Customer, CustomerCreateUpdateDto>();

            // Platform Mappings
            CreateMap<Platform, PlatformDto>()
                .ForMember(dest => dest.ProjectCount,
                    opt => opt.MapFrom(src => src.Projects.Count));
            CreateMap<PlatformDto, Platform>();

            CreateMap<Platform, PlatformSummaryDto>();
            CreateMap<PlatformSummaryDto, Platform>();

            CreateMap<PlatformCreateUpdateDto, Platform>();
            CreateMap<Platform, PlatformCreateUpdateDto>();

            // Keyword Mappings
            CreateMap<Keyword, KeywordDto>();
            CreateMap<KeywordDto, Keyword>();
            CreateMap<KeywordDto, KeywordSummaryDto>();
            CreateMap<Keyword, KeywordSummaryDto>();
            CreateMap<KeywordSummaryDto, Keyword>();

            CreateMap<KeywordCreateUpdateDto, Keyword>();
            CreateMap<Keyword, KeywordCreateUpdateDto>();

            // Social Media Platform Mappings
            CreateMap<SmPlatform, SmPlatformDto>();
            CreateMap<SmPlatformDto, SmPlatform>();

            CreateMap<SmPlatform, SmPlatformSummaryDto>();
            CreateMap<SmPlatformSummaryDto, SmPlatform>();

            CreateMap<SmPlatformCreateUpdateDto, SmPlatform>();
            CreateMap<SmPlatform, SmPlatformCreateUpdateDto>();

            // Promotion Mappings
            CreateMap<Promotion, PromotionDto>()
                .ForMember(dest => dest.Date,
                    opt => opt.MapFrom(src => src.Date.ToString("dd/MM/yyyy")));
            CreateMap<PromotionDto, Promotion>();

            CreateMap<Promotion, PromotionSummaryDto>();
            CreateMap<PromotionSummaryDto, Promotion>();

            CreateMap<PromotionCreateUpdateDto, Promotion>();
            CreateMap<Promotion, PromotionCreateUpdateDto>();

            // Domain Mappings
            CreateMap<Domain, DomainDto>();
            CreateMap<DomainDto, Domain>();

            CreateMap<Domain, DomainSummaryDto>()
                .ForMember(dest => dest.BackLinksCount,
                    opt => opt.MapFrom(src => src.BackLinks.Count));
            CreateMap<DomainSummaryDto, Domain>();

            CreateMap<DomainCreateUpdateDto, Domain>();
            CreateMap<Domain, DomainCreateUpdateDto>();

            // BackLink Mappings
            CreateMap<BackLink, BackLinkDto>()
                .ForMember(dest => dest.LastUpdateDate,
                    opt => opt.MapFrom(src => src.LastUpdateDate.ToString("dd/MM/yyyy")))
                .ForMember(dest => dest.ManualCheckDate,
                    opt => opt.MapFrom(src => src.ManualCheckDate.ToString("dd/MM/yyyy")));
            CreateMap<BackLinkDto, BackLink>();

            CreateMap<BackLink, BackLinkSummaryDto>();
            CreateMap<BackLinkSummaryDto, BackLink>();

            CreateMap<BackLinkCreateUpdateDto, BackLink>();
            CreateMap<BackLink, BackLinkCreateUpdateDto>();

            // KeywordInfo Mappings
            CreateMap<KeywordInfo, KeywordInfoDto>();
            CreateMap<KeywordInfoDto, KeywordInfo>();

            CreateMap<KeywordInfo, KeywordInfoSummaryDto>();
            CreateMap<KeywordInfoSummaryDto, KeywordInfo>();

            CreateMap<KeywordInfoCreateUpdateDto, KeywordInfo>();
            CreateMap<KeywordInfo, KeywordInfoCreateUpdateDto>();

            // KeywordResponse Mappings
            CreateMap<KeywordResponse, KeywordResponseDto>()
                .ForMember(dest => dest.Date,
                    opt => opt.MapFrom(src => src.Date.ToString("dd/MM/yyyy")));
            CreateMap<KeywordResponseDto, KeywordResponse>();

            CreateMap<KeywordResponse, KeywordResponseSummaryDto>();
            CreateMap<KeywordResponseSummaryDto, KeywordResponse>();

            CreateMap<KeywordResponseCreateUpdateDto, KeywordResponse>();
            CreateMap<KeywordResponse, KeywordResponseCreateUpdateDto>();

            // KeywordValue Mappings
            CreateMap<KeywordValue, KeywordValueDto>()
                .ForMember(dest => dest.CreatedDate,
                    opt => opt.MapFrom(src => src.CreatedDate.ToString("dd/MM/yyyy")))
                .ForMember(dest => dest.UpdatedDate,
                    opt => opt.MapFrom(src => src.UpdatedDate.ToString("dd/MM/yyyy")));
            CreateMap<KeywordValueDto, KeywordValue>();

            CreateMap<KeywordValue, KeywordValueSummaryDto>();
            CreateMap<KeywordValueSummaryDto, KeywordValue>();

            CreateMap<KeywordValueCreateUpdateDto, KeywordValue>();
            CreateMap<KeywordValue, KeywordValueCreateUpdateDto>();

            // LogMessage Mappings
            CreateMap<LogMessage, LogMessageDto>();
            CreateMap<LogMessageDto, LogMessage>();

            CreateMap<LogMessageCreateDto, LogMessage>();
            CreateMap<LogMessage, LogMessageCreateDto>();

            // NewsSite Mappings
            CreateMap<NewsSite, NewsSiteDto>()
                .ForMember(dest => dest.LastUpdateDate,
                    opt => opt.MapFrom(src => src.LastUpdateDate.ToString("dd/MM/yyyy")));
            CreateMap<NewsSiteDto, NewsSite>();

            CreateMap<NewsSite, NewsSiteSummaryDto>();
            CreateMap<NewsSiteSummaryDto, NewsSite>();

            CreateMap<NewsSiteCreateUpdateDto, NewsSite>();
            CreateMap<NewsSite, NewsSiteCreateUpdateDto>();

            // UserInfo Mappings
            CreateMap<UserInfo, UserInfoDto>();
            CreateMap<UserInfoDto, UserInfo>();

            CreateMap<UserInfo, UserInfoSummaryDto>();
            CreateMap<UserInfoSummaryDto, UserInfo>();

            CreateMap<UserInfoCreateUpdateDto, UserInfo>();
            CreateMap<UserInfo, UserInfoCreateUpdateDto>();

            // ContentBudget Mappings
            CreateMap<ContentBudget, ContentBudgetSummaryDto>();
            CreateMap<ContentBudgetSummaryDto, ContentBudget>();
        }
    }
}