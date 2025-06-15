using projekt.DTOs;
using projekt.Models;

namespace projekt.Services;

public interface IDbService
{
    Task AddNewPerson(AddPersonClientDto dto);
}