using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MobitekCRMV2.Business.Services;
using MobitekCRMV2.DataAccess.Context;
using MobitekCRMV2.DataAccess.Repository;
using MobitekCRMV2.DataAccess.UoW;
using MobitekCRMV2.Entity.Entities;
using MobitekCRMV2.Extensions;
using MobitekCRMV2.Model.Models;

namespace MobitekCRMV2.Controllers
{
    [Route("api/backlink")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]

    public class BackLinkApiController : ControllerBase
    {
        private readonly IRepository<Domain> _domainRepository;
        private readonly IRepository<BackLink> _backLinkRepository;

        private readonly IUnitOfWork _unitOfWork;
        private readonly CustomReader _customReader;
        private readonly CreateTodos _createTodos;
        private readonly CRMDbContext _context;
        private readonly BacklinksService _backlinksService;

        public BackLinkApiController(IRepository<Domain> domainRepository, IRepository<BackLink> backLinkRepository, IUnitOfWork unitOfWork,
                        CustomReader customReader, CreateTodos createTodos, CRMDbContext context, BacklinksService backlinksService)
        {
            _domainRepository = domainRepository;
            _backLinkRepository = backLinkRepository;
            _unitOfWork = unitOfWork;
            _customReader = customReader;
            _createTodos = createTodos;
            _context = context;
            _backlinksService = backlinksService;
        }
        [HttpGet("index/{id}")]
        public async Task<IActionResult> Index(string? id = null, string? type = null)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest(new { message = "Id cannot be null or empty" });
            }

            var backlinkViewModelList = new List<BackLinkViewModel>();

            var domain = await _domainRepository.Table.AsNoTracking()
                .Include(x => x.Project)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (domain == null)
            {
                return NotFound(new { message = "Domain not found" });
            }

            if (string.IsNullOrEmpty(type))
            {
                var backlinkList = await _backLinkRepository.Table.AsNoTracking()
                    .Where(x => x.DomainId == id)
                    .ToListAsync();

                foreach (var item in backlinkList)
                {
                    backlinkViewModelList.Add(new BackLinkViewModel
                    {
                        Id = item.Id,
                        Status = item.Status,
                        UrlFrom = item.UrlFrom,
                        LandingPage = item.LandingPage,
                        Anchor = item.Anchor,
                        Da = item.Da,
                        Pa = item.Pa,
                        SelectedDate = item.SelectDate
                    });
                }
            }
            else
            {
                var errorList = await _backLinkRepository.Table.AsNoTracking()
                    .Where(x => x.DomainId == id && x.Status != "OK")
                    .ToListAsync();

                foreach (var item in errorList)
                {
                    backlinkViewModelList.Add(new BackLinkViewModel
                    {
                        Id = item.Id,
                        Status = item.Status,
                        UrlFrom = item.UrlFrom,
                        LandingPage = item.LandingPage,
                        Anchor = item.Anchor,
                        SelectedDate = item.SelectDate
                    });
                }
            }

            foreach (var backlink in backlinkViewModelList)
            {
                if (!string.IsNullOrEmpty(backlink.SelectedDate) && backlink.SelectedDate.Contains("."))
                {
                    backlink.SelectedDate = _backlinksService.FixSelectDate(backlink.SelectedDate);
                }
            }

            return Ok(new
            {
                DomainName = domain.Name,
                ProjectId = domain.Project.Id,
                Backlinks = backlinkViewModelList
            });
        }

        [HttpPost("Index")]
        public async Task<IActionResult> Index([FromBody] BacklinkRequestDTO request)
        {
            var backlinkViewModelList = new List<BackLinkViewModel>();
            var date = request.SelectedDate2;
            var year = date.ToString().Split(".")[2];
            year = year.Split(" ")[0];
            var month = date.ToString().Split(".")[1];
            var datetimes = month + "-" + year;

            List<BackLink> backlinks = new List<BackLink>();
            if (date.ToString() == "1.01.0001 00:00:00")
            {
                backlinks = await _backLinkRepository.Table.AsNoTracking()
                            .Where(x => x.Domain.Name == request.DomainName)
                            .ToListAsync();
            }
            else
            {
                backlinks = await _backLinkRepository.Table.AsNoTracking()
                            .Where(x => x.Domain.Name == request.DomainName &&
                                   (x.SelectDate == datetimes || x.SelectDate == date.ToString()))
                            .ToListAsync();
            }

            foreach (var item in backlinks)
            {
                var backlink = new BackLinkViewModel
                {
                    Id = item.Id,
                    Status = item.Status,
                    UrlFrom = item.UrlFrom,
                    LandingPage = item.LandingPage,
                    Anchor = item.Anchor,
                    Da = item.Da,
                    Pa = item.Pa
                };
                backlinkViewModelList.Add(backlink);
            }

            return new JsonResult(backlinkViewModelList);
        }
        public class BacklinkRequestDTO
        {
            public DateTime SelectedDate2 { get; set; }
            public string DomainName { get; set; }
        }
    }
}
