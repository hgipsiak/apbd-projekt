namespace projekt.Models;

public class Software
{
    public int SoftwareId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Version { get; set; }
    public string Category { get; set; }
    public decimal Price { get; set; }
    
    public ICollection<DiscountSoftware> DiscountSoftwares { get; set; }
    
    public ICollection<Contract> Contracts { get; set; }
}