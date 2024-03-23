namespace SiteManagement.WebApp.Requests.Commands.Residents
{
    public class ResidentLoginCommand
    {
        public string? Email { get; set; }
        public string? IdenticalNumber { get; set; }
        public string Password { get; set; }
    }
}
