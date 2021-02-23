using System.ComponentModel.DataAnnotations;

namespace Models.DataTransfer
{
    public class LoginDto
    {
        public string UserName { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
