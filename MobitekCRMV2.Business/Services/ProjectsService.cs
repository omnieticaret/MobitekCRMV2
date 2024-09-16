using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using Mobitek.CRM.Models.KeywordModels;
using MobitekCRMV2.DataAccess.Context;
using MobitekCRMV2.Entity.Entities;
using MobitekCRMV2.Entity.Enums;
using MobitekCRMV2.Model.Models;

namespace MobitekCRMV2.Business.Services
{
    public class ProjectsService
    {
        private readonly CRMDbContext _context;
        private readonly CreateTodos _createTodos;

        public ProjectsService(CRMDbContext context, CreateTodos createTodos)
        {
            _context = context;
            _createTodos = createTodos;
        }

        public int GetTotalBudgetFromList(List<Project> projectList)
        {
            var totalBudget = projectList.Sum(project =>
            {
                if (project.Budget == null)
                    return 0;

                if (project.Budget.Contains("."))
                    project.Budget = project.Budget.Replace(".", "");
                if (project.Budget.Contains(","))
                    project.Budget = project.Budget.Replace(",", "");

                int.TryParse(project.Budget, out int budget);
                return budget;
            });
            return totalBudget;
        }
        public string CalculateDate(Project project)
        {
            string myString = "";
            var duration = DateTime.Now - project.StartDate;
            var totalDays = duration.TotalDays;
            var totalMonths = (totalDays / 30) + 1;
            var totalYears = totalMonths / 12;
            var months = (int)(totalMonths % 12);

            var years = (int)(totalYears);

            if (years > 0)
            {
                myString = myString + years + " yıl ";
            }
            if (months > 0)
            {
                myString = myString + months + " ay ";
            }
            return myString;
        }

        public string GetTotalContractKeywordCount(List<Project> projectList)
        {
            int count = 0;
            int contractCount = 0;
            projectList.ForEach(x =>
            {

                count += x.Keywords.Count();
                contractCount += x.ContractKeywordCount;
            });

            return count.ToString() + "/" + contractCount.ToString();
        }

        public async Task<Task> GetTargetUrlAsync(string KeywordId, Keyword keyword)
        {
            if (KeywordId != null)
            {


                var result = _createTodos.GetConnectionJsonAsync(keyword.TargetURL);
                keyword.TargetStatus = result;
                Console.WriteLine($"{keyword.TargetURL} :  {result}");
                var todoGroup = _context.TodoGroups.Where(x => x.Id == "aa002ad0-9a73-4ced-93d0-0b583b6cc387").FirstOrDefault();
                if (result != "OK")// add todo for 404,500 and error
                {
                    var hasValue = _context.Todos.Any(x => x.Response.Contains(keyword.Id) && x.Status != TodoStatus.Done);

                    if (!hasValue)
                    {

                        var newTodo = new Todo();
                        newTodo.Title = $"{keyword?.TargetStatus} | {keyword.TargetURL}";
                        newTodo.TodoGroupId = todoGroup?.Id;
                        newTodo.ProjectId = keyword?.Project?.Id;
                        newTodo.OwnerId = keyword?.Project?.ExpertId;
                        newTodo.Response = "KeywordId : " + keyword?.Id;
                        newTodo.Status = TodoStatus.New;
                        newTodo.DueDate = DateTime.Now.AddDays(todoGroup.TodoRange);
                        newTodo.Description = $"https://crmv2.mobitek.com/Projects/Detail/{keyword?.ProjectId}";
                        _context.Todos.Add(newTodo);

                    }
                }
                else
                {
                    var hasValue = _context.Todos.Any(x => x.Response.Contains(keyword.Id) && x.Status != TodoStatus.Done);
                    if (hasValue)
                    {
                        var todo = _context.Todos.Where(x => x.Response.Contains(keyword.Id) && x.Status != TodoStatus.Done).FirstOrDefault();
                        todo.Status = TodoStatus.Done;

                    }
                }
            }
            return Task.CompletedTask;

        }

