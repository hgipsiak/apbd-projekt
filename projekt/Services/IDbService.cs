using projekt.DTOs;
using projekt.Models;

namespace projekt.Services;

public interface IDbService
{
    Task AddNewPerson(PersonClientDto dto);
    Task UpdatePerson(int id, PersonClientDto dto);
    Task DeletePerson(int id);
    Task AddNewCompany(CompanyClientDto dto);
    Task UpdateCompany(int id, CompanyClientDto dto);
    Task CreateContract(int clientId, int softwareId, PaymentDto dto);
    Task PayContract(int contractId);
    Task DeleteContract(int contractId);
}