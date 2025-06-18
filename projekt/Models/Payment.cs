using System.ComponentModel.DataAnnotations.Schema;

namespace projekt.Models;

public class Payment
{
    public int PaymentId { get; set; }
    public decimal Price { get; set; }
    public DateTime PaymentDate { get; set; }
    
    [ForeignKey(nameof(Contract))]
    public int ContractId { get; set; }
    public Contract Contract { get; set; }
}