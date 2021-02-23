using System;
using System.ComponentModel;

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
