using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MobitekCRMV2.Business.Services;
using MobitekCRMV2.DataAccess.Context;
using MobitekCRMV2.DataAccess.Repository;
using MobitekCRMV2.DataAccess.UoW;
using MobitekCRMV2.Entity.Entities;
using MobitekCRMV2.Entity.Enums;
using MobitekCRMV2.Extensions;
using MobitekCRMV2.Model;
using MobitekCRMV2.Model.Models;

namespace MobitekCRMV2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminApiController : ControllerBase
    {
        private readonly IRepository<User> _userRepository;
        private UserManager<User> _userManager;
        private RoleManager<Role> _roleManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly CRMDbContext _context;
        private readonly AdminService _adminService;

        public AdminApiController(IRepository<User> userRepository, UserManager<User> userManager,
            RoleManager<Role> roleManager, IUnitOfWork unitOfWork, CRMDbContext context, AdminService adminService)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _roleManager = roleManager;
            _unitOfWork = unitOfWork;
            _context = context;
            _adminService = adminService;
        }
        [HttpGet("index")]
        public async Task<IActionResult> Index(string userType)
        {
            var model = new AdminIndexViewModel();
            var users = await _context.Users.ToListAsync();

            model.UserStatistics = users.Where(x => x.Status == Status.Active).GroupBy(u => u.UserType)
                                .Select(group => new KeyValueModel
                                {
                                    Key = group.Key.GetDisplayName(),
                                    Value = group.Count().ToString()
                                }).ToList();

            var recordList = _adminService.GetRecordList(10);
            var keywordValues = new List<KeyValueModel>();
            recordList.ForEach(item =>
            {
                keywordValues.Add(new KeyValueModel
                {
                    Key = item.Date.ToString("dd-MM-yyyy"),
                    Value = item.Count.ToString()
                });
            });
            model.KeywordValueStatistics = keywordValues;

            model.TodoStatistics = _adminService.GetTodoStatistics();

            return Ok(model);
        }

        [HttpGet("rolelist")]
        public async Task<IActionResult> RoleList(string userType)
        {
            var model = new AdminRoleListModel();
            var users = await _userRepository.Table.AsNoTracking().ToListAsync();

            model.UserList = new List<UserWithRoles>();

            foreach (var item in users)
            {
                var userWithRoles = new UserWithRoles
                {
                    User = item,
                    Roles = (await _userManager.GetRolesAsync(item)).ToList()
                };
                model.UserList.Add(userWithRoles);
            }

            if (!string.IsNullOrEmpty(userType))
            {
                var userTypeEnum = _adminService.GetEnumFromName(userType);
                model.UserList = model.UserList.Where(x => x.User.UserType == userTypeEnum).ToList();
                model.userType = userTypeEnum.GetDisplayName();
            }
            else
            {
                model.UserList = model.UserList.Where(x => x.User.UserType != UserType.Customer
                                                        && x.User.UserType != UserType.Editor
                                                        && x.User.UserType != UserType.Writer).ToList();
                model.userType = "Kullanıcılar";
            }

            model.UserList = model.UserList.OrderBy(x => x.User.Status).ToList();

            return Ok(model); // API için JSON olarak döndürme
        }

        [HttpGet("assignrole/{id}")]
        public async Task<IActionResult> AssignRole(string id)
        {
            var user = await _userRepository.Table.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
            {
                return NotFound($"User with ID {id} not found.");
            }

            var roles = await _userManager.GetRolesAsync(user);
            var roleList = _roleManager.Roles.Select(x => x.Name).ToList();

            var model = new AdminAssignRoleViewModel
            {
                User = user,
                hasRoles = roles.ToList(),
                RoleList = roleList
            };

            return Ok(model);
        }

        [HttpPost("assignrole")]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleRequest request)
        {
            var user = await _userRepository.Table.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.UserId);
            if (user == null)
            {
                return NotFound($"User with ID {request.UserId} not found.");
            }

            var roleList = _roleManager.Roles.Select(x => x.Name).ToList();
            foreach (var role in roleList)
            {
                if (request.SelectedRoles.Contains(role))
                {
                    await _userManager.AddToRoleAsync(user, role);
                }
                else
                {
                    await _userManager.RemoveFromRoleAsync(user, role);
                }
            }

            return Ok(new { message = "Roles updated successfully." });
        }

        [HttpGet("setkeywordvalue")]
        public IActionResult SetKeywordValue()
        {
            var model = new SetKeywordValueModel
            {
                RecordList = _adminService.GetRecordList(10)
            };
            return Ok(model);
        }

        [HttpPost("setkeywordvalue")]
        public async Task<IActionResult> SetKeywordValue([FromBody] SetKeywordValueModel model)
        {
            if (model.TargetDate == DateTime.MinValue || model.SourceDate == DateTime.MinValue)
            {
                model.RecordList = _adminService.GetRecordList(10);
                model.Message = "Kaynak veya Hedef tarihi seçilmedi";
                return BadRequest(model);
            }

            var targetValues = await _context.KeywordValues.Where(x => x.CreatedDate.Date == model.TargetDate).ToListAsync();
            var sourceValues = await _context.KeywordValues.Where(x => x.CreatedDate.Date == model.SourceDate).ToListAsync();

            var differences = sourceValues.Where(sourceItem => !targetValues.Any(targetItem => targetItem.KeywordId == sourceItem.KeywordId)).ToList();
            var todaysKeywords = await _context.Keywords.Include(x => x.Project).Where(x => x.Project.Status == Status.Active).ToListAsync();

            var filteredDifferences = differences.Where(difference => todaysKeywords.Any(keyword => keyword.Id == difference.KeywordId)).ToList();

            if (!filteredDifferences.Any())
            {
                model.RecordList = _adminService.GetRecordList(10);
                model.Message = "Taşınacak veri bulunamadı.";
                return NotFound(model);
            }

            foreach (var difference in filteredDifferences)
            {
                var temp = new KeywordValue
                {
                    Position = difference.Position,
                    Page = difference.Page,
                    Domain = difference.Domain,
                    Link = difference.Link,
                    KeywordId = difference.KeywordId,
                    CreatedDate = model.TargetDate,
                    CountryCode = difference.CountryCode,
                    TargetCheckType = difference.TargetCheckType
                };
                _context.KeywordValues.Add(temp);
                await _context.SaveChangesAsync();
            }

            model.RecordList = _adminService.GetRecordList(10);
            model.Message = $"{model.SourceDate:dd-MM-yyyy} tarihinden {filteredDifferences.Count} adet veri {model.TargetDate:dd-MM-yyyy} tarihine taşındı";
            return Ok(model);
        }

        [HttpGet("todoManage")]
        public IActionResult TodoManage()
        {
            // Assuming you might want to return some default data or status
            return Ok(new { message = "Todo management endpoint is available." });
        }
    }
    public class AssignRoleRequest
    {
        public string UserId { get; set; }
        public List<string> SelectedRoles { get; set; }
    }
}

