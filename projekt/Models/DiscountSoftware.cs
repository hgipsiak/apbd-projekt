using System.ComponentModel.DataAnnotations.Schema;

namespace projekt.Models;

public class DiscountSoftware
{
    [ForeignKey(nameof(Software))]
    public int SoftwareId { get; set; }
    public Software Software { get; set; }
    [ForeignKey(nameof(Discount))]
    public int DiscountId { get; set; }
    public Discount Discount { get; set; }
}