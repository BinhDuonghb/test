namespace Login.models.Reponse
{
    public class LoginResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        
        public string Message { get; set; } = string.Empty;
        public bool Success { get; set; }
    }
}
