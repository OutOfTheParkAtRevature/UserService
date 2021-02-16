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
    public class RecipientListDto
    {
        [DisplayName("Recipient List ID")]
        public Guid RecipientListID { get; set; }
        [DisplayName("Recipient ID")]
        public string RecipientID { get; set; }
    }
}
