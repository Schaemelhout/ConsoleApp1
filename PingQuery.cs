namespace ConsoleApp1;

public class Pong { }

public class PingQuery : IQuery<Pong>, IValidatableQuery { }

public class PingQueryHandler : IQueryHandler<PingQuery, Pong>
{
    public Task<QueryResult<Pong>> Handle(PingQuery query, CancellationToken cancellationToken)
    {
        Console.WriteLine($"'{GetType()}' is processing '{query.GetType()}'");
        
        var response = new QueryResult<Pong>
        {
            Result = new Pong(),
            Succeeded = true,
        };

        return Task.FromResult(response);
    }
}