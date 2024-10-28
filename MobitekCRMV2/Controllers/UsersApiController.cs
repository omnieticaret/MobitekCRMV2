using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MobitekCRMV2.Authentication;
using MobitekCRMV2.Business.Services;
using MobitekCRMV2.DataAccess.Repository;
using MobitekCRMV2.DataAccess.UoW;
using MobitekCRMV2.Dto.Dtos.ProjectsDto;
using MobitekCRMV2.Dto.Dtos.UsersDto;
using MobitekCRMV2.Dto.Dtos.UsersDtos;
using MobitekCRMV2.Entity.Entities;
using MobitekCRMV2.Entity.Enums;
using MobitekCRMV2.Model.Models;
using System.Security.Claims;

namespace MobitekCRMV2.Controllers
{
    //  [Authorize]
    // [Authorize(Roles = MBCRMRoles.Admin_RoleString + ",viewer")]
    [Route("api/users")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class UsersApiController : ControllerBase
    {
        private readonly IRepository<User> _userRepository;
        private UserManager<User> _userManager;
        private RoleManager<Role> _roleManager;
        private SignInManager<User> _signInManager;

        private readonly IAuthService _authService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Project> _projectRepository;
        private readonly IRepository<UserInfo> _userInfoRepository;
        private readonly IRepository<NewsSite> _newsSiteRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly PasswordService _passwordService;
        private readonly TokenHelper _tokenHelper;
        private readonly IConfiguration _configuration;
        private readonly AdminService _adminService;
        private readonly IMapper _mapper;

        public UsersApiController(IRepository<Project> projectRepository, UserManager<User> userManager, SignInManager<User> signInManager, IRepository<User> userRepository, IUnitOfWork unitOfWork, IRepository<UserInfo> userInfoRepository, IRepository<NewsSite> newsSiteRepository, IRepository<Customer> customerRepository, PasswordService passwordService = null, IAuthService authService = null, TokenHelper tokenHelper = null, IConfiguration configuration = null, AdminService adminService = null, IMapper mapper = null, RoleManager<Role> roleManager = null)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userRepository = userRepository;
            _projectRepository = projectRepository;

            _unitOfWork = unitOfWork;
            _userInfoRepository = userInfoRepository;
            _newsSiteRepository = newsSiteRepository;
            _customerRepository = customerRepository;
            _passwordService = passwordService;
            _authService = authService;
            _tokenHelper = tokenHelper;
            _configuration = configuration;
            _adminService = adminService;
            _mapper = mapper;
            _roleManager = roleManager;
        }
        [HttpGet("index")]
        public async Task<ActionResult<UsersResponseDto>> Index([FromQuery] UserType userType, [FromQuery] bool isAll, [FromQuery] string status)
        {
            var userName = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
            var userRole = HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;
            try
            {
                var users = isAll
                    ? await _userManager.Users.ToListAsync()
                    : await _userRepository.Table
                        .AsNoTracking()
                        .Include(x => x.ExpertProjects.Where(y => y.Status == Status.Active))
                        .Where(user => user.UserType == userType && user.Status == Status.Active)
                        .ToListAsync();

                if (status == "Passive")
                {
                    users = await _userRepository.Table
                        .AsNoTracking()
                        .Where(user => user.Status == Status.Passive)
                        .ToListAsync();
                }

                var userDtos = users.Select(u => new UserListDto2
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    ProjectCount = u.ExpertProjects?.Count(p => p.Status == Status.Active) ?? 0,
                    //  KeywordCount = u.ExpertProjects?.Sum(p => p.KeywordCount) ?? 0,
                    Budget = u.ExpertProjects?.Sum(p => decimal.TryParse(p.Budget?.Replace(".", ""), out decimal budget) ? budget : 0) ?? 0
                }).ToList();

                decimal totalBudget = userDtos.Sum(u => u.Budget);

                return Ok(new UsersResponseDto
                {
                    Users = userDtos,
                    UserType = userType,
                    ErrorMessage = null,
                    TotalBudget = totalBudget
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new UsersResponseDto { ErrorMessage = ex.Message });
            }
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            return Ok(new UserLoginViewModel());
        }
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return Unauthorized(new { ErrorMessage = "Email veya parola yanlış" });
            }

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, true, false);

            if (result.Succeeded)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var token = _authService.GenerateJwtToken(user, roles);

                return Ok(new
                {
                    Message = "Login başarılı",
                    Token = token,
                    Roles = roles
                });
            }

            return Unauthorized(new { ErrorMessage = "Email veya parola yanlış" });
        }

        [HttpPost("add")]

        public async Task<IActionResult> Add(CreateUserDto createUserDto)
        {

            IdentityResult result = null;
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var user = new User
                {
                    UserName = createUserDto.UserName,
                    Email = createUserDto.Email,
                    UserType = Enum.Parse<UserType>(createUserDto.UserType),
                    PhoneNumber = createUserDto.PhoneNumber,
                    Status = Enum.Parse<Status>(createUserDto.Status)
                };

                result = await _userManager.CreateAsync(user, createUserDto.Password);

                if (result.Succeeded)
                {
                    await _unitOfWork.CommitAsync();

                    if (user.UserType == UserType.SeoExpert)
                        await _userManager.AddToRoleAsync(user, "seo_expert");
                    else if (user.UserType == UserType.SemExpert)
                        await _userManager.AddToRoleAsync(user, "sem_expert");
                    else if (user.UserType == UserType.Customer)
                        await _userManager.AddToRoleAsync(user, "customer");

                    if (user.UserName.Contains("Admin") || user.UserName.Contains("Turgut"))
                        await _userManager.AddToRoleAsync(user, "admin");

                    await _unitOfWork.CommitAsync();
                    return Ok("Kullanıcı başarıyla eklendi");
                }
                else
                {
                    return BadRequest("Kullanıcı Eklenemedi!");
                }

            }
            catch (Exception)
            {
                return BadRequest(new { Errors = result.Errors.Select(e => e.Description) });
            }

        }

        [HttpGet("detail/{id}")]
        public async Task<ActionResult<UserDetailDto>> GetUserDetail(string id)
        {
            var user = await _userRepository.Table.AsNoTracking()
                .Include(x => x.ExpertProjects.Where(y => y.Status == Status.Active))
                .FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            var customers = _customerRepository.Table
                .Where(x => x.CustomerRepresentativeId == user.Id)
                .ToList();

            if (user.UserType == UserType.Customer)
            {
                user.ExpertProjects = _projectRepository.Table.AsNoTracking()
                    .Where(x => x.Customer.CustomerRepresentativeId == id && x.Status == Status.Active)
                    .ToList();
            }

            var userDetailDto = new UserDetailDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Status = user.Status.ToString(),
                UserType = user.UserType.ToString(),
                ExpertProjects = user.ExpertProjects.Select(project => new ProjectDetailDto11
                {
                    ProjectId = project.Id,
                    ProjectUrl = project.Url,
                    Budget = project.Budget,
                    Duration = CalculateDuration(project.StartDate)
                }).ToList()
            };

            return Ok(userDetailDto);
        }

        [HttpGet("roleList")]
        public async Task<IActionResult> GetRoleList(string? userType = null)
         {
            var users = await _userRepository.Table.AsNoTracking().ToListAsync();
            var userSummaries = new List<UserSummaryDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var userSummary = new UserSummaryDto
                {
                    Id = user.Id.ToString(),
                    UserName = user.UserName,
                    Email = user.Email,
                    Roles = roles.ToList(),
                    UserType = user.UserType,
                    Department = user.Department,
                    Status = user.Status
                };

                userSummaries.Add(userSummary);
            }
            if (!string.IsNullOrEmpty(userType))
            {
                var userTypeEnum = _adminService.GetEnumFromName(userType);
                userSummaries = userSummaries
                    .Where(x => x.UserType == userTypeEnum)
                    .ToList();
            }
            else
            {
                userSummaries = userSummaries
                    .Where(x => x.UserType != UserType.Customer && x.UserType != UserType.Editor && x.UserType != UserType.Writer)
                    .ToList();
            }

            userSummaries = userSummaries.OrderBy(x => x.Status).ToList();
            var totalCount = userSummaries.Count;

            var result = new UserListDto
            {
                Users = userSummaries,
                TotalCount = totalCount
            };

            return Ok(result);
        }



        [HttpGet("assignRole/{id}")]
        public async Task<IActionResult> AssignRole(string id)
        {
            var user = await _userRepository.Table.FirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
                return NotFound("User not found.");

            var roles = await _userManager.GetRolesAsync(user);

            var roleList = await _roleManager.Roles.Select(x => x.Name).ToListAsync();


            var model = new AdminAssignRoleViewModel
            {
                User = _mapper.Map<UserDto>(user),
                hasRoles = roles.ToList(),
                RoleList = roleList
            };

            return Ok(model);
        }

        [HttpPost("assignrole")]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleRequest request)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null)
            {
                return NotFound($"User with ID {request.UserId} not found.");
            }
            var currentRoles = await _userManager.GetRolesAsync(user);
            var roleList = _roleManager.Roles.Select(x => x.Name).ToList();

            var rolesToRemove = currentRoles.Where(r => !request.SelectedRoles.Contains(r));
            if (rolesToRemove.Any())
            {
                await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
            }

            var rolesToAdd = request.SelectedRoles.Where(r => !currentRoles.Contains(r));
            if (rolesToAdd.Any())
            {
                await _userManager.AddToRolesAsync(user, rolesToAdd);
            }

            return Ok(new { message = "Roles updated successfully." });
        }

        private string CalculateDuration(DateTime startDate)
        {
            string myString = "";
            var duration = DateTime.Now - startDate;
            var totalDays = duration.TotalDays;
            var totalMonths = totalDays / 30;
            var totalYears = totalMonths / 12;
            var months = (int)(totalMonths % 12);
            var years = (int)(totalYears);

            if (years > 0)
            {
                myString = myString + years + " yıl ";
            }
            if (months > 0)
            {
                myString = myString + months + " ay ";
            }

            return myString;
        }
        public class AssignRoleRequest
        {
            public string UserId { get; set; }
            public List<string> SelectedRoles { get; set; }
        }
    }



}


