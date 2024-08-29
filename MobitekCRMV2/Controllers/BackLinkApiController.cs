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
    [Route("api/[controller]")]
    [ApiController]
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
        [HttpGet("Index")]
        public async Task<IActionResult> Index(string id, string type)
        {
            var backlinkViewModelList = new List<BackLinkViewModel>();
            var Domain = await _domainRepository.Table.AsNoTracking().Include(x => x.Project).FirstOrDefaultAsync(x => x.Id == id);
            if (type == null)
            {
                if (id == null)
                {
                    return RedirectToAction("Index", "Domain");
                }
                var backlinkList = await _backLinkRepository.Table.AsNoTracking().Where(x => x.DomainId == id).ToListAsync();

                foreach (var item in backlinkList)
                {
                    var baclink = new BackLinkViewModel();
                    baclink.Id = item.Id;
                    baclink.Status = item.Status;
                    baclink.UrlFrom = item.UrlFrom;
                    baclink.LandingPage = item.LandingPage;
                    baclink.Anchor = item.Anchor;
                    baclink.Da = item.Da;
                    baclink.Pa = item.Pa;
                    baclink.SelectedDate = item.SelectDate;
                    backlinkViewModelList.Add(baclink);
                }
            }

            if (type != null)
            {
                var error = await _backLinkRepository.Table.AsNoTracking().Where(x => x.DomainId == id && x.Status != "OK").ToListAsync();

                foreach (var item in error)
                {
                    var baclink = new BackLinkViewModel();
                    baclink.Id = item.Id;
                    baclink.Status = item.Status;
                    baclink.UrlFrom = item.UrlFrom;
                    baclink.LandingPage = item.LandingPage;
                    baclink.Anchor = item.Anchor;
                    baclink.SelectedDate = item.SelectDate;
                    backlinkViewModelList.Add(baclink);
                }

            }

            TempData["DomainName"] = Domain.Name;
            TempData["ProjectId"] = Domain.Project.Id;

            foreach (var backlink in backlinkViewModelList)
            {
                if (backlink.SelectedDate != null)
                {
                    if (backlink.SelectedDate.Contains("."))
                    {
                        backlink.SelectedDate = _backlinksService.FixSelectDate(backlink.SelectedDate);
                    }
                }


            }

            return View(backlinkViewModelList);
        }
    }
}
