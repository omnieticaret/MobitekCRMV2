using AutoMapper;
using AutoMapper.QueryableExtensions;
using DocumentFormat.OpenXml.Presentation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mobitek.CRM.Models.KeywordModels;
using MobitekCRMV2.Authentication;
using MobitekCRMV2.Business.Services;
using MobitekCRMV2.DataAccess.Context;
using MobitekCRMV2.DataAccess.Repository;
using MobitekCRMV2.DataAccess.UoW;
using MobitekCRMV2.Dto.Dtos.BackLinskDto;
using MobitekCRMV2.Dto.Dtos.CustomersDto;
using MobitekCRMV2.Dto.Dtos.KeywordsDto;
using MobitekCRMV2.Dto.Dtos.KeywordsValueDto;
using MobitekCRMV2.Dto.Dtos.PlatformsDto;
using MobitekCRMV2.Dto.Dtos.ProjectsDto;
using MobitekCRMV2.Dto.Dtos.StatisticDto;
using MobitekCRMV2.Dto.Dtos.UsersDto;
using MobitekCRMV2.Entity.Entities;
using MobitekCRMV2.Entity.Enums;
using MobitekCRMV2.Extensions;
using MobitekCRMV2.Jobs;
using MobitekCRMV2.Model.Models;
using Newtonsoft.Json;
using System.Security.Claims;

namespace MobitekCRMV2.Controllers
{
    [Route("api/projects")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]

    public class ProjectsApiController : ControllerBase
    {
        #region Dependecy injection

        private readonly IRepository<Project> _projectRepository;
        private readonly IRepository<Keyword> _keywordRepository;
        private readonly IRepository<Platform> _platformRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<BackLink> _backlinkRepository;
        private readonly IRepository<KeywordInfo> _keywordInfoRepository;
        private readonly IRepository<KeywordValue> _keywordvaluesRepository;
        private readonly UserManager<User> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly SpaceSerpJob _spaceSerpJob;
        private readonly CreateTodos _createTodos;
        private readonly CRMDbContext _context;
        private readonly ProjectsService _projectsService;
        private readonly BacklinksService _backlinksService;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly TokenHelper _tokenHelper;
        private readonly IMapper _mapper;


        public ProjectsApiController(IRepository<Project> projectRepository, IRepository<Keyword> keywordRepository,
            IRepository<Platform> platformRepository, IUnitOfWork unitOfWork, UserManager<User> userManager, IRepository<Customer> customerRepository,
            IRepository<BackLink> backlinkrepository, IRepository<KeywordInfo> keywordInfoRepository, IRepository<KeywordValue> keywordvalue,
            SpaceSerpJob spaceSerpJob, CreateTodos createTodos, CRMDbContext context, ProjectsService projectsService, BacklinksService backlinksService, HttpClient httpClient = null, IConfiguration configuration = null, TokenHelper tokenHelper = null, IMapper mapper = null)
        {
            _projectRepository = projectRepository;
            _keywordRepository = keywordRepository;
            _platformRepository = platformRepository;
            _userManager = userManager;
            _backlinkRepository = backlinkrepository;
            _customerRepository = customerRepository;
            _unitOfWork = unitOfWork;
            _keywordInfoRepository = keywordInfoRepository;
            _keywordvaluesRepository = keywordvalue;
            _spaceSerpJob = spaceSerpJob;
            _createTodos = createTodos;
            _context = context;
            _projectsService = projectsService;
            _backlinksService = backlinksService;
            _httpClient = httpClient;
            _configuration = configuration;
            _tokenHelper = tokenHelper;
            _mapper = mapper;
        }
        #endregion

        [HttpGet("index")]
        public async Task<IActionResult> Index(ProjectType projectType, Status status, bool isAll)
        {
            var userName = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
            var userRole = HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;

            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == userName);

            IQueryable<Project> query = _projectRepository.Table.AsNoTracking()
                .Include(x => x.Expert)
                .Include(x => x.Keywords)
                .Include(x => x.Customer).ThenInclude(x => x.CustomerRepresentative);

            if (!(userRole == "admin" || userRole == "viewer"))
            {
                if (userRole == "customer")
                {
                    query = query.Where(x => x.Customer.CustomerRepresentativeId == user.Id);
                }
                else
                {
                    query = query.Where(x => x.Expert.UserName == userName);
                }
            }
            List<Project> projects;
            if (isAll)
            {
                projects = await query.ToListAsync();
            }
            else if (status == Status.Passive)
            {
                projects = await query.Where(p => p.Status == Status.Passive).ToListAsync();
            }
            else if (status == Status.Frozen)
            {
                projects = await query.Where(p => p.Status == Status.Frozen).ToListAsync();
            }
            else
            {
                projects = await query
                    .Where(p => p.Status == status && p.ProjectType == projectType)
                    .ToListAsync();
            }

