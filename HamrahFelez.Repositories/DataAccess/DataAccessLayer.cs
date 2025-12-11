using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Data.Common;

namespace HamrahFelez.Repositories;

public sealed class DataAccessLayer(string connectionString, int commandTimeout = 60)
{
    private readonly string _connectionString =
        !string.IsNullOrWhiteSpace(connectionString)
        ? connectionString
        : throw new ArgumentException("Connection string cannot be null or empty.", nameof(connectionString));

    private readonly int _commandTimeout =
        commandTimeout > 0
        ? commandTimeout
        : throw new ArgumentOutOfRangeException(nameof(commandTimeout), "Command timeout must be positive.");

    private DbConnection CreateConnection()
        => new SqlConnection(_connectionString);

    private CommandDefinition CreateCommand(
        string sqlOrProcedure,
        object? parameters,
        CommandType commandType,
        CancellationToken cancellationToken) =>
        new(
            sqlOrProcedure,
            parameters,
            commandType: commandType,
            commandTimeout: _commandTimeout,
            cancellationToken: cancellationToken);

    public async Task<List<T>> QueryViewAsync<T>(
        string sql,
        object? parameters = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var connection = CreateConnection();
            await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

            var command = CreateCommand(sql, parameters, CommandType.Text, cancellationToken);
            var rows = await connection.QueryAsync<T>(command).ConfigureAwait(false);

            return rows.AsList();
        }
        catch
        {
            throw;
        }
    }

    public async Task<List<T>> QueryStoredProcedureAsync<T>(
        string procedure,
        object? parameters = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var connection = CreateConnection();
            await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

            var command = CreateCommand(procedure, parameters, CommandType.StoredProcedure, cancellationToken);
            var rows = await connection.QueryAsync<T>(command).ConfigureAwait(false);

            return rows.AsList();
        }
        catch
        {
            throw;
        }
    }

    public async Task<int> ExecuteStoredProcedureAsync(
        string procedure,
        object? parameters = null,
        CommandType commandType = CommandType.StoredProcedure,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var connection = CreateConnection();
            await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

            var command = CreateCommand(procedure, parameters, commandType, cancellationToken);
            return await connection.ExecuteAsync(command).ConfigureAwait(false);
        }
        catch
        {
            throw;
        }
    }
}