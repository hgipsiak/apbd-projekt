namespace projekt.Models;

public class Instalment : Payment
{
    public int InstalmentNumber { get; set; }
    public DateTime DueTo { get; set; }
}