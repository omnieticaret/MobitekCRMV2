using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MobitekCRMV2.Business.Services;
using MobitekCRMV2.DataAccess.Context;
using MobitekCRMV2.DataAccess.Repository;
using MobitekCRMV2.DataAccess.UoW;
using MobitekCRMV2.Dto.Dtos.KeywordsDto;
using MobitekCRMV2.Entity.Entities;
using MobitekCRMV2.Jobs;

namespace MobitekCRMV2.Controllers
{
    [Route("api/keywords")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]

    public class KeywordApiController : ControllerBase
    {
        private readonly IRepository<Keyword> _keywordRepository;
        private readonly IRepository<Project> _projectRepository;
        private readonly IRepository<Promotion> _promotionRepository;
        private readonly IRepository<KeywordValue> _keywordValuesRepository;
        private readonly SpaceSerpJob _spaceSerpJob;
        private readonly IUnitOfWork _unitOfWork;
        private readonly CRMDbContext _context;
        private readonly ProjectsService _projectsService;
        private readonly KeywordsService _keywordsService;
        private readonly IRepository<KeywordValue> _keywordvaluesRepository;
        public KeywordApiController(IRepository<Keyword> keywordRepository, IRepository<Project> projectRepository, IRepository<Promotion> promotionRepository, IUnitOfWork unitOfWork, IRepository<KeywordValue> keywordValuesRepository, SpaceSerpJob spaceSerpJob, CRMDbContext context, ProjectsService projectsService, KeywordsService keywordsService, IRepository<KeywordValue> keywordvaluesRepository)
        {
            _keywordRepository = keywordRepository;
            _projectRepository = projectRepository;
            _promotionRepository = promotionRepository;
            _unitOfWork = unitOfWork;
            _keywordValuesRepository = keywordValuesRepository;
            _spaceSerpJob = spaceSerpJob;
            _context = context;
            _projectsService = projectsService;
            _keywordsService = keywordsService;
            _keywordvaluesRepository = keywordvaluesRepository;
        }
        [HttpPost("toggleStar/{keywordId}")]
        public async Task<IActionResult> ToggleStar(string keywordId)
        {
            var keyword = await _context.Keywords.FirstOrDefaultAsync(x => x.Id == keywordId);

            if (keyword == null)
            {
                return NotFound(new { message = "Keyword not found." });
            }

            keyword.IsStarred = !keyword.IsStarred;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Keyword star status toggled.", isStarred = keyword.IsStarred });
        }