        public async Task<List<KeywordValue>> GetFilteredValuesByDate(string SelectedDate, string KeywordId)
        {
            var endString = "";
            var startString = "";
            var values = new List<KeywordValue>();
            if (SelectedDate != null)//selected date varsa seçilen aralığı döndürür
            {

                if (SelectedDate.Contains("ay"))
                {
                    DateTime today = DateTime.Today.AddHours(23);

                    DateTime MonthAgo = new DateTime();
                    if (SelectedDate == "1ay")
                    {
                        MonthAgo = today.AddMonths(-1);
                    }
                    else if (SelectedDate == "3ay")
                    {
                        MonthAgo = today.AddMonths(-3);
                    }
                    else if (SelectedDate == "6ay")
                    {
                        MonthAgo = today.AddMonths(-6);
                    }
                    else if (SelectedDate == "12ay")
                    {
                        MonthAgo = today.AddMonths(-12);
                    }
                    values = await _context.KeywordValues
                   .Where(x => x.KeywordId == KeywordId && x.CreatedAt >= MonthAgo && x.CreatedAt <= today).OrderByDescending(x => x.CreatedAt).ToListAsync();

                    return values;
                }
                else
                {

                    startString = SelectedDate.Split("to")[0].Trim();

                    try
                    {
                        endString = SelectedDate.Split("to")[1].Trim();
                    }
                    catch
                    {
                        endString = startString;
                    }

                    DateTime startDate = new DateTime(int.Parse(startString.Split("/")[2]), int.Parse(startString.Split("/")[0]), int.Parse(startString.Split("/")[1]));
                    DateTime endDate = new DateTime(int.Parse(endString.Split("/")[2]), int.Parse(endString.Split("/")[0]), int.Parse(endString.Split("/")[1]), 23, 59, 59);


                    values = await _context.KeywordValues
                   .Where(x => x.KeywordId == KeywordId && x.CreatedAt >= startDate && x.CreatedAt <= endDate)
                   .OrderByDescending(x => x.CreatedDate).ToListAsync();

                    return values;

                }

            }
            return values;
        }

        public List<DateTime> GetDateTimeListFromPicker(string SelectedDate)
        {
            var endString = "";
            var startString = "";
            List<DateTime> dateList = new List<DateTime>();
            if (SelectedDate != null)
            {
                if (SelectedDate.Contains("ay"))
                {
                    DateTime today = DateTime.Today.AddHours(23);

                    DateTime MonthAgo = new DateTime();
                    if (SelectedDate == "1ay")
                    {
                        MonthAgo = today.AddMonths(-1);
                    }
                    else if (SelectedDate == "3ay")
                    {
                        MonthAgo = today.AddMonths(-3);
                    }
                    else if (SelectedDate == "6ay")
                    {
                        MonthAgo = today.AddMonths(-6);
                    }
                    else if (SelectedDate == "12ay")
                    {
                        MonthAgo = today.AddMonths(-12);
                    }
                    dateList.Add(MonthAgo);
                    dateList.Add(DateTime.Today.AddHours(23));
                }
                else
                {

                    startString = SelectedDate.Split("to")[0].Trim();

                    try
                    {
                        endString = SelectedDate.Split("to")[1].Trim();
                    }
                    catch
                    {
                        endString = startString;
                    }

                    DateTime startDate = new DateTime(int.Parse(startString.Split("/")[2]), int.Parse(startString.Split("/")[0]), int.Parse(startString.Split("/")[1]));
                    DateTime endDate = new DateTime(int.Parse(endString.Split("/")[2]), int.Parse(endString.Split("/")[0]), int.Parse(endString.Split("/")[1]), 23, 59, 59);

                    dateList.Add(startDate);
                    dateList.Add(endDate);
                }
                return dateList;
            }
            return null;

        }

