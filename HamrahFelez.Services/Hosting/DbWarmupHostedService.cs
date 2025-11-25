using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;

namespace HamrahFelez.Services.Hosting
{
    public class DbWarmupHostedService : IHostedService
    {
        private readonly IConfiguration _configuration;

        public DbWarmupHostedService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                var mainCs = _configuration.GetConnectionString("DbMain");
                var developCs = _configuration.GetConnectionString("DbMain_Develop");

                await WarmpupConnectionAsync(mainCs, "DbMain", cancellationToken);
                await WarmpupConnectionAsync(developCs, "DbMain_Develop", cancellationToken);
            }
            catch (Exception ex)
            {
            }
        }

        private static async Task WarmpupConnectionAsync(string connectionString, string name, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(connectionString)) return;

            var sw = System.Diagnostics.Stopwatch.StartNew();

            await using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync(cancellationToken);

            sw.Stop();
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
