using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MobitekCRMV2.Business.Services;
using MobitekCRMV2.DataAccess.Context;
using MobitekCRMV2.Dto.Dtos.ContentsBudgetDto;
using MobitekCRMV2.Dto.Dtos.CustomersDto;
using MobitekCRMV2.Entity.Entities;
using Newtonsoft.Json;
using System.Security.Claims;

namespace MobitekCRMV2.Controllers
{
    [Route("api/contentBudget")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class ContentBudgetApiController : ControllerBase
    {
        private readonly CRMDbContext _context;
        private readonly ContentBudgetService _contentBudgetService;

        public ContentBudgetApiController(CRMDbContext context, ContentBudgetService contentBudgetService)
        {
            _context = context;
            _contentBudgetService = contentBudgetService;
        }
        [HttpGet("getListContentBudget")]
        public async Task<IActionResult> GetListContentBudget()
        {
            var userName = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
            var userRole = HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;
            var projects = await _contentBudgetService.GetSeoProjectsAsync();

            if (!User.IsInRole("admin"))
            {
                projects = projects.Where(x => x.Expert?.UserName == userName).ToList();
            }

            List<ContentBudgetListDto2> viewModel = new List<ContentBudgetListDto2>();

            foreach (var project in projects)
            {
                var contentSpend = await _contentBudgetService.GetMonthlyContentBudgetTotalCost(project.Id);
                var backlinkSpend = await _contentBudgetService.GetMonthlyBacklinkBudgetTotalCost(project.Id);

                var contentBugdetListdto = new ContentBudgetListDto2
                {
                    ProjectId = project.Id,
                    ProjectName = project.Url,
                    ProjectTotalBudget = _contentBudgetService.ConvertBudgetToInt(project.Budget),
                    ProjectBudget = _contentBudgetService.GetSpendingBudget(project.Budget, 30),
                    ProjectCurrentExpense = contentSpend + backlinkSpend
                };
                viewModel.Add(contentBugdetListdto);
            }

            return Content(JsonConvert.SerializeObject(viewModel), "application/json");
        }
    }
}