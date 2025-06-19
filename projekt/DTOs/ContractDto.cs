namespace projekt.DTOs;

public class ContractDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int UpdateYears { get; set; }
    public decimal SoftwareVersion { get; set; }
    public bool IsInstalment { get; set; }
    public int? InstalmentNumber { get; set; }
}