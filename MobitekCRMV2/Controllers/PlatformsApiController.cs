using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MobitekCRMV2.DataAccess.Repository;
using MobitekCRMV2.DataAccess.UoW;
using MobitekCRMV2.Dto.Dtos;
using MobitekCRMV2.Dto.Dtos.PlatformsDto;
using MobitekCRMV2.Entity.Entities;

namespace MobitekCRMV2.Controllers
{
    [Route("api/platforms")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class PlatformsApiController : ControllerBase
    {
        private readonly IRepository<Platform> _platformRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PlatformsApiController(IRepository<Platform> platformRepository, IUnitOfWork unitOfWork)
        {
            _platformRepository = platformRepository;
            _unitOfWork = unitOfWork;
        }

        [HttpGet("getListPlatforms")]
        public async Task<ActionResult<List<PlatformsListDto2>>> GetListPlatforms()
        {
            var platforms = await _platformRepository.GetAllAsync();

            var allPlatformsViews = platforms.Select(x => new PlatformsListDto2
            {
                Id = x.Id,
                Name = x.Name
            }).ToList();
            return Ok(allPlatformsViews);
        }

        [HttpPost("add")]
        public async Task<IActionResult> Add(PlatformsListDto2 model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Succeeded = false, Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
            }

            try
            {
                var platform = new Platform { Name = model.Name }; 
                await _platformRepository.AddAsync(platform);
                await _unitOfWork.CommitAsync();

                return Ok(new { Succeeded = true, Message = "Platform başarıyla eklendi." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Succeeded = false, Message = "Bir hata oluştu.", Error = ex.Message });
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var platform = await _platformRepository.Table.FirstOrDefaultAsync(x => x.Id == id);
                if (platform == null)
                {
                    return NotFound(new { Succeeded = false, Message = "Platform bulunamadı." });
                }

                _platformRepository.Remove(platform);
                await _unitOfWork.CommitAsync();

                return Ok(new { Succeeded = true, Message = "Platform başarıyla silindi." });
            }
            catch (Exception ex)
            {
                // Hata loglama işlemi burada yapılabilir
                return StatusCode(500, new { Succeeded = false, Message = "Bir hata oluştu.", Error = ex.Message });
            }
        }
    }
}
