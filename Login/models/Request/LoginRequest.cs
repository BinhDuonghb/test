using System.ComponentModel.DataAnnotations;

namespace Login.models.Request
{
    public class LoginRequest
    {
        [Required]
        public string StudentCode { get; set; } = string.Empty;
        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}
