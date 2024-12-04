using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MobitekCRMV2.Authentication;
using MobitekCRMV2.DataAccess.Context;
using MobitekCRMV2.DataAccess.Repository;
using MobitekCRMV2.DataAccess.UoW;
using MobitekCRMV2.Dto.Dtos.CustomersDto;
using MobitekCRMV2.Entity.Entities;
using MobitekCRMV2.Entity.Enums;
using Newtonsoft.Json;
using System.Security.Claims;

namespace MobitekCRMV2.Controllers
{
    [Route("api/customers")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]

    /// <summary>
    /// Müşteri operasyonları
    /// </summary>
    public class CustomersApiController : ControllerBase
    {
        private readonly IRepository<Customer> _customerRepository;
        private readonly IConfiguration _configuration;
        private UserManager<User> _userManager;
        private readonly IRepository<User> _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly CRMDbContext _context;
        private readonly TokenHelper _tokenHelper;

        public CustomersApiController(IRepository<Customer> customerRepository, IUnitOfWork unitOfWork, UserManager<User> userManager,
            IRepository<User> userRepository, CRMDbContext context, IConfiguration configuration, TokenHelper tokenHelper)
        {
            _customerRepository = customerRepository;
            _userManager = userManager;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _context = context;
            _configuration = configuration;
            _tokenHelper = tokenHelper;
        }

        [HttpGet("index")]
        public async Task<IActionResult> Index()
        {
            var userName = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
            var userRole = HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;

            // Kullanıcıyı database'den al
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == userName);

            var customers = _customerRepository.Table.Include(s => s.CustomerRepresentative);
            var customerList = new List<Customer>();
            if (User.IsInRole("customer"))
            {
                customerList = await customers.Where(x => x.CustomerRepresentativeId == user.Id).ToListAsync();
            }
            else
            {
                customerList = await customers.ToListAsync();
            }

            List<CustomerListDto2> viewModel = new List<CustomerListDto2>();
            foreach (var customer in customerList)
            {
                var customerListDto = new CustomerListDto2

                {
                    Id = customer.Id,
                    CompanyName = customer.CompanyName,
                    CompanyOfficialWebsite = customer.CompanyOfficialWebsite,
                    CustomerType = customer.CustomerType.ToString(),
                    CustomerRepresentative = (customer.CustomerRepresentative != null && !string.IsNullOrEmpty(customer.CustomerRepresentative.UserName))
            ? customer.CustomerRepresentative.UserName
            : "Yetkili yok",
                    CompanyPhone = customer.CompanyPhone,
                    CompanyEmail = customer.CompanyEmail
                };
                viewModel.Add(customerListDto);
            }

            return Content(JsonConvert.SerializeObject(viewModel), "application/json");

        }


        [HttpPost("add")]

        public async Task<IActionResult> AddCustomer([FromBody] CreateCustomerDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Müşteri Eklenemedi, bilgileri kontrol ediniz");
            }

            var customer = new Customer
            {
                CompanyName = model.CompanyName,
                CompanyAddress = model.CompanyAddress,
                CompanyEmail = model.CompanyEmail,
                CompanyPhone = model.CompanyPhone,
                CompanyOfficialWebsite = model.CompanyOfficialWebsite,
                CustomerType = Enum.Parse<CustomerType>(model.CustomerType)

            };
            await _customerRepository.AddAsync(customer);
            await _unitOfWork.CommitAsync();
            return Ok("Müşteri başarıyla eklendi");
        }


        [HttpGet("detail/{id}")]
        public async Task<ActionResult<CustomerDetailDto>> GetCustomerDetail(string id)
        {
            var customer = await _customerRepository.Table
                                                    .Include(x => x.CustomerRepresentative)
                                                    .Include(x => x.Projects)
                                                    .FirstOrDefaultAsync(x => x.Id == id);
            if (customer == null)
            {
                return NotFound("Customer not found");
            }

            var customerRepresentatives = await _userRepository.Table
                                                               .Where(x => x.UserType == UserType.Customer)
                                                               .OrderBy(x => x.UserName)
                                                               .Select(x => new CustomerRepresentativeDto
                                                               {
                                                                   Id = x.Id,
                                                                   UserName = x.UserName
                                                               })
                                                               .ToListAsync();
            var customerDetailDto = new CustomerDetailDto
            {
                Id = customer.Id,
                CompanyName = customer.CompanyName,
                CustomerRepresentativeId = customer.CustomerRepresentativeId,
                CustomerRepresentativeName = customer.CustomerRepresentative?.UserName,
                CompanyAddress = customer.CompanyAddress,
                CompanyEmail = customer.CompanyEmail,
                CompanyPhone = customer.CompanyPhone,
                CompanyOfficialWebsite = customer.CompanyOfficialWebsite,
                CustomerType = customer.CustomerType.ToString(),
                Projects = customer.Projects.Select(p => new ProjectDto11
                {
                    Id = p.Id,
                    ProjectType = p.ProjectType.ToString(),
                    Status = p.Status.ToString(),
                    Budget = p.Budget,
                    Contract = p.Contract.ToString(),
                    StartDate = p.StartDate,
                    EndDate = p.ReportDate
                }).ToList()
            };

            return Ok(new
            {
                Customer = customerDetailDto,
                CustomerRepresentatives = customerRepresentatives
            });
        }

        [HttpGet("GetCustomerRepresentatives")]
        public async Task<IActionResult> GetCustomerRepresentatives()
        {
            try
            {
                var customerRepresentatives = _userRepository.Table
                    .Where(x => x.UserType == UserType.Customer)
                    .Select(item => new
                    {
                        id = item.Id,
                        userName = item.UserName
                    })
                    .ToList();

                return Ok(customerRepresentatives);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching customer representatives." });
            }
        }

        [HttpGet("GetCustomerRepresentativesByCustomerId/{customerId}")]
        public async Task<IActionResult> GetCustomerRepresentativesByCustomerId(string customerId)
        {
            try
            {
                var customer = await _customerRepository.Table
                    .Include(x => x.CustomerRepresentative)
                    .FirstOrDefaultAsync(x => x.Id == customerId);

                if (customer == null || customer.CustomerRepresentativeId == null)
                    return NotFound(new { message = "Customer or representative not found" });

                var user = await _userRepository.Table
                    .FirstOrDefaultAsync(x => x.Id == customer.CustomerRepresentativeId);

                if (user == null)
                    return NotFound(new { message = "Representative not found" });

                return Ok(new { id = user.Id, userName = user.UserName });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", details = ex.Message });
            }
        }
    }

}

