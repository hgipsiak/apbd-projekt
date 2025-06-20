using projekt.DTOs;
using projekt.Models;

namespace projekt.Services;

public interface IClientsService
{
    Task AddNewPerson(PersonClientDto dto);
    Task UpdatePerson(int id, PersonClientDto dto);
    Task DeletePerson(int id);
    Task AddNewCompany(CompanyClientDto dto);
    Task UpdateCompany(int id, CompanyClientDto dto);
}