using AutoMapper;
using MobitekCRMV2.Dto.Dtos.BackLinkDto;
using MobitekCRMV2.Dto.Dtos.CustomersDto;
using MobitekCRMV2.Dto.Dtos.DomainDto;
using MobitekCRMV2.Dto.Dtos.KeywordsDto;
using MobitekCRMV2.Dto.Dtos.KeywordsInfoDto;
using MobitekCRMV2.Dto.Dtos.KeywordsResponseDto;
using MobitekCRMV2.Dto.Dtos.KeywordsValueDto;
using MobitekCRMV2.Dto.Dtos.LogsMessageDto;
using MobitekCRMV2.Dto.Dtos.NewsSitesDto;
using MobitekCRMV2.Dto.Dtos.PlatformDto;
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
            CreateMap<Project, ProjectDto>()
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate.ToString("dd/MM/yyyy")))
                .ForMember(dest => dest.ReportDate, opt => opt.MapFrom(src => src.ReportDate.ToString("dd/MM/yyyy")))
                .ForMember(dest => dest.MeetingDate, opt => opt.MapFrom(src => src.MeetingDate.ToString("dd/MM/yyyy")))
                .ForMember(dest => dest.StatusUpdateDate, opt => opt.MapFrom(src => src.StatusUpdateDate.ToString("dd/MM/yyyy")))
                .ForMember(dest => dest.CompletedKeywordsCount, opt => opt.MapFrom(src => src.Keywords.Count(k => k.IsStarred)))
                .ForMember(dest => dest.TotalKeywordsCount, opt => opt.MapFrom(src => src.ContractKeywordCount))
                .ForMember(dest => dest.CompletionRate,
                    opt => opt.MapFrom(src => src.ContractKeywordCount > 0
                        ? (decimal)src.Keywords.Count(k => k.IsStarred) / src.ContractKeywordCount * 100
                        : 0));

            CreateMap<Project, ProjectSummaryDto>()
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.CompanyName))
                .ForMember(dest => dest.ExpertName, opt => opt.MapFrom(src => src.Expert.UserName))
                .ForMember(dest => dest.KeywordsCount, opt => opt.MapFrom(src => src.Keywords.Count))
                .ForMember(dest => dest.ReportDate, opt => opt.MapFrom(src => src.ReportDate.ToString("dd/MM/yyyy")));

            CreateMap<ProjectCreateUpdateDto, Project>().ReverseMap();

            CreateMap<Customer, CustomerDto>().ReverseMap();
            CreateMap<Customer, CustomerSummaryDto>().ReverseMap();
            CreateMap<CustomerCreateUpdateDto, Customer>().ReverseMap();

            CreateMap<Platform, PlatformDto>()
                .ForMember(dest => dest.ProjectCount, opt => opt.MapFrom(src => src.Projects.Count));
            CreateMap<Platform, PlatformSummaryDto>().ReverseMap();
            CreateMap<PlatformCreateUpdateDto, Platform>().ReverseMap();

            CreateMap<Keyword, KeywordDto>().ReverseMap();
            CreateMap<Keyword, KeywordSummaryDto>().ReverseMap();
            CreateMap<KeywordCreateUpdateDto, Keyword>().ReverseMap();

            CreateMap<SmPlatform, SmPlatformDto>().ReverseMap();
            CreateMap<SmPlatform, SmPlatformSummaryDto>().ReverseMap();
            CreateMap<SmPlatformCreateUpdateDto, SmPlatform>().ReverseMap();

            CreateMap<Promotion, PromotionDto>()
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date.ToString("dd/MM/yyyy")));
            CreateMap<Promotion, PromotionSummaryDto>().ReverseMap();
            CreateMap<PromotionCreateUpdateDto, Promotion>().ReverseMap();

            CreateMap<Domain, DomainDto>().ReverseMap();
            CreateMap<Domain, DomainSummaryDto>()
                .ForMember(dest => dest.BackLinksCount, opt => opt.MapFrom(src => src.BackLinks.Count));
            CreateMap<DomainCreateUpdateDto, Domain>().ReverseMap();

            CreateMap<BackLink, BackLinkDto>()
                .ForMember(dest => dest.LastUpdateDate, opt => opt.MapFrom(src => src.LastUpdateDate.ToString("dd/MM/yyyy")))
                .ForMember(dest => dest.ManualCheckDate, opt => opt.MapFrom(src => src.ManualCheckDate.ToString("dd/MM/yyyy")));
            CreateMap<BackLink, BackLinkSummaryDto>().ReverseMap();
            CreateMap<BackLinkCreateUpdateDto, BackLink>().ReverseMap();

            CreateMap<KeywordInfo, KeywordInfoDto>().ReverseMap();
            CreateMap<KeywordInfo, KeywordInfoSummaryDto>().ReverseMap();
            CreateMap<KeywordInfoCreateUpdateDto, KeywordInfo>().ReverseMap();

            CreateMap<KeywordResponse, KeywordResponseDto>()
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date.ToString("dd/MM/yyyy")));
            CreateMap<KeywordResponse, KeywordResponseSummaryDto>().ReverseMap();
            CreateMap<KeywordResponseCreateUpdateDto, KeywordResponse>().ReverseMap();

            CreateMap<KeywordValue, KeywordValueDto>()
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate.ToString("dd/MM/yyyy")))
                .ForMember(dest => dest.UpdatedDate, opt => opt.MapFrom(src => src.UpdatedDate.ToString("dd/MM/yyyy")));
            CreateMap<KeywordValue, KeywordValueSummaryDto>().ReverseMap();
            CreateMap<KeywordValueCreateUpdateDto, KeywordValue>().ReverseMap();

            CreateMap<LogMessage, LogMessageDto>().ReverseMap();
            CreateMap<LogMessageCreateDto, LogMessage>().ReverseMap();

            CreateMap<NewsSite, NewsSiteDto>()
                .ForMember(dest => dest.LastUpdateDate, opt => opt.MapFrom(src => src.LastUpdateDate.ToString("dd/MM/yyyy")));
            CreateMap<NewsSite, NewsSiteSummaryDto>().ReverseMap();
            CreateMap<NewsSiteCreateUpdateDto, NewsSite>().ReverseMap();

            CreateMap<UserInfo, UserInfoDto>().ReverseMap();
            CreateMap<UserInfo, UserInfoSummaryDto>().ReverseMap();
            CreateMap<UserInfoCreateUpdateDto, UserInfo>().ReverseMap();

            CreateMap<ProjectDto, Project>().ReverseMap();
            CreateMap<CustomerDto, Customer>().ReverseMap();
            CreateMap<PlatformDto, Platform>().ReverseMap();
            CreateMap<KeywordDto, Keyword>().ReverseMap();
            CreateMap<SmPlatformDto, SmPlatform>().ReverseMap();
            CreateMap<PromotionDto, Promotion>().ReverseMap();
            CreateMap<DomainDto, Domain>().ReverseMap();
            CreateMap<BackLinkDto, BackLink>().ReverseMap();
            CreateMap<KeywordInfoDto, KeywordInfo>().ReverseMap();
            CreateMap<KeywordResponseDto, KeywordResponse>().ReverseMap();
            CreateMap<KeywordValueDto, KeywordValue>().ReverseMap();
            CreateMap<LogMessageDto, LogMessage>().ReverseMap();
            CreateMap<NewsSiteDto, NewsSite>().ReverseMap();
            CreateMap<UserInfoDto, UserInfo>().ReverseMap();

            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<UserDto, UserSummaryDto>()
           .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
           .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
           .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
           .ForMember(dest => dest.UserType, opt => opt.MapFrom(src => src.UserType))
           .ForMember(dest => dest.Department, opt => opt.MapFrom(src => src.Department))
           .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status));
            CreateMap<UserCreateUpdateDto, User>().ReverseMap();
        }
    }
}