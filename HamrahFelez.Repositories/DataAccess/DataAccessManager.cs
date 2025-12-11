using Dapper;
using HamrahFelez.ViewModels;
using Microsoft.AspNetCore.Http;
using System.Data;

namespace HamrahFelez.Repositories;

public static class DataAccessManager
{
    private static readonly AsyncLocal<string?> _connectionString = new();
    private static IHttpContextAccessor? _httpContextAccessor;

    public static void Configure(IHttpContextAccessor httpContextAccessor)
    {
        ArgumentNullException.ThrowIfNull(httpContextAccessor, nameof(httpContextAccessor));

        _httpContextAccessor = httpContextAccessor;
    }

    public static string ConnectionString
    {
        get => _connectionString.Value
            ?? throw new InvalidOperationException("Connection string is not set for the current context.");
        set => _connectionString.Value = value;
    }

    public static int DefaultTimeoutSeconds { get; set; } = 60;

    private static CancellationTokenSource? CreateTimeoutCts()
        => DefaultTimeoutSeconds <= 0
            ? null
            : new CancellationTokenSource(TimeSpan.FromSeconds(DefaultTimeoutSeconds));

    private static CancellationTokenSource? CreateLinkedCts(
        CancellationToken callerToken,
        CancellationToken? timeoutToken)
    {
        var tokens = new List<CancellationToken>();

        if (callerToken.CanBeCanceled)
            tokens.Add(callerToken);

        var requestAborted = _httpContextAccessor?.HttpContext?.RequestAborted ?? default;
        if (requestAborted.CanBeCanceled)
            tokens.Add(requestAborted);

        if (timeoutToken is { } t && t.CanBeCanceled)
            tokens.Add(t);

        return tokens.Count == 0
            ? null
            : CancellationTokenSource.CreateLinkedTokenSource(tokens.ToArray());
    }

    private static CancellationToken BuildEffectiveToken(CancellationToken callerToken)
    {
        using var timeoutCts = CreateTimeoutCts();
        using var linkedCts = CreateLinkedCts(callerToken, timeoutCts?.Token);

        return linkedCts?.Token ?? callerToken;
    }

    private static DynamicParameters BuildDynamicParameters(StoredProcedureParameter[]? parameters)
    {
        var dp = new DynamicParameters();

        if (parameters is null || parameters.Length == 0)
            return dp;

        foreach (var p in parameters)
        {
            var name = p.Name.StartsWith("@", StringComparison.Ordinal)
                ? p.Name
                : "@" + p.Name;

            if (string.Equals(p.Name, "FileContent", StringComparison.OrdinalIgnoreCase))
            {
                dp.Add(name, (p.Value as byte[]) ?? Array.Empty<byte>(), DbType.Binary);
            }
            else if (p.DataTable is not null)
            {
                if (string.IsNullOrWhiteSpace(p.TvpTypeName))
                    throw new InvalidOperationException("TVP TypeName is required when DataTable is provided.");

                dp.Add(name, p.DataTable.AsTableValuedParameter(p.TvpTypeName));
            }
            else
            {
                dp.Add(name, p.Value);
            }
        }

        return dp;
    }

    public static async Task<IQueryable<T>> GetFromViewAsync<T>(
        string select,
        string? where,
        object? whereParams = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(select))
            throw new ArgumentNullException(nameof(select));

        var sql = select.Trim().TrimEnd(';');

        if (!string.IsNullOrWhiteSpace(where))
            sql += " " + where.Trim().TrimEnd(';');

        sql += ";";

        var dal = new DataAccessLayer(ConnectionString, DefaultTimeoutSeconds);
        var effectiveToken = BuildEffectiveToken(cancellationToken);

        var list = await dal.QueryViewAsync<T>(sql, whereParams, effectiveToken).ConfigureAwait(false);

        return list.AsQueryable();
    }

    public static async Task<BaseStoredProcedureResponse> ExecuteStoredProcedureAsync(
        string procedure,
        CancellationToken cancellationToken = default,
        params StoredProcedureParameter[] parameters)
    {
        if (string.IsNullOrWhiteSpace(procedure))
            throw new ArgumentNullException(nameof(procedure));

        var dal = new DataAccessLayer(ConnectionString, DefaultTimeoutSeconds);
        var dp = BuildDynamicParameters(parameters);

        dp.Add("@msg", dbType: DbType.String, size: 1000, direction: ParameterDirection.Output);
        dp.Add("@result", dbType: DbType.Int32, direction: ParameterDirection.Output);

        try
        {
            var effectiveToken = BuildEffectiveToken(cancellationToken);

            await dal.ExecuteStoredProcedureAsync(
                    procedure,
                    dp,
                    CommandType.StoredProcedure,
                    effectiveToken)
                .ConfigureAwait(false);

            var result = dp.Get<int?>("@result") ?? 0;
            var msg = dp.Get<string>("@msg") ?? string.Empty;

            return new BaseStoredProcedureResponse(result, msg);
        }
        catch (Exception ex)
        {
            return new BaseStoredProcedureResponse(-1, ex.Message);
        }
    }

    public static async Task<IQueryable<T>> GetFromStoredProcedureAsync<T>(
        string procedure,
        CancellationToken cancellationToken = default,
        params StoredProcedureParameter[] parameters)
    {
        if (string.IsNullOrWhiteSpace(procedure))
            throw new ArgumentNullException(nameof(procedure));

        var dal = new DataAccessLayer(ConnectionString, DefaultTimeoutSeconds);
        var dp = BuildDynamicParameters(parameters);

        var effectiveToken = BuildEffectiveToken(cancellationToken);

        var list = await dal.QueryStoredProcedureAsync<T>(
                procedure,
                dp,
                effectiveToken)
            .ConfigureAwait(false);

        return list.AsQueryable();
    }
}