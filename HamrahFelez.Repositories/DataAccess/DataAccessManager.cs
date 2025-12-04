using Dapper;
using HamrahFelez.ViewModels;
using Microsoft.AspNetCore.Http;
using System.Data;

namespace HamrahFelez.Repositories
{
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
            get => _connectionString.Value ?? throw new InvalidOperationException("Connection string is not set for the current context.");
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
            List<CancellationToken> tokens = [];

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

        public static async Task<IQueryable<T>> GetFromViewAsync<T>(
            string select,
            string? where,
            object? whereParams = null,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(select)) throw new ArgumentNullException(nameof(select));

            var sql = select.Trim().TrimEnd(';');

            if (!string.IsNullOrWhiteSpace(where)) sql += " " + where.Trim().TrimEnd(';');

            sql += ";";

            var dal = new DataAccessLayer(ConnectionString, DefaultTimeoutSeconds);

            using var timeoutCts = CreateTimeoutCts();
            using var linkedCts = CreateLinkedCts(cancellationToken, timeoutCts?.Token);

            var list = await dal.QueryAsync<T>(sql, whereParams, linkedCts?.Token ?? cancellationToken).ConfigureAwait(false);

            return list.AsQueryable();
        }

        public static async Task<BaseStoredProcedureResponse> ExecuteStoredProcedureAsync(
            string procedure,
            CancellationToken cancellationToken = default,
            params StoredProcedureParameter[] parameters)
        {
            if (string.IsNullOrEmpty(procedure)) throw new ArgumentNullException(nameof(procedure));

            var dal = new DataAccessLayer(ConnectionString, DefaultTimeoutSeconds);

            var dp = new DynamicParameters();

            try
            {
                if (parameters is not null)
                {
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        var p = parameters[i];
                        var name = p.Name.StartsWith("@", StringComparison.Ordinal) ? p.Name : "@" + p.Name;

                        if (string.Equals(p.Name, "FileContent", StringComparison.OrdinalIgnoreCase))
                        {
                            dp.Add(name, (p.Value as byte[]) ?? Array.Empty<byte>(), DbType.Binary);
                        }
                        else if (p.DataTable is not null)
                        {
                            var typeName = p.Value?.ToString();

                            if (string.IsNullOrWhiteSpace(p.TvpTypeName))
                                throw new InvalidOperationException("TVP TypeName is required when DataTable is provided.");

                            dp.Add(name, p.DataTable.AsTableValuedParameter(p.TvpTypeName));
                        }
                        else
                        {
                            dp.Add(name, p.Value);
                        }
                    }
                }

                dp.Add("@msg", dbType: DbType.String, size: 1000, direction: ParameterDirection.Output);
                dp.Add("@result", dbType: DbType.Int32, direction: ParameterDirection.Output);

                using var timeoutCts = CreateTimeoutCts();
                using var linkedCts = CreateLinkedCts(cancellationToken, timeoutCts?.Token);

                await dal.ExecuteAsync(procedure, dp, CommandType.StoredProcedure, linkedCts?.Token ?? cancellationToken).ConfigureAwait(false);

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

            var dp = new DynamicParameters();

            if (parameters is not null)
            {
                for (int i = 0; i < parameters.Length; i++)
                {
                    var p = parameters[i];
                    var name = p.Name.StartsWith("@", StringComparison.Ordinal) ? p.Name : "@" + p.Name;

                    if (string.Equals(p.Name, "FileContent", StringComparison.OrdinalIgnoreCase))
                    {
                        dp.Add(name, (p.Value as byte[]) ?? Array.Empty<byte>(), DbType.Binary);
                    }
                    else if (p.DataTable is not null)
                    {
                        var typeName = p.Value?.ToString();

                        if (string.IsNullOrWhiteSpace(p.TvpTypeName))
                            throw new InvalidOperationException("TVP TypeName is required when DataTable is provided.");

                        dp.Add(name, p.DataTable.AsTableValuedParameter(p.TvpTypeName));
                    }
                    else
                    {
                        dp.Add(name, p.Value);
                    }
                }
            }

            using var timeoutCts = CreateTimeoutCts();
            using var linkedCts = CreateLinkedCts(cancellationToken, timeoutCts?.Token);

            var list = await dal.QuerySPAsync<T>(procedure, dp, linkedCts?.Token ?? cancellationToken).ConfigureAwait(false);
            return list.AsQueryable();
        }
    }
}
