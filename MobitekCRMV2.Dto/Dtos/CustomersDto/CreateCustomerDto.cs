using MobitekCRMV2.Entity.Entities;
using MobitekCRMV2.Entity.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobitekCRMV2.Dto.Dtos.CustomersDto
{
    public class CreateCustomerDto
    {
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyEmail { get; set; }
        public string CompanyPhone { get; set; }
        public string CompanyOfficialWebsite { get; set; }
        public string CustomerType { get; set; }
        public string CustomerRepresentative { get; set; }
        public string Projects { get; set; }    

    }
}
