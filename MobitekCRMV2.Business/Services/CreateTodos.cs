using Microsoft.EntityFrameworkCore;
using MobitekCRMV2.DataAccess.Context;
using MobitekCRMV2.Entity.Entities;
using MobitekCRMV2.Entity.Enums;

namespace MobitekCRMV2.Business.Services
{
    public class CreateTodos
    {
        private readonly CRMDbContext _context;


        public CreateTodos(CRMDbContext context)
        {
            _context = context;
        }



        /// <summary>
        /// Proje oluştuğunda projenin tipi ile uyuşan todoGrupları çeker ve altlarındaki templateler ile ilgili projeye ait todolar oluşturur.
        /// </summary>
        public bool CreateTodosFromTemplates(string ProjectId)
        {
            var project = _context.Projects.Include(x => x.Expert).FirstOrDefault(x => x.Id == ProjectId);
            if (project == null)
            {
                return false;
            }
            var todoGroups = _context.TodoGroups.Include(x => x.TemplateItems)
                .Where(x => x.ProjectType == project.ProjectType && x.Status == Status.Active).ToList();

            foreach (var todoGroup in todoGroups)
            {

                foreach (var item in todoGroup.TemplateItems)
                {
                    var range = 0;
                    var newTodo = new Todo();
                    newTodo.Title = item.Title;
                    //newTodo.Description = item.Desc;
                    newTodo.OwnerId = project.ExpertId;
                    newTodo.TodoGroupId = todoGroup.Id;
                    newTodo.TemplateItemId = item.Id;
                    newTodo.ProjectId = project.Id;
                    newTodo.Status = TodoStatus.New;

                    if (item.TodoRange == 0)
                        range = todoGroup.TodoRange;
                    else
                        range = item.TodoRange;
                    newTodo.DueDate = DateTime.Now.Date.AddDays(range);

                    _context.Todos.Add(newTodo);

                }

            }

            _context.SaveChanges();

            return true;

        }


        /// <summary>
        /// Idsi gönderilen TodoGroupbun altındaki templateler ile proje tipi gönderilen projeler için todo açar . varsa ekleme yapmaz
        /// </summary>
        public async Task<string> CreateTodosFromTemplatesToAll(string TodoGroupId, ProjectType projectType)
        {
            if (TodoGroupId == null) return null;

            var projects = await _context.Projects.AsNoTracking().Where(x => x.ProjectType == projectType && x.Status == Status.Active).ToListAsync();
            var templates = await _context.TemplateItem.AsNoTracking().Where(x => x.TodoGroupId == TodoGroupId).ToListAsync();
            var todoGroup = await _context.TodoGroups.FindAsync(TodoGroupId);
            foreach (var project in projects)
            {
                foreach (var template in templates)
                {
                    if (!_context.Todos.Any(x => x.ProjectId == project.Id && x.TemplateItemId == template.Id))
                    {

                        var range = 0;
                        var newTodo = new Todo();
                        newTodo.Title = template.Title;
                        //newTodo.Description = template.Desc;
                        newTodo.OwnerId = project.ExpertId;
                        newTodo.TodoGroupId = todoGroup.Id;
                        newTodo.TemplateItemId = template.Id;
                        newTodo.ProjectId = project.Id;
                        newTodo.Status = TodoStatus.New;

                        if (template.TodoRange == 0)
                            range = todoGroup.TodoRange;
                        else
                            range = template.TodoRange;
                        newTodo.DueDate = DateTime.Now.Date.AddDays(range);

                        _context.Todos.Add(newTodo);

                    }
                }
            }

            _context.SaveChanges();

            return $"Seçili TodoGruba ait {templates.Count} adet Template {projects.Count} adet proje için Todo olarak eklendi";

        }


        /// <summary>
        /// Projenin sahibi değiştiğinde tamamlanmamış todoları yeni sahibine aktarır .
        /// </summary>
        public bool ChangeTodosOwner(string ProjectId, string NewOwnerId)
        {
            var project = _context.Projects.Where(x => x.Id == ProjectId).FirstOrDefault();
            var todos = _context.Todos.Where(x => x.ProjectId == ProjectId && x.Status != TodoStatus.Done).ToList();
            foreach (var todo in todos)
            {
                if (project.ExpertId == todo.OwnerId)
                {
                    todo.OwnerId = NewOwnerId;
                }

            }
            _context.SaveChanges();
            return true;
        }


        /// <summary>
        /// Backlink kontrolü sonrası hata todolarını oluşturur
        /// </summary>
        public bool TodosFromBacklinks(BackLink backLink, string result)
        {

            var todoGroup = _context.TodoGroups
               .Where(x => x.Id == "79d551fb-bf51-4b83-87d5-e61f3a89eabc").FirstOrDefault();

            if (backLink == null)
            {
                return false;
            }

            if (result != "OK" && result != "403")
            {
                var hasValue = _context.Todos.Any(x => x.Response.Contains(backLink.Id) && x.Status != TodoStatus.Done);

                if (!hasValue)
                {
                    var newTodo = new Todo();
                    newTodo.Title = $"{backLink.Status} | {backLink.UrlFrom}";
                    newTodo.TodoGroupId = todoGroup?.Id;
                    newTodo.ProjectId = backLink.Domain?.Project?.Id;
                    newTodo.OwnerId = backLink.Domain?.Project?.ExpertId;
                    newTodo.Response = "BacklinkId : " + backLink.Id;
                    newTodo.Status = TodoStatus.New;
                    if (todoGroup != null)
                        newTodo.DueDate = DateTime.Now.Date.AddDays(todoGroup.TodoRange);
                    else
                        newTodo.DueDate = DateTime.Now.AddDays(7);
                    newTodo.Description = $"https://crmv2.mobitek.com/BackLink/Index/{backLink.DomainId}?type=ERROR";
                    _context.Todos.Add(newTodo);
                }

            }
            else
            {
                var hasValue = _context.Todos.Any(x => x.Response.Contains(backLink.Id) && x.Status != TodoStatus.Done);
                if (hasValue)
                {
                    var todo = _context.Todos.Where(x => x.Response.Contains(backLink.Id) && x.Status != TodoStatus.Done).FirstOrDefault();
                    todo.Status = TodoStatus.Done;

                }

            }
            _context.SaveChanges();

            return true;
        }


        // KeywordTarget url kontrolü için
        public string GetConnectionJsonAsync(string url)
        {
            try
            {
                url = url.Trim();
                if (!url.Contains("https://"))
                {

                    if (!url.Contains("www."))
                    {
                        url = "www." + url;
                    }
                    url = "https://" + url;
                }

                using HttpClient httpClient = new()
                {
                    Timeout = TimeSpan.FromSeconds(20)
                };

                httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.3");


                var checkingResponse = httpClient.GetAsync(url).Result;
                if (checkingResponse.IsSuccessStatusCode)
                {

                    return checkingResponse.StatusCode.ToString();
                }
                else if (checkingResponse.StatusCode.ToString() == "NotFound")
                {
                    return "404";
                }
                else
                {
                    return "500";
                }

            }
            catch (TaskCanceledException Tcx)
            {
                Console.WriteLine($"Timeout Exception: {Tcx.ToString()}");
                return "ERROR";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.ToString()}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.ToString()}");
                }

                return "ERROR";
            }
        }

    }
}
