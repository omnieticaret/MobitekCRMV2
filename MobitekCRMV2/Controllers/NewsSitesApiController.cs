using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mobitek.CRM.Models.NewsSiteModels;
using MobitekCRMV2.Authentication;
using MobitekCRMV2.Business.Services;
using MobitekCRMV2.DataAccess.Context;
using MobitekCRMV2.DataAccess.Repository;
using MobitekCRMV2.DataAccess.UoW;
using MobitekCRMV2.Dto.Dtos;
using MobitekCRMV2.Dto.Dtos.NewsSitesDto;
using MobitekCRMV2.Entity.Entities;
using MobitekCRMV2.Extensions;
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
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<BackLink> _backlinkRepository;
        private readonly CustomReader _customReader;
        private readonly CRMDbContext _context;
        private readonly NewsSitesService _newsSitesService;

        public NewsSitesApiController(IRepository<NewsSite> newsSiteRepository, IMapper mapper, IUnitOfWork unitOfWork, IRepository<User> userRepository, IRepository<BackLink> backlinkRepository, CustomReader customReader, CRMDbContext context, NewsSitesService newsSitesService)
        {
            _newsSiteRepository = newsSiteRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
            _backlinkRepository = backlinkRepository;
            _customReader = customReader;
            _context = context;
            _newsSitesService = newsSitesService;
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetNewsSites()
        {
            try
            {
                var newsSites = await _newsSiteRepository.Table
                    .AsNoTracking()
                   // .Where(x => x.OrganicTraffic >= 1000 || x.OrganicTraffic == 0)
                    .Include(x => x.User)
                    .ToListAsync();

                var newsSiteDtos = _mapper.Map<List<NewsSiteDto>>(newsSites);

                return Ok(newsSiteDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", details = ex.Message });
            }
        }

        [HttpPost("filter")]
        public async Task<IActionResult> FilterNewsSites([FromBody] NewsSiteFilterDto filter)
        {
            try
            {
                var query = _newsSiteRepository.Table.AsNoTracking();

                if (!string.IsNullOrEmpty(filter.Name))
                    query = query.Where(x => x.Name.Contains(filter.Name));

                if (filter.MinPrice.HasValue)
                    query = query.Where(x => x.Price >= filter.MinPrice.Value);

                if (filter.MaxPrice.HasValue)
                    query = query.Where(x => x.Price <= filter.MaxPrice.Value);

                if (filter.MinDAScore.HasValue)
                    query = query.Where(x => x.DAScore >= filter.MinDAScore.Value);

                if (filter.MaxDAScore.HasValue)
                    query = query.Where(x => x.DAScore <= filter.MaxDAScore.Value);

                if (filter.MinPAScore.HasValue)
                    query = query.Where(x => x.PAScore >= filter.MinPAScore.Value);

                if (filter.MaxPAScore.HasValue)
                    query = query.Where(x => x.PAScore <= filter.MaxPAScore.Value);

                if (filter.MinDRScore.HasValue)
                    query = query.Where(x => x.DRScore >= filter.MinDRScore.Value);

                if (filter.MaxDRScore.HasValue)
                    query = query.Where(x => x.DRScore <= filter.MaxDRScore.Value);

                var filteredNewsSites = await query.Include(x => x.User).ToListAsync();
                var filteredNewsSiteDtos = _mapper.Map<List<NewsSiteDto>>(filteredNewsSites);

                return Ok(new NewsSiteListDto
                {
                    NewsSites = filteredNewsSiteDtos,
                    TotalCount = filteredNewsSiteDtos.Count,
                    AverageMetrics = new Dictionary<string, double>
            {
                { "AveragePrice", filteredNewsSiteDtos.Average(x => x.Price) },
                { "AverageDAScore", filteredNewsSiteDtos.Average(x => x.DAScore) },
            }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", details = ex.Message });
            }
        }

        [HttpPost("add")]
        public async Task<IActionResult> Add([FromBody] NewsSiteCreateUpdateDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { Succeeded = false, Message = "Model is not valid." });

            var existingNewsSite = await _newsSiteRepository.GetByAsync(x => x.Name == model.Name);
            if (existingNewsSite != null)
            {
                return Conflict(new { Succeeded = false, Message = "Bu isimde bir haber sitesi zaten mevcut!" });
            }

            var user = await _userRepository.GetByAsync(x => x.Id == model.UserId);
            if (user == null)
            {
                if (!string.IsNullOrEmpty(model.EditorMail))
                {
                    user = _newsSitesService.CreateUserFromMailAddress(model.EditorMail);

                    if (user == null)
                    {
                        return BadRequest(new { Succeeded = false, Message = "Editor mail already exists!" });
                    }
                }
                else
                {
                    return BadRequest(new { Succeeded = false, Message = "Editor not selected!" });
                }
            }

            var newsSite = new NewsSite
            {
                Name = model.Name,
                Price = model.Price,
                GoogleNews = model.GoogleNews,
                DRScore = model.DRScore,
                LinkedDomains = model.LinkedDomains,
                TotalBacklinks = model.TotalBacklinks,
                OrganicTraffic = model.OrganicTraffic,
                AllTraffic = model.AllTraffic,
                PAScore = model.PAScore,
                DAScore = model.DAScore,
                SpamScore = model.SpamScore,
                EditorMail = model.EditorMail,
                EditorPhone = model.EditorPhone,
                MozDA = model.MozDA,
                Note = model.Note,
                PromotionId = model.PromotionId,
                User = user,
                LastUpdateDate = DateTime.Now
            };

            try
            {
                await _newsSiteRepository.AddAsync(newsSite);
                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Succeeded = false, Message = "Haber sitesi eklenirken bir hata oluştu.", Details = ex.Message });
            }

            return Ok(new { Succeeded = true, Message = "Haber sitesi başarıyla eklendi." });
        }

        [HttpGet("checkName")]
        public async Task<IActionResult> CheckName([FromQuery] string name)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Site adı boş olamaz!"
                    });
                }

                if (name.Length < 2)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Site adı en az 2 karakter olmalıdır!"
                    });
                }

                if (name.Length > 50)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Site adı 50 karakterden uzun olamaz!"
                    });
                }

                if (name.StartsWith(" ") || name.EndsWith(" "))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Site adı başında veya sonunda boşluk olamaz!"
                    });
                }

                var existingNewsSite = await _newsSiteRepository.GetByAsync(x => x.Name.ToLower() == name.ToLower());

                if (existingNewsSite != null)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Bu isimde bir haber sitesi zaten mevcut!"
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Site adı kullanılabilir."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Bir hata oluştu. Lütfen daha sonra tekrar deneyin.",
                    error = ex.Message
                });
            }
        }
    }
}

