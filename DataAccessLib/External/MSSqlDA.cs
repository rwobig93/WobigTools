using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace DataAccessLib.External
{
    public class MSSqlDA : ISqlDA
    {
        private readonly IConfiguration _config;
        public MSSqlDA(IConfiguration config)
        {
            _config = config;
        }
        public async Task<List<T>> LoadData<T, U>(string sql, U parameters)
        {
            string connString = _config.GetConnectionString(_config.GetConnectionString("default"));

            using IDbConnection conn = new SqlConnection(connString);
            var data = await conn.QueryAsync<T>(sql, parameters);

            return data.ToList();
        }
        public async Task SaveData<T>(string sql, T parameters)
        {
            string connString = _config.GetConnectionString(_config.GetConnectionString("default"));

            using IDbConnection conn = new SqlConnection(connString);
            await conn.ExecuteAsync(sql, parameters);
        }
    }
}
