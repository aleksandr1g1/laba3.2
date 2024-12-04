using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace laba3test.Infrastrucutre
{
    public abstract class RepositoryBase<T>
    {
        protected readonly string ConnectionString;

        public RepositoryBase(IConfiguration configuration)
        {
            ConnectionString = configuration.GetConnectionString("DefaultConnection") ??
                throw new ArgumentNullException("ConnectionStrings:DefaultConnection is missing");
        }

        public abstract Task<IEnumerable<T>> GetAllAsync();
        public abstract Task<T> GetByIdAsync(Guid id);
        public abstract Task<Guid> InsertAsync(T entity);
        public abstract Task UpdateAsync(Guid id, T entity);
        public abstract Task DeleteAsync(Guid id);

        protected async Task<IEnumerable<T>> ExecuteSqlReaderAsync(string sql)
        {
            var results = new List<T>();

            using var connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();

            using var command = new NpgsqlCommand(sql, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                results.Add(GetEntityFromReader(reader));
            }
            return results;
        }

        protected async Task ExecuteSqlAsync(string sql)
        {
            using var connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();
            using var command = new NpgsqlCommand(sql, connection);
            await command.ExecuteNonQueryAsync();
        }

        protected abstract T GetEntityFromReader(NpgsqlDataReader reader);
    }
}