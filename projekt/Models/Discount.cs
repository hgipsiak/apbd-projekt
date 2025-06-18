using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace projekt.Models;

public class Discount
{
    public int DiscountId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    [Range(0,1)]
    public decimal Value { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    
    public ICollection<DiscountSoftware> DiscountSoftwares { get; set; }
}