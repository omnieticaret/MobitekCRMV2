namespace MobitekCRMV2.Dto.Dtos.LogsMessageDto
{
    public class LogMessageDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string CreatedDate { get; set; }
    }
    public class LogMessageListDto
    {
        public List<LogMessageDto> LogMessages { get; set; }
        public int TotalCount { get; set; }
        public Dictionary<string, int> LogDistribution { get; set; }
    }

    public class LogMessageCreateDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class LogMessageFilterDto
    {
        public string Name { get; set; }
        public string SearchTerm { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class LogMessageStatsDto
    {
        public int TotalLogs { get; set; }
        public int TodayLogs { get; set; }
        public Dictionary<string, int> LogsByName { get; set; }
    }

}