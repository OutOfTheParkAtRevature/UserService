using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        [DisplayName("Team ID")]
        [ForeignKey("TeamID")]
        public Guid TeamID { get; set; }
        [DisplayName("Role Name")]
        [ForeignKey("Name")]
        public string RoleName { get; set; }
        [DisplayName("Stat Line ID")]
        [ForeignKey("StatLineID")]
        public Guid? StatLineID { get; set; } = null;
    }
}
