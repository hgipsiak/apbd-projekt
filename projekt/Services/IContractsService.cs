using projekt.DTOs;

namespace projekt.Services;

public interface IContractsService
{
    Task CreateContract(int clientId, int softwareId, ContractDto dto);
    Task PayContract(int contractId);
    Task DeleteContract(int contractId);
}