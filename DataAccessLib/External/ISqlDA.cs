using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLib.External
{
    public interface ISqlDA
    {
        Task<List<T>> LoadData<T, U>(string sql, U parameters);
        Task SaveData<T>(string sql, T parameters);
    }
}