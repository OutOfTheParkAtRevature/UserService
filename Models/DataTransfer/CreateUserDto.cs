using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Models.DataTransfer
{
    public class CreateUserDto
    {
        public string UserName { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
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
        public Guid? TeamID { get; set; }
        [DisplayName("Role Name")]
        public string RoleName { get; set; }
        public string ClientURI { get; set; } = "http://20.62.210.88:80/api/Account/EmailConfirmation";
    }
}
