using System.Data;
using Microsoft.Data.SqlClient;
using Dapper;

namespace HamrahFelez.Repositories
{
    public sealed class DataAccessLayer(string connectionString, int commandTimeout = 60)
    {
        private readonly string _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        private readonly int _commandTimeout = commandTimeout;

        public async Task<List<T>> QueryAsync<T>(
            string sql,
            object? parameters = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

                var command = new CommandDefinition(
                    sql,
                    parameters,
                    commandType: CommandType.Text,
                    commandTimeout: _commandTimeout,
                    cancellationToken: cancellationToken);

                var rows = await connection.QueryAsync<T>(command).ConfigureAwait(false);

                return rows.AsList();
            }
            catch
            {
                throw;
            }
        }

        public async Task<int> ExecuteAsync(
            string procedure,
            object? parameters = null,
            CommandType commandType = CommandType.StoredProcedure,
            CancellationToken cancellationToken = default)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

                var command = new CommandDefinition(
                    procedure,
                    parameters,
                    commandType: commandType,
                    commandTimeout: _commandTimeout,
                    cancellationToken: cancellationToken);

                return await connection.ExecuteAsync(command).ConfigureAwait(false);
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<T>> QuerySPAsync<T>(
            string procedure,
            object? parameters = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

                var command = new CommandDefinition(
                    procedure,
                    parameters,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: _commandTimeout,
                    cancellationToken: cancellationToken);

                var rows = await connection.QueryAsync<T>(command).ConfigureAwait(false);

                return rows.AsList();
            }
            catch
            {
                throw;
            }
        }
    }
}
