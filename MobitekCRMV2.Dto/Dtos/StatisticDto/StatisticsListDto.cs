using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobitekCRMV2.Dto.Dtos.StatisticDto
{
    public class StatisticsListDto
    {
        public FilterDto FilterModel { get; set; }
        public List<UserDto11> ExpertList { get; set; }
        public List<DataModelDto> DataList { get; set; } = new List<DataModelDto>(); 
    }

    public class FilterDto
    {
        public string ExpertId { get; set; }
        public string ExpertName { get; set; }
    }

    public class UserDto11
    {
        public string Id { get; set; }
        public string UserName { get; set; }
    }

    public class DataModelDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ExpertName { get; set; }
        public int KeywordCount { get; set; }
        public int Position { get; set; }
    }
}