        [HttpPost("addKeywords")]
        public async Task<IActionResult> AddKeywords([FromQuery] string projectId, [FromBody] List<KeywordDto> keywords)
        {
            if (string.IsNullOrEmpty(projectId))
            {
                return BadRequest(new { message = "Proje ID'si gerekli." });
            }

            if (keywords == null || !keywords.Any())
            {
                return BadRequest(new { message = "Kelime ve URL listesi boş olamaz." });
            }

            try
            {
                foreach (var item in keywords)
                {
                    if (string.IsNullOrEmpty(item.KeywordName) || string.IsNullOrEmpty(item.TargetURL))
                    {
                        return BadRequest(new { message = "Tüm kelime ve URL çiftleri dolu olmalıdır." });
                    }

                    var keywordString = item.KeywordName.ToLower();
                    var url = item.TargetURL;

                    var existingKeyword = _keywordRepository.Table
                        .FirstOrDefault(x => x.KeywordName.ToLower() == keywordString && x.ProjectId == projectId);

                    if (existingKeyword != null)
                    {
                        existingKeyword.TargetURL = url;
                        _keywordRepository.Update(existingKeyword);
                    }
                    else
                    {
                        var newKeyword = new Keyword
                        {
                            KeywordName = keywordString,
                            TargetURL = url,
                            ProjectId = projectId
                        };

                        await _keywordRepository.AddAsync(newKeyword);
                    }
                }

                await _unitOfWork.CommitAsync();

                return Ok(new { message = "Kelime ekleme işlemi başarılı.", projectId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Sunucu tarafında bir hata oluştu.", error = ex.Message });
            }
        }

        [HttpPost("deleteSelectedItems")]
        public async Task<IActionResult> DeleteSelectedItems([FromBody] List<string> selectedIds)
        {
            if (selectedIds == null || selectedIds.Count == 0)
            {
                return BadRequest("No items were selected for deletion.");
            }

            var selectedKeywords = await _keywordRepository.Table
                .Include(x => x.KeywordValues)
                .Where(x => selectedIds.Contains(x.Id))
                .ToListAsync();

            if (!selectedKeywords.Any())
            {
                return NotFound("No keywords found with the provided IDs.");
            }

            var project = await _context.Projects
                .Include(x => x.Expert)
                .Where(x => x.Expert.UserName == User.Identity.Name && x.Id == selectedKeywords.First().ProjectId)
                .FirstOrDefaultAsync();

            if (!User.IsInRole("admin") && project == null)
            {
                return Forbid("You can only delete keywords belonging to your own projects.");
            }

            foreach (var keyword in selectedKeywords)
            {
                if (keyword.KeywordValues.Any())
                {
                    _keywordValuesRepository.RemoveRange(keyword.KeywordValues);
                }



                _keywordRepository.Remove(keyword);
            }

            await _unitOfWork.CommitAsync();

            return Ok("Selected keywords have been successfully deleted.");
        }

        [HttpPost("keywordValues")]
        public async Task<IActionResult> GetKeywordValues([FromBody] List<string> keywordIds)
        {
            if (keywordIds == null || !keywordIds.Any())
            {
                return BadRequest(new { ErrorMessage = "Keyword IDs cannot be null or empty." });
            }

            var results = new List<object>();

            foreach (var keywordId in keywordIds)
            {
                var keywordEntity = await _keywordRepository.Table
                    .AsNoTracking()
                    .Include(x => x.Project)
                    .FirstOrDefaultAsync(x => x.Id == keywordId);

                if (keywordEntity == null)
                {
                    results.Add(new { KeywordId = keywordId, ErrorMessage = "Keyword not found" });
                    continue;
                }

                var keywordName = keywordEntity.KeywordName;
                var domainUrl = keywordEntity.Project.Url;

                var countryCodeList = _projectsService.StringToList(keywordEntity.Project.CountryCode);

                foreach (var countryCode in countryCodeList)
                {
                    var today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.Local);

                    var lastKeywordValue = await _keywordvaluesRepository.Table
                        .Where(x => x.KeywordId == keywordEntity.Id && x.CreatedDate.Date == today && x.CountryCode == countryCode)
                        .FirstOrDefaultAsync();

                    if (lastKeywordValue == null)
                    {
                        var result = await _spaceSerpJob.GetKeywordValuesAsync(keywordName, domainUrl, countryCode);
                        var values = new KeywordValue
                        {
                            Position = result.position,
                            Domain = domainUrl,
                            Link = result.link,
                            KeywordId = keywordEntity.Id,
                            CreatedDate = DateTime.UtcNow,
                            CountryCode = countryCode
                        };

                        await _keywordvaluesRepository.AddAsync(values);
                        await _unitOfWork.CommitAsync();
                    }
                    else
                    {
                        if (lastKeywordValue.UpdatedDate.ToString("dd/MM/yyyy") != DateTime.UtcNow.ToString("dd/MM/yyyy"))
                        {
                            var result = await _spaceSerpJob.GetKeywordValuesAsync(keywordName, domainUrl, countryCode);
                            lastKeywordValue.Position = result.position;
                            lastKeywordValue.Domain = domainUrl;
                            lastKeywordValue.Link = result.link?.Split(domainUrl)[1];
                            lastKeywordValue.UpdatedDate = DateTime.UtcNow;
                            lastKeywordValue.CountryCode = countryCode;

                            _keywordvaluesRepository.Update(lastKeywordValue);
                            await _unitOfWork.CommitAsync();
                        }
                    }
                }

                results.Add(new { KeywordId = keywordId, Message = "Keyword values updated successfully." });
            }


            return Ok(results);
        }


    }

}
