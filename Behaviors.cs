using MediatR;

namespace ConsoleApp1;

public class Behavior_2<TQuery, TResult> : IPipelineBehavior<TQuery, QueryResult<TResult>>
    where TQuery : IQuery<TResult>
{
    public async Task<QueryResult<TResult>> Handle(TQuery query, RequestHandlerDelegate<QueryResult<TResult>> next, CancellationToken cancellationToken)
    {
        Console.WriteLine($"'{GetType()}' is processing '{query.GetType()}'");

        if (DoSomeValidation() == false)
            return new QueryResult<TResult> { Succeeded = false, Error = "Validaton failed" };
        
        var result = await next(cancellationToken);
        return result;
    }
    
    private static bool DoSomeValidation() => false;
}
