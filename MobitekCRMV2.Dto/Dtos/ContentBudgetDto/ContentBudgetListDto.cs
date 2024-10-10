using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobitekCRMV2.Dto.Dtos.ContentBudgetDto
{
    public class ContentBudgetListDto
    {
        public string Id { get; set; }
        public string ProjectId { get; set; }
        public string ProjectName { get; set; }
        public int ProjectTotalBudget { get; set; }
        public int ProjectBudget { get; set; }
        public decimal ProjectCurrentExpense { get; set; }
    }
}
