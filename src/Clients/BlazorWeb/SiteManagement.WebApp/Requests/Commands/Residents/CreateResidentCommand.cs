namespace SiteManagement.WebApp.Requests.Commands.Residents
{
    public class CreateResidentCommand
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string IdenticalNumber { get; set; }
        public string PhoneNumber { get; set; }
    }
}
