using MobitekCRMV2.Dto.Dtos.ProjectsDto;
using MobitekCRMV2.Entity.Enums;

namespace MobitekCRMV2.Dto.Dtos.PlatformsDto
{
    public class PlatformDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<ProjectSummaryDto> Projects { get; set; }
        public int ProjectCount { get; set; }
    }

    public class PlatformListDto
    {
        public List<PlatformSummaryDto> Platforms { get; set; }
        public int TotalCount { get; set; }
    }
    public class PlatformSummaryDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int ProjectCount { get; set; }
    }
    public class PlatformCreateUpdateDto
    {
        public string Name { get; set; }
    }
    public class PlatformFilterDto
    {
        public string Name { get; set; }
        public bool? HasProjects { get; set; }
        public int? MinProjectCount { get; set; }
        public int? MaxProjectCount { get; set; }
    }

    public class PlatformStatsDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int TotalProjects { get; set; }
        public int ActiveProjects { get; set; }
        public Dictionary<ProjectType, int> ProjectTypeDistribution { get; set; }
        public Dictionary<Status, int> ProjectStatusDistribution { get; set; }
    }
}