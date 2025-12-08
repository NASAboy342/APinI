using Dapper;
using System.Data;
using System.Data.SqlClient;

namespace APinI.Repository
{
    public abstract class BaseRepository
    {
        private readonly string _connectionString;

        protected BaseRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IEnumerable<T> GetData<T>(string spName)
        {
            using var connection = new SqlConnection(_connectionString);
            var data = connection.Query<T>(spName, null, null, true, null, CommandType.StoredProcedure);
            return data;
        }

        public IEnumerable<T> GetData<T>(string spName, object param)
        {
            using var connection = new SqlConnection(_connectionString);
            var data = connection.Query<T>(spName, param, null, true, null, CommandType.StoredProcedure);
            return data;
        }
    }
}