        public KeywordHistoryModel ConstructKeywordHistoryModel(List<KeywordValue> result, string CountryCode, DateTime? startDate = null, DateTime? endDate = null)
        {
            var model = new KeywordHistoryModel();

            // If start or end date is null, default to the last 30 days
            endDate ??= DateTime.Now;
            startDate ??= endDate.Value.AddDays(-30);

            // Calculate the difference in days between start and end dates
            var daysDiff = (int)(endDate.Value.Date - startDate.Value.Date).TotalDays;

            for (var i = 0; i <= daysDiff; i++)
            {
                var date = startDate.Value.AddDays(i);
                var averagePosition = result.Where(x => x.CreatedDate.Date == date.Date && x.CountryCode == CountryCode).DefaultIfEmpty().Average(x => x?.Position ?? model.HistoryList.LastOrDefault()?.Position ?? 0);

                var item = new HistoryAndPosition
                {
                    Position = (int)averagePosition,
                    Date = date.Date.ToString("dd/MM/yyyy")
                };

                model.HistoryList.Add(item);
            }

            return model;
        }

        public async Task<List<Keyword>> GetKeywordsByProjectId(string projectId, string IsStarredFilter)
        {
            var keywords = await _context.Keywords.Where(x => x.ProjectId == projectId).ToListAsync();

            if (IsStarredFilter == "star")
            {
                keywords = keywords.Where(x => x.IsStarred).ToList();
            }
            else if (IsStarredFilter == "unstar")
            {
                keywords = keywords.Where(x => !x.IsStarred).ToList();
            }

            return keywords;
        }

        public async Task<List<KeywordValue>> GetKeywordValuesWithinDateRange(List<Keyword> keywords, DateTime? startDate, DateTime? endDate, string CountryCode)
        {
            startDate ??= DateTime.Now.AddDays(-30);
            endDate ??= DateTime.Now;

            return await _context.KeywordValues
                .Where(x => x.CreatedDate >= startDate && x.CreatedDate <= endDate && keywords.Contains(x.Keyword) && x.CountryCode == CountryCode)
                .ToListAsync();
        }

        public string ListToString(List<string> countryCodes)
        {
            return string.Join(",", countryCodes);
        }

        public List<string> StringToList(string countryCodeString)
        {
            return countryCodeString.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();
        }


        public async Task<int> GetLastKeywordValuesAverage(Keyword keyword, string CountryCode)
        {
            var startDate = DateTime.Now.AddDays(-10);
            var endDate = DateTime.Now;

            var value = (int)_context.KeywordValues
                .Where(x => x.CreatedDate >= startDate && x.CreatedDate <= endDate && x.CountryCode == CountryCode && x.KeywordId == keyword.Id).Average(x => x.Position);

            return value;

        }

        public List<int> SplitExportPosition(string value)
        {
            var returnList = new List<int>();

            if (string.IsNullOrEmpty(value))
            {
                returnList.Add(0);
                returnList.Add(101);
                return returnList;
            }


            var trimmed = value.Trim();

            if (!trimmed.Contains("-"))
            {
                returnList.Add(0);
                returnList.Add(101);
                return returnList;
            }

            var splitList = trimmed.Split("-");

            returnList.Add(int.Parse(splitList[0]));
            returnList.Add(int.Parse(splitList[1]));

            return returnList;
        }

        public byte[] AllProjectKeywordsExportExcel(List<ExportKeywordItem> KeywordList)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Projeler Kelime Listesi");

                int RowCount = 1;
                foreach (var item in KeywordList)
                {
                    worksheet.Cell(RowCount, 1).Value = item.Name;
                    worksheet.Cell(RowCount, 2).Value = item.Url;
                    worksheet.Cell(RowCount, 3).Value = item.AveragePosition;
                    worksheet.Cell(RowCount, 4).Value = item.CountryCode;
                    RowCount++;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }

            }
        }


    }
}
