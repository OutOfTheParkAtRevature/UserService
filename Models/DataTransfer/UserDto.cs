﻿using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Model;

namespace Models.DataTransfer
{
    public class UserDto
    {
        [DisplayName("User ID")]
        public string Id { get; set; }
        public string UserName { get; set; }
        [DisplayName("Full Name")]
        public string FullName { get; set; }
        [DisplayName("Phone Number")]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }
        [DisplayName("Email Address")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Email must be a valid email address")]
        public string Email { get; set; }
        [DisplayName("Team ID")]
        public Guid TeamID { get; set; }
        public TeamDto Team { get; set; }
        [DisplayName("Role Name")]
        public string RoleName { get; set; }
    }
}
