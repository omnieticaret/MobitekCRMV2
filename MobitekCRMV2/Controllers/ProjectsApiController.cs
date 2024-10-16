using DocumentFormat.OpenXml.Presentation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MobitekCRMV2.Authentication;
using MobitekCRMV2.Business.Services;
using MobitekCRMV2.DataAccess.Context;
using MobitekCRMV2.DataAccess.Repository;
using MobitekCRMV2.DataAccess.UoW;
using MobitekCRMV2.Dto.Dtos;
using MobitekCRMV2.Dto.Dtos.CustomerDto;
using MobitekCRMV2.Dto.Dtos.PlatformsDto;
using MobitekCRMV2.Dto.Dtos.ProjectDto;
using MobitekCRMV2.Dto.Dtos.StatisticDto;
using MobitekCRMV2.Dto.Dtos.UserDto;
using MobitekCRMV2.Entity.Entities;
using MobitekCRMV2.Entity.Enums;
using MobitekCRMV2.Extensions;
using MobitekCRMV2.Jobs;
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


        public ProjectsApiController(IRepository<Project> projectRepository, IRepository<Keyword> keywordRepository,
            IRepository<Platform> platformRepository, IUnitOfWork unitOfWork, UserManager<User> userManager, IRepository<Customer> customerRepository,
            IRepository<BackLink> backlinkrepository, IRepository<KeywordInfo> keywordInfoRepository, IRepository<KeywordValue> keywordvalue,
            SpaceSerpJob spaceSerpJob, CreateTodos createTodos, CRMDbContext context, ProjectsService projectsService, BacklinksService backlinksService, HttpClient httpClient = null, IConfiguration configuration = null, TokenHelper tokenHelper = null)
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

            // Roller bazında sorgu filtreleme
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

            List<ProjectListDto> viewModel = new List<ProjectListDto>();
            foreach (var project in projects)
            {
                var projectDto = new ProjectListDto
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
                    .Select(u => new UserDto { Id = u.Id, UserName = u.UserName })
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
        public async Task<ActionResult<ProjectDetailDto>> GetProjectDetail(string id, string? returnType = null, string? type = null, string? starFilter = null, string? countryCode = null)
        {
            var userName = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
            var userRole = HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == userName);

            //if (string.IsNullOrEmpty(starFilter) && string.IsNullOrEmpty(countryCode))
            //{

            //    HttpContext.Session.Remove("StarFilter");
            //    HttpContext.Session.Remove("CountryCode");
            //}
            //else
            //{
            //    if (!string.IsNullOrEmpty(starFilter)) HttpContext.Session.SetString("starFilter", starFilter);
            //    else starFilter = HttpContext.Session.GetString("StarFilter");

            //    if (!string.IsNullOrEmpty(countryCode)) HttpContext.Session.SetString("countryCode", countryCode);
            //    else countryCode = HttpContext.Session.GetString("CountryCode");
            //}

            var project = await _projectRepository.Table.AsNoTracking().IncludeAll().FirstOrDefaultAsync(x => x.Id == id);
            if (id == null || project == null)
            {
                return NotFound("Project not found");
            }

            var model = new ProjectDetailDto();

            if (type == "backlink")
            {
                model.BackLinks = _backlinkRepository.Table
                    .AsNoTracking()
                    .Where(x => x.Domain.Project.Id == id)
                    .OrderByDescending(x => x.CreatedAt)
                    .Take(20)
                    .Select(bl => new BackLinkDto
                    {
                        Id = bl.Id,
                    })
                    .ToList();
            }

            if (project.ProjectType == ProjectType.Seo)
            {
                model.DomainId = (await _context.Domains.FirstOrDefaultAsync(x => x.Project.Id == id))?.Id;
            }
            model.Users = await _userManager.Users.Select(u => new UserListDto
            {
                Id = u.Id,
                UserName = u.UserName,
                Email = u.Email
            }).ToListAsync();

            model.Platforms = await _platformRepository.Table.AsNoTracking().Select(p => new PlatformsListDto
            {
                Id = p.Id,
                Name = p.Name
            }).ToListAsync();

            model.Customers = await _customerRepository.Table.AsNoTracking().Select(c => new CustomerListDto
            {
                Id = c.Id
            }).ToListAsync();


            if (countryCode == null)
            {
                var codeList = _projectsService.StringToList(project.CountryCode);
                countryCode = codeList.Count == 1 ? project.CountryCode : codeList[0];
            }

            model.CountryCodeFilter = countryCode;

            var keywords = await _keywordRepository.Table.AsNoTracking()
                .Where(x => x.ProjectId == id)
                .Include(x => x.KeywordValues.Where(kv => kv.CountryCode == countryCode))
                .ToListAsync();

            if (starFilter != null)
            {
                keywords = starFilter switch
                {
                    "star" => keywords.Where(x => x.IsStarred).ToList(),
                    "unstar" => keywords.Where(x => !x.IsStarred).ToList(),
                    _ => keywords
                };
                model.IsStarredFilter = starFilter;
            }

            model.CountryCodeList = _projectsService.StringToList(project.CountryCode);

            if (!(User.IsInRole("admin") || User.IsInRole("viewer")))
            {
                if (!User.IsInRole("sm_expert") || project.ProjectType != ProjectType.Sm)
                {
                    if (!(project.Expert?.UserName == User.Identity.Name) && !(User.IsInRole("customer") && project.Customer?.CustomerRepresentativeId == user.Id))
                    {
                        return Forbid("You can only view details of your own projects");
                    }
                }
            }
            model.Project = new ProjectListDto
            {
                Id = project.Id,
                ProjectType = project.ProjectType.ToString(),
                ExpertId = project.ExpertId,
                CustomerId = project.CustomerId,
                ReportMail = project.ReportMail,
                Phone = project.Phone,
                Budget = project.Budget,
                ContractKeywordCount = project.ContractKeywordCount,
                Contract = project.Contract.ToString(),
                StartDate = project.StartDate,
                ReportDate = project.ReportDate,
                MeetingDate = project.MeetingDate,
                PacketInfo = project.PacketInfo,
                DevelopmentStatus = project.DevelopmentStatus,
                PlatformId = project.PlatformId,
                ServerStatus = project.ServerStatus,
                CountryCode = project.CountryCode,
                AccessInfo = project.AccessInfo,
                Note = project.Note,
                Status = project.Status.ToString(),
            };

            model.Keywords = keywords.FirstOrDefault() != null ? new KeywordDto
            {
                Id = keywords.First().Id,
                Keyword = keywords.First().KeywordName,
                IsStarred = keywords.First().IsStarred,
                KeywordValues = keywords.First().KeywordValues.Select(kv => new KeywordValueDto
                {
                    Id = kv.Id,
                    CountryCode = kv.CountryCode,
                    Position = kv.Position,
                    Date = kv.CreatedDate.ToString("yyyy-MM-dd")
                }).ToList()
            } : null;

            if (project.Customer != null)
            {
                model.Customers = new List<CustomerListDto>
                {
                    new CustomerListDto
                    {
                        Id = project.Customer.Id,
                        CompanyName = project.Customer.CompanyName,
                        CompanyAddress = project.Customer.CompanyAddress,
                        CompanyEmail = project.Customer.CompanyEmail,
                        CompanyPhone = project.Customer.CompanyPhone,
                        CompanyOfficialWebsite = project.Customer.CompanyOfficialWebsite,
                        CustomerType = project.Customer.CustomerType.ToString(),
                        CustomerRepresentative = project.Customer.CustomerRepresentative != null
                            ? project.Customer.CustomerRepresentative.UserName
                            : "Unknown",
                                Projects = project.Customer.Projects?.ToString()
                            }
                        };
            }
            else
            {
                model.Customers = new List<CustomerListDto>();
            }

            return model;

        }
    }

}
