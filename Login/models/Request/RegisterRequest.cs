using System.ComponentModel.DataAnnotations;

namespace Login.models.Request
{
    public class RegisterRequest
    {
        [Required]
        public string UserName { get; set; } = string.Empty;
        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
        [DataType(DataType.Password),Compare(nameof(Password),ErrorMessage = "PassWord must not alike")]
        public string Password2 { get; set; } = string.Empty;
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
