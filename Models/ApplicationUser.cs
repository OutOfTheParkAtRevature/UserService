using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public Guid StatLineID { get; set; }
    }
}
