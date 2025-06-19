namespace projekt.DTOs;

public class GetProfitDto
{
    public string CurrencyCode { get; set; }
    public decimal Sum { get; set; }
    public List<GetSoftwareDto> Softwares { get; set; }
}

public class GetSoftwareDto
{
    public int SoftwareId { get; set; }
    public string SoftwareName { get; set; }
    public List<GetContractDto> Contracts { get; set; }
}

public class GetContractDto
{
    public int ContractId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TotalPrice { get; set; }
}
public class GetNBPDto
{
    public string Code { get; set; }
    public List<GetNBPRateDto> Rates { get; set; }
}

public class GetNBPRateDto
{
    public decimal Mid { get; set; }
}