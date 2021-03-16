using System.Collections.Generic;
using System.Threading.Tasks;
using DataAccessLib.Models;

namespace DataAccessLib.Queriables
{
    public interface IPeopleData
    {
        Task<List<PersonModel>> GetPeople();
        Task InsertPerson(PersonModel person);
    }
}