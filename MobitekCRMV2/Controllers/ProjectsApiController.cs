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
using MobitekCRMV2.Dto.Dtos.ProjectDto;
using MobitekCRMV2.Entity.Entities;
using MobitekCRMV2.Entity.Enums;
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
    }
       
          

}