            List<ProjectListDto2> viewModel = new List<ProjectListDto2>();
            foreach (var project in projects)
            {
                var projectDto = new ProjectListDto2
                {
                    Id = project.Id,
                    ProjectType = project.ProjectType.ToString(),
                    ExpertName = project.Expert?.UserName,
                    ExpertId = project.ExpertId,
                    Contracts = _projectsService.GetContractKeywordCount(project),
                    Url = project.Url,
                    Budget = project.Budget,
                    StartDate = project.StartDate,
                    ReportDate = project.ReportDate,
                    StatusUpdateDate = project.StatusUpdateDate,
                    ElapsedTime = _projectsService.CalculateDate(project)
                };
                viewModel.Add(projectDto);
            }

            return Content(JsonConvert.SerializeObject(viewModel), "application/json");
        }

        [HttpPost("createProject")]
        public async Task<IActionResult> CreateProject([FromBody] CreateProjectDto dto)
        {
            try
            {
                var userName = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
                var userRole = HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var currentUser = await _context.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity.Name);
                if (string.IsNullOrEmpty(dto.Url))
                {
                    return BadRequest("URL cannot be empty.");
                }

                if (string.IsNullOrEmpty(dto.ExpertId))
                {
                    dto.ExpertId = currentUser?.Id;
                }

                var project = new Project
                {
                    Url = dto.Url,
                    ProjectType = Enum.Parse<ProjectType>(dto.ProjectType),
                    ExpertId = dto.ExpertId,
                    CustomerId = dto.CustomerId,
                    CustomerTypeUserId = dto.CustomerTypeUserId,
                    ReportMail = dto.ReportMail,
                    Budget = dto.Budget,
                    ContractKeywordCount = dto.ContractKeywordCount ?? 0,
                    Contract = Enum.Parse<ContractType>(dto.ContractType),
                    ServerStatus = dto.ServerStatus,
                    StartDate = DateTime.Parse(dto.StartDate),
                    ReportDate = DateTime.Parse(dto.ReportDate),
                    MeetingDate = DateTime.Parse(dto.MeetingDate),
                    PacketInfo = dto.PacketInfo,
                    CountryCode = string.Join(",", dto.CountryCodeList),
                    DevelopmentStatus = dto.DevelopmentStatus,
                    PlatformId = dto.PlatformId,
                    AccessInfo = dto.AccessInfo,
                    Note = dto.Note
                };


                try
                {
                    var suffix = "";
                    if (project.Url.Contains("/en"))
                        suffix = "/en";
                    else if (project.Url.Contains("/ar"))
                        suffix = "/ar";
                    else if (project.Url.Contains("/tr"))
                        suffix = "/tr";
                    project.Url = Helper.GetAuthoritativeUrl(project.Url) + suffix;
                }
                catch
                {
                    return BadRequest("Invalid URL format. Please check and try again.");
                }

                if (project.ProjectType == ProjectType.Seo)
                {
                    var domain = new Domain { Name = project.Url };
                    project.Domain = domain;
                }

                if (dto.CountryCodeList == null || !dto.CountryCodeList.Any())
                {
                    project.CountryCode = "tr";
                }
                else
                {
                    project.CountryCode = string.Join(",", dto.CountryCodeList);
                }

                await _projectRepository.AddAsync(project);
                await _unitOfWork.CommitAsync();

                if (project.ProjectType != ProjectType.None)
                {
                    _createTodos.CreateTodosFromTemplates(project.Id);
                }

                return Ok("Proje başarıyla eklendi");
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        [HttpGet("getStatistics")]
        public async Task<ActionResult<StatisticsListDto>> GetStatistics(string? expertId = null)
        {
            var userName = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
            var userRole = HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;
            var authUser = await _context.Users.FirstOrDefaultAsync(x => x.UserName == userName);

            var statisticsDto = new StatisticsListDto
            {
                FilterModel = new FilterDto(),
                ExpertList = await _context.Users
                    .Where(x => x.Status == Status.Active && x.UserType == UserType.SeoExpert)
                    .Select(u => new UserDto11 { Id = u.Id, UserName = u.UserName })
                    .ToListAsync()
            };

            var projectQuery = _context.Projects
                .Include(x => x.Expert)
                .Where(x => x.Status == Status.Active && x.ProjectType == ProjectType.Seo);

            if (!User.IsInRole("admin"))
            {
                projectQuery = projectQuery.Where(x => x.ExpertId == authUser.Id);
            }

            if (!string.IsNullOrEmpty(expertId))
            {
                var selectedUser = await _context.Users.FirstOrDefaultAsync(x => x.Id == expertId);
                if (User.IsInRole("admin") || User.Identity.Name == selectedUser.UserName)
                {
                    projectQuery = projectQuery.Where(x => x.ExpertId == expertId);
                    statisticsDto.FilterModel.ExpertId = expertId;
                    statisticsDto.FilterModel.ExpertName = selectedUser.UserName;
                }
                else
                {
                    return Forbid("Erişim yetkiniz bulunmamaktadır");
                }
            }

            var projectList = await projectQuery.ToListAsync();

            if (projectList.Count > 0)
            {
                foreach (var project in projectList)
                {
                    var data = new DataModelDto
                    {
                        Id = project.Id,
                        Name = project.Url,
                        ExpertName = project.Expert?.UserName,
                        KeywordCount = await _context.Keywords.CountAsync(x => x.ProjectId == project.Id)
                    };

                    if (data.KeywordCount > 0)
                    {
                        var keywordValues = await _context.KeywordValues
                            .Include(x => x.Keyword)
                            .Where(x => x.CreatedDate == DateTime.Now.Date && x.Keyword.ProjectId == project.Id)
                            .ToListAsync();

                        data.Position = keywordValues.Any() ? (int)keywordValues.Average(x => x.Position) : 0;
                    }
                    else
                    {
                        data.Position = 0;
                    }

                    statisticsDto.DataList.Add(data);
                }
            }

            return Ok(statisticsDto);
        }

        [HttpGet("detail/{id}")]
        public async Task<ActionResult<ProjectDetailDto>> GetProjectDetail(
     string id,
     string? returnType = null,
     string? type = null,
     string? starFilter = null,
     string? countryCode = null)
        {
            var userName = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
            var userRole = HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;
            var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.UserName == userName);

            if (string.IsNullOrEmpty(starFilter) && string.IsNullOrEmpty(countryCode))
            {
                HttpContext.Session.Remove("StarFilter");
                HttpContext.Session.Remove("CountryCode");
            }
            else
            {
                starFilter ??= HttpContext.Session.GetString("StarFilter");
                countryCode ??= HttpContext.Session.GetString("CountryCode");
                if (starFilter != null) HttpContext.Session.SetString("StarFilter", starFilter);
                if (countryCode != null) HttpContext.Session.SetString("CountryCode", countryCode);
            }

            var project = await _projectRepository.Table.AsNoTracking()
                .Include(x => x.Expert)
                .Include(x => x.Customer)
                .Include(x => x.Platform)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (project == null) return NotFound(new { ErrorMessage = "Project not found" });

            var projectDto = _mapper.Map<ProjectDto>(project);
            projectDto.ReturnType = returnType;


            //var backlinks = await _backlinkRepository.Table.AsNoTracking()
            //                        .Where(x => x.Domain.Project.Id == id)
            //                        .OrderByDescending(x => x.CreatedAt)
            //                        .ToListAsync();
            //projectDto.BackLinks = _mapper.Map<List<BackLinkDto>>(backlinks);


            if (project.ProjectType == ProjectType.Seo)
                projectDto.DomainId = await _context.Domains
                                                    .Where(x => x.Project.Id == id)
                                                    .Select(x => x.Id)
                                                    .FirstOrDefaultAsync();

            projectDto.Users = _mapper.Map<List<UserDto>>(
                await _userManager.Users.Where(u => u.Id == project.ExpertId).ToListAsync());

            projectDto.Customers = _mapper.Map<List<CustomerDto>>(
                await _customerRepository.Table.AsNoTracking().Where(c => c.Id == project.CustomerId).ToListAsync());

            projectDto.Platforms = _mapper.Map<List<PlatformDto>>(
                await _platformRepository.Table.AsNoTracking().Where(p => p.Id == project.PlatformId).ToListAsync());

            if (string.IsNullOrEmpty(countryCode))
            {
                var codeList = _projectsService.StringToList(project.CountryCode);
                countryCode = codeList.Count == 1 ? project.CountryCode : codeList[0];
            }
            projectDto.CountryCodeFilter = countryCode;

            var keywords = await _keywordRepository.Table.AsNoTracking()
                           .Where(x => x.ProjectId == id)
                           .Include(x => x.KeywordValues.Where(kv => kv.CountryCode == countryCode))
                           .ToListAsync();


            var keywordDtos = _mapper.Map<List<KeywordDto>>(keywords);


            if (!string.IsNullOrEmpty(starFilter))
            {
                keywordDtos = starFilter == "star"
                                ? keywordDtos.Where(x => x.IsStarred).ToList()
                                : keywordDtos.Where(x => !x.IsStarred).ToList();
                projectDto.IsStarredFilter = starFilter;
            }

            var keywordSummaryDtos = _mapper.Map<List<KeywordSummaryDto>>(keywordDtos);

            foreach (var keywordDto in keywordDtos)
            {
                var keywordValues = keywords.FirstOrDefault(k => k.Id == keywordDto.Id)?.KeywordValues;
                if (keywordValues != null)
                {
                    keywordDto.KeywordValues = _mapper.Map<List<KeywordValueDto>>(keywordValues);
                }
            }

            projectDto.Keywords = keywordSummaryDtos;

            projectDto.CountryCodeList = _projectsService.StringToList(project.CountryCode);

            if (!(User.IsInRole("admin") || User.IsInRole("viewer")) &&
                (!User.IsInRole("sm_expert") || project.ProjectType != ProjectType.Sm) &&
                !(project.Expert?.UserName == User.Identity.Name) &&
                !(User.IsInRole("customer") && project.Customer?.CustomerRepresentativeId == user.Id))
            {
                return Unauthorized(new { ErrorMessage = "You can only view your own projects' details" });
            }

            return Ok(projectDto);
        }

