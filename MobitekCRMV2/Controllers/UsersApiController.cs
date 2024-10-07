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
using MobitekCRMV2.Dto.Dtos.UserDto;
using MobitekCRMV2.Dto.Dtos.UserDto.UserDto;
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

        public UsersApiController(IRepository<Project> projectRepository, UserManager<User> userManager, SignInManager<User> signInManager, IRepository<User> userRepository, IUnitOfWork unitOfWork, IRepository<UserInfo> userInfoRepository, IRepository<NewsSite> newsSiteRepository, IRepository<Customer> customerRepository, PasswordService passwordService = null, IAuthService authService = null, TokenHelper tokenHelper = null, IConfiguration configuration = null)
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

                var userDtos = users.Select(u => new UserListDto
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
    }


}


