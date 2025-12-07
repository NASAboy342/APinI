using Dapper;
using System.Data;
using System.Data.SqlClient;

namespace APinI.Repository
{
    public abstract class BaseRepository
    {
        protected readonly IDbConnection GetConnection;

        protected BaseRepository(string connectionString)
        {
            GetConnection = new SqlConnection(connectionString);
        }

        public IEnumerable<T> GetData<T>(string spName)
        {
            var data = GetConnection.Query<T>(spName, null, null, true, null, CommandType.StoredProcedure);
            return data;
        }

        public IEnumerable<T> GetData<T>(string spName, object param)
        {
            var data = GetConnection.Query<T>(spName, param, null, true, null, CommandType.StoredProcedure);
            return data;
        }
    }
}