using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mobitek.CRM.Models.NewsSiteModels;
using MobitekCRMV2.Authentication;
using MobitekCRMV2.DataAccess.Repository;
using MobitekCRMV2.Dto.Dtos;
using MobitekCRMV2.Entity.Entities;
using System.Security.Claims;

namespace MobitekCRMV2.Controllers
{
    [Route("api/newssites")]
    [ApiController]
    public class NewsSitesApiController : ControllerBase
    {
        private readonly IRepository<NewsSite> _newsSiteRepository;

        public NewsSitesApiController(IRepository<NewsSite> newsSiteRepository)
        {
            _newsSiteRepository = newsSiteRepository;
        }

        [HttpGet("Index")]

        public async Task<ActionResult<List<NewsSiteDto2>>> Index()
        {
            var userName = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
            var userRole = HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;

            var newsSites = await _newsSiteRepository.Table.AsNoTracking()
                .Where(x => x.OrganicTraffic >= 1000 || x.OrganicTraffic == 0)
                .Include(x => x.User)
                .ToListAsync();

            var newsSiteDtos = newsSites.Select(x => new NewsSiteDto2
            {
                Id = x.Id,
                Name = x.Name,
                OrganicTraffic = x.OrganicTraffic,
                Pa = x.PAScore,
                Da = x.DAScore,
                EditorMail = x.EditorMail,

                //   UserName = x.User.UserName
            }).ToList();

            return Ok(newsSiteDtos);
        }
    }
}
