using MediatR;

namespace ConsoleApp1;

public interface IQuery<TResult> : IRequest<QueryResult<TResult>>;

public interface IQueryHandler<TQuery, TResult> : IRequestHandler<TQuery, QueryResult<TResult>>
    where TQuery : IQuery<TResult>;

public class QueryResult<TResult>
{
    public TResult? Result { get; set; }
    public bool Succeeded { get; set; }
    public string? Error { get; set; }
}

