using MobitekCRMV2.Dto.Dtos.BackLinkDto;
using MobitekCRMV2.Dto.Dtos.ProjectsDto;
using MobitekCRMV2.Entity.Enums;

namespace MobitekCRMV2.Dto.Dtos.DomainDto
{
    public class DomainDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<BackLinkSummaryDto> BackLinks { get; set; }
        public ProjectSummaryDto Project { get; set; }
    }
    public class DomainListDto
    {
        public List<DomainSummaryDto> Domains { get; set; }
        public int TotalCount { get; set; }
    }

    public class DomainSummaryDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int BackLinksCount { get; set; } 
    }

    public class DomainCreateUpdateDto
    {
        public string Name { get; set; }
    }

    public class DomainFilterDto
    {
        public string Name { get; set; }
        public string ProjectId { get; set; }
        public bool? HasBackLinks { get; set; }
    }
}