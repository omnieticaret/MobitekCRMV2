using MobitekCRMV2.Dto.Dtos.ProjectsDto;
using MobitekCRMV2.Entity.Enums;

namespace MobitekCRMV2.Dto.Dtos.SmPlatformsDto
{
    public class SmPlatformDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Link { get; set; }
        public int PostCount { get; set; }
        public string Note { get; set; }
        public string ProjectId { get; set; }
        public ProjectSummaryDto Project { get; set; }
    }

    public class SmPlatformListDto
    {
        public List<SmPlatformSummaryDto> SmPlatforms { get; set; }
        public int TotalCount { get; set; }
        public Dictionary<string, int> PlatformDistribution { get; set; }
    }

    public class SmPlatformSummaryDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Link { get; set; }
        public int PostCount { get; set; }
    }
    public class SmPlatformCreateUpdateDto
    {
        public string Name { get; set; }
        public string Link { get; set; }
        public int PostCount { get; set; }
        public string Note { get; set; }
        public string ProjectId { get; set; }
    }
    public class SmPlatformFilterDto
    {
        public string Name { get; set; }
        public string ProjectId { get; set; }
        public int? MinPostCount { get; set; }
        public int? MaxPostCount { get; set; }
    }
    public class SmPlatformAnalysisDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Link { get; set; }
        public SmPlatformMetricsDto Metrics { get; set; }
        public List<PostHistoryDto> PostHistory { get; set; }
    }
    public class SmPlatformMetricsDto
    {
        public int TotalPosts { get; set; }
        public double AveragePostsPerMonth { get; set; }
        public int PostsLastMonth { get; set; }
        public int PostsThisMonth { get; set; }
        public double GrowthRate { get; set; } 
    }
    public class PostHistoryDto
    {
        public string Date { get; set; }
        public int PostCount { get; set; }
    }
}