using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mobitek.CRM.Models.NewsSiteModels;
using MobitekCRMV2.Authentication;
using MobitekCRMV2.DataAccess.Repository;
using MobitekCRMV2.Dto.Dtos;
using MobitekCRMV2.Dto.Dtos.NewsSitesDto;
using MobitekCRMV2.Entity.Entities;
using System.Security.Claims;

namespace MobitekCRMV2.Controllers
{
    [Route("api/newssites")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class NewsSitesApiController : ControllerBase
    {
        private readonly IRepository<NewsSite> _newsSiteRepository;
        private readonly IMapper _mapper;

        public NewsSitesApiController(IRepository<NewsSite> newsSiteRepository, IMapper mapper)
        {
            _newsSiteRepository = newsSiteRepository;
            _mapper = mapper;
        }

        [HttpGet("getNewsSites")]
        public async Task<ActionResult<NewsSiteListDto>> GetNewsSites([FromQuery] NewsSiteFilterDto filter)
       {
            var query = _newsSiteRepository.Table.AsNoTracking();

            if (filter != null)
            {
                if (filter.MinDAScore.HasValue) query = query.Where(x => x.DAScore >= filter.MinDAScore.Value);
                if (filter.MaxDAScore.HasValue) query = query.Where(x => x.DAScore <= filter.MaxDAScore.Value);
                if (filter.MinPAScore.HasValue) query = query.Where(x => x.PAScore >= filter.MinPAScore.Value);
                if (filter.MaxPAScore.HasValue) query = query.Where(x => x.PAScore <= filter.MaxPAScore.Value);
                if (filter.MinPrice.HasValue) query = query.Where(x => x.Price >= filter.MinPrice.Value);
                if (filter.MaxPrice.HasValue) query = query.Where(x => x.Price <= filter.MaxPrice.Value);
                if (!string.IsNullOrEmpty(filter.UserId)) query = query.Where(x => x.UserId == filter.UserId);
                if (!string.IsNullOrEmpty(filter.PromotionId)) query = query.Where(x => x.PromotionId == filter.PromotionId);
                if (!string.IsNullOrEmpty(filter.Name)) query = query.Where(x => x.Name.Contains(filter.Name));
                if (filter.StartDate.HasValue) query = query.Where(x => x.LastUpdateDate >= filter.StartDate);
                if (filter.EndDate.HasValue) query = query.Where(x => x.LastUpdateDate <= filter.EndDate);
            }

            var newsSites = await query
                .Where(x => x.OrganicTraffic >= 1000 || x.OrganicTraffic == 0)
                .Include(x => x.User)
                .ToListAsync();

            var newsSiteListDto = _mapper.Map<NewsSiteListDto>(newsSites);

            return Ok(newsSiteListDto);
        }
    }
}
