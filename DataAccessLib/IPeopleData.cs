using System.Collections.Generic;
using System.Threading.Tasks;
using DataAccessLib.Models;

namespace DataAccessLib
{
    public interface IPeopleData
    {
        Task<List<PersonModel>> GetPeople();
        Task InsertPerson(PersonModel person);
    }
}