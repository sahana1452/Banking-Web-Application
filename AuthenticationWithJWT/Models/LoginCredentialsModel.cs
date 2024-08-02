namespace AuthenticationWithJWT.Models
{
    public class LoginCredentialsModel
{
    public int CustomerID { get; set; }
    public string Username { get; set; }
    public string PasswordHash { get; set; }
    // Add other properties as needed
}
}
