using MediatR;

namespace ConsoleApp1;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        Console.WriteLine($"'{GetType().Name}' is processing '{request.GetType().Name}'");
        
        var result = await next(cancellationToken);
        return result;
    }
}

public interface IValidatableQuery 
{
}

public class QueryValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IValidatableQuery
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        Console.WriteLine($"'{GetType().Name}' is processing '{request.GetType().Name}'");

        if (typeof(TResponse).IsGenericType && typeof(TResponse).GetGenericTypeDefinition() == typeof(QueryResult<>))
        {
            if (DoSomeValidation() == false)
            {
                var failedResult = Activator.CreateInstance<TResponse>();
                typeof(TResponse).GetProperty("Succeeded")?.SetValue(failedResult, false);
                typeof(TResponse).GetProperty("Error")?.SetValue(failedResult, "Validation failed from QueryValidationBehavior");
                return failedResult;
            }
        }
        
        var result = await next(cancellationToken);
        return result;
    }
    
    private static bool DoSomeValidation() => false; // Set to false to demonstrate failure
}

public class QueryResultValidationBehavior<TQuery, TResult> : IPipelineBehavior<TQuery, QueryResult<TResult>>
    where TQuery : IQuery<TResult>
{
    public async Task<QueryResult<TResult>> Handle(TQuery query, RequestHandlerDelegate<QueryResult<TResult>> next, CancellationToken cancellationToken)
    {
        Console.WriteLine($"'{GetType().Name}' is processing '{query.GetType().Name}'");

        if (DoSomeValidation() == false)
        {
            return new QueryResult<TResult> 
            { 
                Succeeded = false, 
                Error = "Validation failed from QueryResultValidationBehavior" 
            };
        }
        
        var result = await next(cancellationToken);
        return result;
    }
    
    private static bool DoSomeValidation() => false; 
