using MobitekCRMV2.Dto.Dtos.KeywordsInfoDto;
using MobitekCRMV2.Dto.Dtos.KeywordsResponseDto;
using MobitekCRMV2.Dto.Dtos.KeywordsValueDto;
using MobitekCRMV2.Dto.Dtos.ProjectsDto;
using MobitekCRMV2.Dto.Dtos.PromotionsDto;
using MobitekCRMV2.Entity.Enums;

namespace MobitekCRMV2.Dto.Dtos.KeywordsDto
{
    public class KeywordDto
    {
        public string Id { get; set; }
        public string KeywordName { get; set; }
        public string TargetURL { get; set; }
        public string TargetStatus { get; set; }
        public bool IsStarred { get; set; }
        public string MetaTitle { get; set; }
        public string MetaDescription { get; set; }
        public string H1 { get; set; }
        public string ProjectId { get; set; }
        public ProjectSummaryDto Project { get; set; }
        public List<KeywordResponseDto> KeywordResponses { get; set; }
        public List<KeywordValueDto> KeywordValues { get; set; }
        public KeywordInfoDto KeywordInfo { get; set; }
        public List<PromotionSummaryDto> Promotions { get; set; }
    }

    public class KeywordListDto
    {
        public List<KeywordSummaryDto> Keywords { get; set; }
        public int TotalCount { get; set; }
    }

    public class KeywordSummaryDto
    {
        public string Id { get; set; }
        public string KeywordName { get; set; }
        public string TargetURL { get; set; }
        public string TargetStatus { get; set; }
        public bool IsStarred { get; set; }
        //  public List<KeywordValueDto> KeywordValues { get; set; }
    }

    public class KeywordCreateUpdateDto
    {
        public string KeywordName { get; set; }
        public string TargetURL { get; set; }
        public string TargetStatus { get; set; }
        public bool IsStarred { get; set; }
        public string MetaTitle { get; set; }
        public string MetaDescription { get; set; }
        public string H1 { get; set; }
        public string ProjectId { get; set; }
    }
    public class KeywordSummaryDto2
    {
        public string Id { get; set; }
        public string KeywordName { get; set; }
        public string TargetURL { get; set; }
        public string TargetStatus { get; set; }
        public bool IsStarred { get; set; }
        public List<KeywordValueDto> KeywordValues { get; set; }
    }
    public class KeywordSummaryDto3
    {
        public string Id { get; set; }
        public List<KeywordValueDto> KeywordValues { get; set; }
    }

}