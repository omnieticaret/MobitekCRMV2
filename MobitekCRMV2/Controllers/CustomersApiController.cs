using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MobitekCRMV2.Authentication;
using MobitekCRMV2.DataAccess.Context;
using MobitekCRMV2.DataAccess.Repository;
using MobitekCRMV2.DataAccess.UoW;
using MobitekCRMV2.Dto.Dtos.CustomerDto;
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

            List<CustomerListDto> viewModel = new List<CustomerListDto>();
            foreach (var customer in customerList)
            {
                var customerListDto = new CustomerListDto

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
        [Authorize(AuthenticationSchemes = "Bearer")]
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

 

    }
}
