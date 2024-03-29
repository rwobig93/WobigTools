﻿using System.Collections.Generic;
using System.Threading.Tasks;
using DataAccessLib.External;
using DataAccessLib.Models;

namespace DataAccessLib.Queriables
{
    public class PeopleData : IPeopleData
    {
        private readonly ISqlDA _db;
        public PeopleData(ISqlDA db)
        {
            _db = db;
        }
        public Task<List<PersonModel>> GetPeople()
        {
            string sql = "select FirstName,LastName from people";

            return _db.LoadData<PersonModel, dynamic>(sql, new { });
        }
        public Task InsertPerson(PersonModel person)
        {
            string sql = "insert into people (FirstName, LastName) values (@FirstName, @LastName);";

            return _db.SaveData(sql, person);
        }
    }
}
