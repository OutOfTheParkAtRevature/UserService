using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public int TeamID { get; set; }
        [DisplayName("Role ID")]
        public int RoleID { get; set; }
    }
}
