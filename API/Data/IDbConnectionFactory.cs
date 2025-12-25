using System;
using System.Data;

namespace API.Data;

public interface IDbConnectionFactory
{
    public Task<IDbConnection> CreateConnectionAsync(CancellationToken cancellationToken = default);
}
