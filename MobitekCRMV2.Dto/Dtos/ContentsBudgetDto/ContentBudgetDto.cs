using MobitekCRMV2.Dto.Dtos.ProjectsDto;

namespace MobitekCRMV2.Dto.Dtos.ContentsBudgetDto
{
    public class ContentBudgetDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public string SelectedDate { get; set; }

        public string ProjectId { get; set; }
        public ProjectSummaryDto Project { get; set; }
    }
    public class ContentBudgetListDto
    {
        public List<ContentBudgetSummaryDto> ContentBudgets { get; set; }
        public int TotalCount { get; set; }
    }
    public class ContentBudgetSummaryDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public string SelectedDate { get; set; }
    }

    public class ContentBudgetCreateUpdateDto
    {
        public string Name { get; set; }
        public int Price { get; set; }
        public string SelectedDate { get; set; }
        public string ProjectId { get; set; }
    }

}