        [HttpGet("dashboard")]
        public async Task<ActionResult<DashboardProjectsDto>> GetDashboardProjects()
        {
            var userName = User.Identity.Name;
            var isAdmin = User.IsInRole("admin");

            var dueThisWeekQuery = _projectRepository.Table
                .AsNoTracking()
                .Include(x => x.Expert)
                .Where(x => x.ReportDate < DateTime.Today.AddDays(7)
                    && x.ReportDate >= DateTime.Today
                    && x.ProjectType == ProjectType.Seo
                    && x.Status == Status.Active);

            var pastProjectsQuery = _projectRepository.Table
                .AsNoTracking()
                .Include(x => x.Expert)
                .Where(x => x.ReportDate < DateTime.Today
                    && x.ProjectType == ProjectType.Seo
                    && x.Status == Status.Active);

            if (!isAdmin)
            {
                dueThisWeekQuery = dueThisWeekQuery.Where(x => x.Expert.UserName == userName);
                pastProjectsQuery = pastProjectsQuery.Where(x => x.Expert.UserName == userName);
            }
            var dueThisWeekProjects = await dueThisWeekQuery
                .ProjectTo<ProjectSummaryDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            var pastProjects = await pastProjectsQuery
                .ProjectTo<ProjectSummaryDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            var dueThisWeekView = await dueThisWeekQuery
                .Where(x => x.Expert.UserName != userName)
                .ProjectTo<ProjectSummaryDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            var pastView = await pastProjectsQuery
                .Where(x => x.Expert.UserName != userName)
                .ProjectTo<ProjectSummaryDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            var response = new DashboardProjectsDto
            {
                ThisWeekProjects = dueThisWeekProjects,
                PastProjects = pastProjects,
                DueThisWeekView = dueThisWeekView,
                PastView = pastView
            };

            return Ok(response);
        }

