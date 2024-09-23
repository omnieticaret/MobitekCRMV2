﻿using MobitekCRMV2.Entity.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobitekCRMV2.Dto.Dtos
{
    public class UserDto
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public int ProjectCount { get; set; }
        public int KeywordCount { get; set; }
        public decimal Budget { get; set; }
    }

    public class UsersResponseDto
    {
        public List<UserDto> Users { get; set; }
        public UserType UserType { get; set; }
        public string ErrorMessage { get; set; }
        public decimal TotalBudget { get; set; }
    }

}
