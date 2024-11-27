using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MobitekCRMV2.Business.Services;
using MobitekCRMV2.DataAccess.Context;
using MobitekCRMV2.DataAccess.Repository;
using MobitekCRMV2.DataAccess.UoW;
using MobitekCRMV2.Dto.Dtos.BackLinskDto;
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

        [HttpPost("addBacklinks")]
        public async Task<IActionResult> AddLinks([FromBody] BackLinkCreateUpdateDto dto)
        {
            if (string.IsNullOrEmpty(dto.SelectDate))
            {
                return BadRequest(new { Error = "Link eklerken tarih seçmelisiniz!" });
            }

            var domain = await _domainRepository.GetByAsync(x => x.Id == dto.DomainId);
            if (domain == null)
            {
                return NotFound(new { Error = "Domain bulunamadı!" });
            }

            var selectDate = "";
            var year = dto.SelectDate.Split("-")[0];
            var month = dto.SelectDate.Split("-")[1];
            selectDate = $"{month}-{year}";

            var backlinks = dto.UrlFrom.Select(url => new BackLink
            {
                UrlFrom = url,
                Domain = domain,
                SelectDate = selectDate
            }).ToList();


            foreach (var backlink in backlinks)
            {
                await _backLinkRepository.AddAsync(backlink);
            }

            await _unitOfWork.CommitAsync();

            return Ok(new { Message = "Links başarıyla eklendi!" });
        }

        [HttpPost("checkStatus")]
        public async Task<IActionResult> CheckStatus(string? id = null, string? returnUrl = null)
        {
            var backlink = await _backLinkRepository.Table
                .Include(x => x.Domain)
                .ThenInclude(x => x.Project)
                .Where(y => y.Id == id)
                .FirstOrDefaultAsync();

            if (backlink == null)
                return NotFound("Backlink not found");

            var result = _customReader.CheckBackLink(backlink.UrlFrom, backlink.Domain.Name);
            var domainId = backlink.DomainId;
            var result2 = await result;

            backlink.Status = result2.Status;
            backlink.LandingPage = result2.LandingPage;
            backlink.Anchor = result2.Anchor;
            backlink.LastUpdateDate = DateTime.Now.Date;
            _backLinkRepository.Update(backlink);
            await _unitOfWork.CommitAsync();

            _createTodos.TodosFromBacklinks(backlink, result2.Status);

            if (!string.IsNullOrEmpty(returnUrl))
                return Redirect(returnUrl);

         
      

            return Ok(new { Message = "Status updated successfully" });
        }

        [HttpPost("deleteSelectedItem")]
        public async Task<IActionResult> DeleteSelectedItems([FromBody] List<string> selectedIds)
        {
            if (selectedIds == null || selectedIds.Count == 0)
            {
                return BadRequest(new { message = "No items selected." });
            }

            var project = _context.Projects
                .Include(x => x.Expert)
                .Include(x => x.Domain).ThenInclude(x => x.BackLinks)
                .FirstOrDefault(x => x.Domain.BackLinks.Any(b => b.Id == selectedIds.FirstOrDefault()));

            if (project == null)
            {
                return NotFound(new { message = "Project not found." });
            }

            if (!User.IsInRole("admin"))
            {
                if (!(project.Expert?.UserName == User.Identity?.Name))
                {
                    return Forbid("You are only allowed to manage your own projects.");
                }
            }

            var backlinkForRoute = _backLinkRepository.Table
                .AsNoTracking()
                .Include(x => x.Domain)
                .FirstOrDefault(x => x.Id == selectedIds.First());

            if (backlinkForRoute == null)
            {
                return NotFound(new { message = "Backlink not found." });
            }

            try
            {
                foreach (var id in selectedIds)
                {
                    var backlink = await _backLinkRepository.GetByAsync(x => x.Id == id);
                    if (backlink != null)
                    {
                        _backLinkRepository.Remove(backlink);
                    }
                }
                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the items.", details = ex.Message });
            }

            return Ok(new { message = "Items deleted successfully.", domainId = backlinkForRoute.DomainId });
        }
    }
}
public class BackLinkCreateUpdateDto
{
    public List<string> UrlFrom { get; set; }
    public string SelectDate { get; set; }
    public string DomainId { get; set; }
}
public class BacklinkRequestDTO
{
    public DateTime SelectedDate2 { get; set; }
    public string DomainName { get; set; }
}
