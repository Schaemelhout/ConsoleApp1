using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace ConsoleApp1;

public static class MediatRExtensions
{
    private static List<Type> GetTypesImplementing(Type interfaceTypeDefinition)
    {
        return Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(type => type is { IsAbstract: false, IsInterface: false })
            .Where(type => type.GetInterfaces().Any(interfaceType =>
                interfaceType.IsGenericType &&
                interfaceType.GetGenericTypeDefinition() == interfaceTypeDefinition))
            .ToList();
    }

    private static Type GetInterfaceOfType(this Type type, Type interfaceTypeDefinition) =>
        type.GetInterfaces()
            .First(interfaceType => 
                interfaceType.IsGenericType && 
                interfaceType.GetGenericTypeDefinition() == interfaceTypeDefinition);

    public static MediatRServiceConfiguration RegisterQueryResultPipelineBehaviors(this MediatRServiceConfiguration configuration)
    {
        // Get all relevant pipeline behaviors
        var behaviorTypes = GetTypesImplementing(typeof(IPipelineBehavior<,>))
            .Where(behaviorType =>
            {
                // Check if TResponse in IPipelineBehavior<TRequest, TResponse> is QueryResult<T>
                var responseType = behaviorType.GetInterfaceOfType(typeof(IPipelineBehavior<,>)).GetGenericArguments()[1];
                return responseType.IsGenericType && responseType.GetGenericTypeDefinition() == typeof(QueryResult<>);
            })
            .ToList();
        
        // Get all TQuery types (implementing IQuery<TResult>)
        var queryTypes = GetTypesImplementing(typeof(IQuery<>));

        foreach (var queryType in queryTypes)
        {
            // Get TResult from IQuery<TResult>
            var resultType = queryType.GetInterfaceOfType(typeof(IQuery<>)).GetGenericArguments()[0];

            // Create the closed type for QueryResult<TResult>
            var queryResultType = typeof(QueryResult<>).MakeGenericType(resultType);
            
            // Create the closed type for IPipelineBehavior<TQuery, QueryResult<TResult>>
            var behaviorInterfaceType = typeof(IPipelineBehavior<,>).MakeGenericType(queryType, queryResultType);
            
            foreach (var behaviorType in behaviorTypes)
            {
                var behaviorImplementationType = behaviorType.MakeGenericType(queryType, resultType);
                var serviceDescriptor = new ServiceDescriptor(behaviorInterfaceType, behaviorImplementationType, ServiceLifetime.Transient);
                configuration.BehaviorsToRegister.Add(serviceDescriptor);
            }
        }
        
        return configuration;
    }
}