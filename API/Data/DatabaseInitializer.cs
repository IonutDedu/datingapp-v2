using DbUp;

namespace API.Data;

public class DatabaseInitializer : IHostedService
{

    private readonly string _connectionString;

    public DatabaseInitializer(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await InitializeAsync();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        //throw new NotImplementedException();
    }
    
    public async Task InitializeAsync()
    {
        // the database is already in the connection string; if it doesn't exists, it will be created.
        EnsureDatabase.For.MySqlDatabase(_connectionString);

        // make sure scripts are embeded in Assembly when they are added in the Scripts folder !!!
        var upgrader = DeployChanges.To.MySqlDatabase(_connectionString)
            .WithScriptsEmbeddedInAssembly(typeof(DatabaseInitializer).Assembly)
            .WithTransactionPerScript()
            //.LogToConsole()
            .Build();

        if (upgrader.IsUpgradeRequired())
        {
            var result = upgrader.PerformUpgrade();
        }

        await Task.CompletedTask; 
    }
}
