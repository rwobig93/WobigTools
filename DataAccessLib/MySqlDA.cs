using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace DataAccessLib
{
    public class MySqlDA : ISqlDA
    {
        private readonly IConfiguration _config;
        public MySqlDA(IConfiguration config)
        {
            _config = config;
        }

        public async Task<List<T>> LoadData<T, U>(string sql, U parameters)
        {
            using IDbConnection conn = new MySqlConnection(_config.GetConnectionString("default"));
            var rows = await conn.QueryAsync<T>(sql, parameters);
            return rows.ToList();
        }

        public Task SaveData<T>(string sql, T parameters)
        {
            using IDbConnection conn = new MySqlConnection(_config.GetConnectionString("default"));
            return conn.ExecuteAsync(sql, parameters);
        }
    }
}
