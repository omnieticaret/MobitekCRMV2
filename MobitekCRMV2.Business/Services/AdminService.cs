using MobitekCRMV2.DataAccess.Context;
using MobitekCRMV2.Entity.Enums;
using MobitekCRMV2.Model.Models;
using System.Text;
using System.Text.Json;

namespace MobitekCRMV2.Business.Services
{
    public class AdminService
    {
        private readonly CRMDbContext _context;
        private readonly HttpClient httpClient;

        public AdminService(CRMDbContext context)
        {
            _context = context;
            httpClient = new()
            {
                Timeout = TimeSpan.FromSeconds(20)
            };
        }


        public List<DateCountModel> GetRecordList(int day)
        {
            var endDate = DateTime.Today;
            var startDate = endDate.AddDays(-day);

            var keywordValueCounts = _context.KeywordValues
                .Where(kv => kv.CreatedDate.Date >= startDate && kv.CreatedDate.Date <= endDate)
                .GroupBy(kv => kv.CreatedDate.Date)
                .Select(group => new DateCountModel
                {
                    Date = group.Key,
                    Count = group.Count()
                }).OrderBy(x => x.Date)
                .ToList();

            return keywordValueCounts;
        }


        public UserType GetEnumFromName(string enumName)
        {
            if (Enum.TryParse(enumName, true, out UserType result))
            {
                return result;
            }
            throw new ArgumentException($"No UserType with name {enumName} found");
        }


        public bool SendErrorNotification()
        {
            string uri = "https://n8n.mobitek.org/webhook/discorditalarm";

            using StringContent jsonContent = new(
                JsonSerializer.Serialize(new
                {
                    content = "content",
                    title = "title",
                    description = "description"

                }),
            Encoding.UTF8,
                "application/json");
            try
            {
                using HttpResponseMessage response = httpClient.PostAsync(
                    uri,
                   jsonContent).Result;

                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }

            return true;
        }

        public List<KeyValueModel> GetTodoStatistics()
        {
            var list = new List<KeyValueModel>
            {
                new KeyValueModel { Key = "Tümü", Value = _context.Todos.Count().ToString() },
                new KeyValueModel { Key = "Sistem tarafından  ", Value = _context.Todos.Where(x=>x.Response!=null).Count().ToString() },
                new KeyValueModel { Key = "Manuel veya Template tarafından ", Value = _context.Todos.Where(x=>x.Response==null).Count().ToString() },
                new KeyValueModel { Key = "Aktif ", Value = _context.Todos.Where(x=>x.Status!=TodoStatus.Done).Count().ToString() },
                new KeyValueModel { Key = "Tamamlanan", Value = _context.Todos.Where(x=>x.Status==TodoStatus.Done).Count().ToString() },

            };

            return list;
        }




    }
}
