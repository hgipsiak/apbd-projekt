using System.ComponentModel.DataAnnotations.Schema;

namespace projekt.Models;

public class Contract
{
    public int ContractId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TotalPrice { get; set; }
    public int UpdateYears { get; set; }
    public decimal SoftwareVersion { get; set; }
    public bool IsInstalment { get; set; }
    public int? InstalmentsQuantity { get; set; }
    
    [ForeignKey(nameof(Client))]
    public int ClientId { get; set; }
    public Client Client { get; set; }
    [ForeignKey(nameof(Software))]
    public int SoftwareId { get; set; }
    public Software Software { get; set; }
    
    public ICollection<Payment> Payments { get; set; }
}