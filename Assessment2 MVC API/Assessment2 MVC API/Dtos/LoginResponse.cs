namespace Assessment2_MVC_API.Dtos
{
    public class LoginResponse
    {
        public bool Success { get; set; }
        public string? Email { get; set; }
        public string? AccessToken { get; set; }
        public string? Message { get; set; }

        public int? ExpiresIn { get; set; }
    }
}
