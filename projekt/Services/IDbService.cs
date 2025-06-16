using projekt.DTOs;
using projekt.Models;

namespace projekt.Services;

public interface IDbService
{
    Task AddNewPerson(PersonClientDto dto);
    Task UpdatePerson(int id, PersonClientDto dto);
    Task DeletePerson(int id);
}