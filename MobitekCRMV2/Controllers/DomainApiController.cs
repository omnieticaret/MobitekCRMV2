using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MobitekCRMV2.DataAccess.Repository;
using MobitekCRMV2.DataAccess.UoW;
using MobitekCRMV2.Entity.Entities;
using MobitekCRMV2.Entity.Enums;
using MobitekCRMV2.Dto.Dtos;
using Microsoft.EntityFrameworkCore;
using MobitekCRMV2.Business.Services;
using MobitekCRMV2.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace MobitekCRMV2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DomainApiController : ControllerBase
    {

        private readonly IRepository<Domain> _domainRepository;
        private readonly IRepository<BackLink> _backLinkRepository;
        private readonly IRepository<Project> _projectRepository;
        private readonly IRepository<NewsSite> _newsSiteRepository;
        private readonly UserManager<User> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly DomainService _domainService;
        private readonly TokenHelper _tokenHelper;
        private readonly IConfiguration _configuration;

        public DomainApiController(IRepository<Domain> domainRepository, IRepository<BackLink> backLinkRepository, UserManager<User> userManager, IUnitOfWork unitOfWork, IRepository<Project> projectRepository, IRepository<NewsSite> newsSiteRepository, DomainService domainService, TokenHelper tokenHelper, IConfiguration configuration)
        {
            _domainRepository = domainRepository;
            _backLinkRepository = backLinkRepository;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _projectRepository = projectRepository;
            _newsSiteRepository = newsSiteRepository;
            _domainService = domainService;
            _tokenHelper = tokenHelper;
            _configuration = configuration;
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet("index")]
        public async Task<ActionResult<List<DomainDto>>> Index([FromQuery] string projectStatus)
        {
            var authHeader = HttpContext.Request.Headers["Authorization"].ToString();
            var claimsPrincipal = _tokenHelper.ValidateToken(_configuration["Jwt:Key"], authHeader);

            if (claimsPrincipal == null)
            {
                return Unauthorized();
            }

            var userName = claimsPrincipal.FindFirst(ClaimTypes.Name)?.Value;
            var userRole = claimsPrincipal.FindFirst(ClaimTypes.Role)?.Value;
            var domains = new List<Domain>();

            if (!(User.IsInRole("admin") || User.IsInRole("viewer")))
            {
                domains = await _domainRepository.Table
                    .Include(x => x.Project)
                    .ThenInclude(x => x.Expert)
                    .Where(x => x.Project.Expert.UserName == User.Identity.Name)
                    .OrderByDescending(x => x.CreatedAt)
                    .ToListAsync();
            }
            else
            {
                domains = await _domainRepository.Table
                    .Include(x => x.Project)
                    .OrderByDescending(x => x.CreatedAt)
                    .ToListAsync();
            }

            domains = _domainService.FilterDomainsByStatus(domains, projectStatus);

            var domainDtos = domains.Select(domain => new DomainDto
            {
                Id = domain.Id,
                Name = domain.Name,
                CreatedAt = domain.CreatedAt,
                BacklinkCount = _domainService.GetDomainsBacklinkCount(domain.Id),
                TotalBacklink = _domainService.GetTotalBacklinkCount(domain.Id),
                ProjectStatus = domain.Project?.Status.ToString()
            }).ToList();

            return Ok(domainDtos);
        }

    }
}