        [HttpGet("getLast30DaysAverage/{id}")]
        public async Task<IActionResult> GetLast30DaysAverage(string id, string? isStarredFilter = null, string? selectedDate = null)
        {
            try
            {
                var countryCode = HttpContext.Session.GetString("CountryCode");
                if (countryCode == null)
                {
                    var project = await _context.Projects.FirstOrDefaultAsync(x => x.Id == id);
                    countryCode = project?.CountryCode ?? string.Empty;
                    countryCode = _projectsService.StringToList(countryCode).FirstOrDefault();
                }
                var keywords = await _projectsService.GetKeywordsByProjectId(id, isStarredFilter);
                var dateRange = _projectsService.GetDateTimeListFromPicker(selectedDate);
                var result = new List<KeywordValue>();

                if (dateRange != null)
                {
                    result = await _projectsService.GetKeywordValuesWithinDateRange(keywords, dateRange[0], dateRange[1], countryCode);
                }
                else
                {
                    result = await _projectsService.GetKeywordValuesWithinDateRange(keywords, null, null, countryCode);
                }
                var model = _projectsService.ConstructKeywordHistoryModel(result, countryCode, dateRange?[0], dateRange?[1]);
                var modelDto = _mapper.Map<KeywordHistoryModel>(model);

                return Ok(modelDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while processing your request.", Error = ex.Message });
            }
        }

        [HttpGet("getKeywordPositionHistory/{id}")]
        public async Task<IActionResult> GetKeywordPositionHistory(string id, string? isStarredFilter = null, string? selectedDate = null)
        {
            try
            {
                var countryCode = HttpContext.Session.GetString("CountryCode");
                if (countryCode == null)
                {
                    var project = await _context.Projects.FirstOrDefaultAsync(x => x.Id == id);
                    countryCode = project?.CountryCode ?? string.Empty;
                    countryCode = _projectsService.StringToList(countryCode).FirstOrDefault();
                }

                var keywords = await _projectsService.GetKeywordsByProjectId(id, isStarredFilter);
                var dateRange = _projectsService.GetDateTimeListFromPicker(selectedDate);
                var startDate = dateRange?[0] ?? DateTime.Now.AddDays(-30);
                var endDate = dateRange?[1] ?? DateTime.Now;


                var daysDiff = (int)(endDate.Date - startDate.Date).TotalDays;
                var allDates = Enumerable.Range(0, daysDiff + 1)
                    .Select(i => startDate.AddDays(i))
                    .ToList();

                var result = await _projectsService.GetKeywordValuesWithinDateRange(keywords, startDate, endDate, countryCode);

                var groupedResults = result
                    .Where(x => x.CountryCode == countryCode)
                    .GroupBy(x => x.CreatedDate.Date)
                    .ToDictionary(g => g.Key, g => g.ToList());

                var historyList = new List<object>();
                double lastKnownPosition = 0;

                foreach (var date in allDates)
                {
                    var dailyResults = groupedResults.ContainsKey(date.Date) ? groupedResults[date.Date] : null;
                    var averagePosition = dailyResults?.Average(x => x.Position) ?? lastKnownPosition;

                    historyList.Add(new
                    {
                        Date = date.ToString("dd/MM/yyyy"),
                        Positions = new
                        {
                            Poz1 = dailyResults?.Count(x => x.Position == 1) ?? 0,
                            Poz2_3 = dailyResults?.Count(x => x.Position >= 2 && x.Position <= 3) ?? 0,
                            Poz4_10 = dailyResults?.Count(x => x.Position >= 4 && x.Position <= 10) ?? 0,
                            Poz11_30 = dailyResults?.Count(x => x.Position >= 11 && x.Position <= 30) ?? 0,
                            Poz31_100 = dailyResults?.Count(x => x.Position >= 31 && x.Position <= 100) ?? 0,
                            Poz100plus = dailyResults?.Count(x => x.Position > 100) ?? 0
                        }
                    });

                    lastKnownPosition = averagePosition;
                }

                return Ok(new { history = historyList });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while processing your request.", Error = ex.Message });
            }
        }





        [HttpGet("exportAll")]
        public async Task<IActionResult> ExportAllProjectKeywords()
        {
            try
            {
                var keywords = await _context.Keywords.AsNoTracking()
              .Include(x => x.Project)
              .Where(x => x.IsStarred
                          && x.Project.ProjectType == ProjectType.Seo
                          && x.Project.Status == Status.Active
                          && x.Project.CountryCode.Contains("tr"))
              .ToListAsync();

                var exportList = new List<ExportKeywordItem>();

                foreach (var item in keywords)
                {

                    var countryCodeList = _projectsService.StringToList(item.Project.CountryCode);
                    var result = await _projectsService.GetLastKeywordValuesAverage(item, countryCodeList[0]);
                    var range = _projectsService.SplitExportPosition(item.Project.ExportPosition);

                    if (range.Count == 2 && result >= range[0] && result <= range[1])
                    {
                        exportList.Add(new ExportKeywordItem
                        {
                            Name = item.KeywordName,
                            Url = item.TargetURL,
                            AveragePosition = result,
                            CountryCode = countryCodeList[0],
                        });
                    }
                }

                var content = _projectsService.AllProjectKeywordsExportExcel(exportList);

                return File(content,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    "AllKeywordList.xlsx");
            }
            catch (Exception ex)
            {

                throw;
            }

        }
    }
}
