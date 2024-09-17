using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MobitekCRMV2.Authentication;
using MobitekCRMV2.Business.Services;
using MobitekCRMV2.DataAccess.Repository;
using MobitekCRMV2.DataAccess.UoW;
using MobitekCRMV2.Dto.Dtos;
using MobitekCRMV2.Entity.Entities;
using MobitekCRMV2.Entity.Enums;
using MobitekCRMV2.Model.Models;

namespace MobitekCRMV2.Controllers
{
  //  [Authorize]
 // [Authorize(Roles = MBCRMRoles.Admin_RoleString + ",viewer")]
    [Route("api/[controller]")]
    [ApiController]
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
        public UsersApiController(IRepository<Project> projectRepository, UserManager<User> userManager, SignInManager<User> signInManager, IRepository<User> userRepository, IUnitOfWork unitOfWork, IRepository<UserInfo> userInfoRepository, IRepository<NewsSite> newsSiteRepository, IRepository<Customer> customerRepository, PasswordService passwordService = null, IAuthService authService = null)
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
        }
        [HttpGet("index")]
        public async Task<IActionResult> Index([FromQuery] UserType userType, [FromQuery] bool isAll, [FromQuery] string errorMessage, [FromQuery] string status)
        {
            var allUsers = _userManager.Users.Select(user => new UserDto
            {
                Id = user.Id,
                Name = user.UserName,
                UserType = user.UserType,
                Status = user.Status.ToString(),
                ExpertProjects = user.ExpertProjects.Select(project => new ProjectDto
                {
                    ProjectId = project.Id,
                    ProjectName = project.Name,
                    Budget = !string.IsNullOrEmpty(project.Budget) ? Convert.ToInt32(project.Budget.Replace(".", "")) : (int?)null
                }).ToList()
            }).ToList();

            var usersWithTypes = _userRepository.Table.AsNoTracking()
                .Include(x => x.ExpertProjects.Where(y => y.Status == Status.Active))
                .Where(user => user.UserType == userType && user.Status == Status.Active)
                .Select(user => new UserDto
                {
                    Id = user.Id,
                    Name = user.UserName,
                    UserType = user.UserType,
                    Status = user.Status.ToString(),
                    ExpertProjects = user.ExpertProjects.Select(project => new ProjectDto
                    {
                        ProjectId = project.Id,
                        ProjectName = project.Name,
                        Budget = !string.IsNullOrEmpty(project.Budget) ? Convert.ToInt32(project.Budget.Replace(".", "")) : (int?)null
                    }).ToList()
                })
                .ToList();

            var totalBudget = usersWithTypes.Sum(user => user.ExpertProjects.Sum(project => project.Budget ?? 0));

            if (status == "Passive")
            {
                var passiveUsers = _userRepository.Table.AsNoTracking()
                    .Where(user => user.Status == Status.Passive)
                    .Select(user => new UserDto
                    {
                        Id = user.Id,
                        Name = user.UserName,
                        UserType = user.UserType,
                        Status = user.Status.ToString(),
                        ExpertProjects = user.ExpertProjects.Select(project => new ProjectDto
                        {
                            ProjectId = project.Id,
                            ProjectName = project.Name,
                            Budget = !string.IsNullOrEmpty(project.Budget) ? Convert.ToInt32(project.Budget.Replace(".", "")) : (int?)null
                        }).ToList()
                    })
                    .ToList();

                return Ok(new IndexResponseDto
                {
                    Users = passiveUsers,
                    ErrorMessage = errorMessage,
                    TotalBudget = 0 
                });
            }

            if (!isAll)
            {
                return Ok(new IndexResponseDto
                {
                    Users = usersWithTypes,
                    UserType = userType,
                    ErrorMessage = errorMessage,
                    TotalBudget = totalBudget
                });
            }

            return Ok(new IndexResponseDto
            {
                Users = allUsers,
                ErrorMessage = errorMessage,
                TotalBudget = totalBudget
            });
        }


        [HttpGet("login")]
        public IActionResult Login()
        {
            return Ok(new UserLoginViewModel());
        }

        /// <summary>
        /// Login işlemini yapan method. Başarılı ise token veya success mesajı, başarısız ise hata mesajı döner.
        /// </summary>
        /// <param name="model">Kullanıcının giriş bilgilerini içerir (Email ve Şifre)</param>
        /// <returns>Başarı veya hata mesajı</returns>
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
                // Kullanıcı rollerini alalım
                var roles = await _userManager.GetRolesAsync(user);

                // JWT Token üretelim
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

    }
}

