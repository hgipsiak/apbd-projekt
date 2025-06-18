namespace projekt.Models;

public abstract class Client
{
    public int IdClient { get; set; }
    public string Address { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    
    public DateTime? DeletionDate { get; set; }
    
    public ICollection<Contract> Contracts { get; set; }
}