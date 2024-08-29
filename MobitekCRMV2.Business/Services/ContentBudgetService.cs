using Microsoft.EntityFrameworkCore;
using MobitekCRMV2.DataAccess.Context;
using MobitekCRMV2.Entity.Entities;
using MobitekCRMV2.Entity.Enums;

namespace MobitekCRMV2.Business.Services
{
    public class ContentBudgetService
    {
        private readonly CRMDbContext _context;

        public ContentBudgetService(CRMDbContext context)
        {
            _context = context;
        }

        public async Task<List<Project>> GetSeoProjectsAsync()
        {
            return await _context.Projects.AsNoTracking()
                .Include(x => x.Expert)
                .Where(x => x.Status == Status.Active && x.ProjectType == ProjectType.Seo).ToListAsync();
        }

        public async Task<Project> GetProjectById(string id)
        {
            return await _context.Projects
                .Include(x => x.Expert)
                .Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        public int ConvertBudgetToInt(string budgetString)
        {
            if (string.IsNullOrEmpty(budgetString))
                return 0;

            string budgetWithoutSpaces = budgetString.Replace(" ", "");
            int budgetAsInt;

            if (int.TryParse(budgetWithoutSpaces, out budgetAsInt))
            {
                return budgetAsInt;
            }
            else
            {
                return 0;
            }
        }

        public int GetSpendingBudget(string budgetString, int percentage)
        {
            if (string.IsNullOrEmpty(budgetString))
                return 0;

            string budgetWithoutSpaces = budgetString.Replace(" ", "");
            int budgetAsInt;

            if (int.TryParse(budgetWithoutSpaces, out budgetAsInt))
            {
                return budgetAsInt * percentage / 100;
            }
            else
            {
                return 0;
            }
        }

        public string ConvertStringDateToSelectedDate(string dateString = null)
        {
            DateTime date;

            if (string.IsNullOrEmpty(dateString))
            {
                date = DateTime.Now;
            }
            else
            {
                date = DateTime.Parse(dateString);
            }

            string formattedDate = date.ToString("MM-yyyy");

            return formattedDate;
        }

        public async Task<List<ContentBudget>> GetContentBudgets(string ProjectId, string SelectedDate = null)
        {
            var date = ConvertStringDateToSelectedDate(SelectedDate);

            return await _context.ContentBudgets.AsNoTracking().Where(x => x.ProjectId == ProjectId && x.SelectedDate == date).OrderByDescending(x => x.CreatedAt).ToListAsync();
        }

        public async Task<int> GetMonthlyContentBudgetTotalCost(string ProjectId, string SelectedDate = null)
        {
            var date = ConvertStringDateToSelectedDate(SelectedDate);
            return await _context.ContentBudgets.AsNoTracking().Where(x => x.ProjectId == ProjectId && x.SelectedDate == date).SumAsync(x => x.Price);
        }

        public async Task<int> GetMonthlyBacklinkBudgetTotalCost(string ProjectId, string SelectedDate = null)
        {
            var date = ConvertStringDateToSelectedDate(SelectedDate);
            var total = 0;
            var backlinks = await _context.BackLinks
                .Include(x => x.Domain)
                .ThenInclude(x => x.Project)
                .Where(x => x.Domain.Project.Id == ProjectId && x.SelectDate == date).ToListAsync();

            foreach (var backlink in backlinks)
            {
                var site = await _context.NewsSites.Where(x => backlink.UrlFrom.ToLower().Contains(x.Name.ToLower())).FirstOrDefaultAsync();
                if (site != null)
                {
                    total += site.Price;
                }
            }

            return total;
        }

    }
}
