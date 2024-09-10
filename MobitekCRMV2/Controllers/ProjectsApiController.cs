using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MobitekCRMV2.Business.Services;
using MobitekCRMV2.DataAccess.Context;
using MobitekCRMV2.DataAccess.Repository;
using MobitekCRMV2.DataAccess.UoW;
using MobitekCRMV2.Entity.Entities;
using MobitekCRMV2.Entity.Enums;
using MobitekCRMV2.Jobs;

namespace MobitekCRMV2.Controllers
{
    [Route("api/projects")]
    [ApiController]
    [AllowAnonymous]

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

        
        public ProjectsApiController(IRepository<Project> projectRepository, IRepository<Keyword> keywordRepository,
            IRepository<Platform> platformRepository, IUnitOfWork unitOfWork, UserManager<User> userManager, IRepository<Customer> customerRepository,
            IRepository<BackLink> backlinkrepository, IRepository<KeywordInfo> keywordInfoRepository, IRepository<KeywordValue> keywordvalue,
            SpaceSerpJob spaceSerpJob, CreateTodos createTodos, CRMDbContext context, ProjectsService projectsService, BacklinksService backlinksService)
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
        }
        #endregion
        [HttpGet("index")]
        public async Task<IActionResult> Index(ProjectType projectType, Status status, bool isAll)
        {
            //var userName = User.Identity.Name;
            var userName = "adminMain";
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == userName);
            IQueryable<Project> query = _projectRepository.Table.AsNoTracking()
                .Include(x => x.Expert)
                .Include(x => x.Keywords)
                .Include(x => x.Customer).ThenInclude(x => x.CustomerRepresentative);

            if (!(User.IsInRole("admin") || User.IsInRole("viewer")))
            {
                if (User.IsInRole("customer"))
                {
                    query = query.Where(x => x.Customer.CustomerRepresentativeId == user.Id);
                }
                else if (User.IsInRole("sm_expert"))
                {
                    query = query.Where(x => x.ProjectType == projectType);
                }
                else
                {
                    query = query.Where(x => x.Expert.UserName == userName);
                }
            }

            List<Project> projects;
            string page = "";

            if (isAll)
            {
                projects = await query.ToListAsync();
            }
            else if (status == Status.Passive)
            {
                projects = await query.Where(p => p.Status == Status.Passive).ToListAsync();
                page = "Passive";
            }
            else if (status == Status.Frozen)
            {
                projects = await query.Where(p => p.Status == Status.Frozen).ToListAsync();
                page = "Frozen";
            }
            else
            {
                if (User.IsInRole("sm_expert"))
                {
                    projects = await query
                        .Where(p => p.Status == status && p.ProjectType == ProjectType.Sm).ToListAsync();
                }
                else
                {
                    projects = await query
                        .Where(p => p.Status == status && p.ProjectType == projectType)
                        .ToListAsync();
                }

                page = "ProjectType";
            }

            var totalBudget = _projectsService.GetTotalBudgetFromList(projects);
            var totalContractKeyword = _projectsService.GetTotalContractKeywordCount(projects);

            var viewModel = new ProjectsViewModel()
            {
                Projects = projects,
                ProjectType = projectType,
                Status = status,
                Page = page,
                TotalBudget = totalBudget,
                TotalContractKeyword = int.Parse(totalContractKeyword)
        };
            return Ok(viewModel);
        }
        public class ProjectsViewModel
        {
            public List<Project> Projects { get; set; }
            public ProjectType ProjectType { get; set; }
            public Status Status { get; set; }
            public string Page { get; set; }
            public decimal TotalBudget { get; set; }
            public int TotalContractKeyword { get; set; }
        }
    }
}
