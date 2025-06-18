namespace projekt.DTOs;

public class PaymentDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime? PaymentDate { get; set; }
    public int UpdateYears { get; set; }
    public decimal SoftwareVersion { get; set; }
    public bool IsInstalment { get; set; }
    public int? InstalmentNumber { get; set; }
    public int ClientId { get; set; }
    public int SoftwareId { get; set; }
}