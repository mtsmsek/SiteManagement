namespace SiteManagement.Application.Validators.Residents;

public interface IResidentCommand
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public int BirthYear { get; set; }
    public int BirthMonth { get; set; }
    public int BirthDay { get; set; }
    public string IdenticalNumber { get; set; }
    public string PhoneNumber { get; set; }
}
