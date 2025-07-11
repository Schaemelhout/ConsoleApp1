using System.Reflection;
using ConsoleApp1;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
services.AddLogging();

var queryTypes = Assembly
    .GetExecutingAssembly()
    .GetTypes()
    .Where(type => type is { IsAbstract: false, IsInterface: false })
    .Where(type => type.GetInterfaces().Any(interfaceType => interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IQuery<>)));

foreach (var queryType in queryTypes)
{
    var resultType = queryType
        .GetInterfaces()
        .First(interfaceType => interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IQuery<>))
        .GetGenericArguments()[0];

    var pipelineType = typeof(IPipelineBehavior<,>).MakeGenericType(queryType, typeof(QueryResult<>).MakeGenericType(resultType));
    var behavior2Type = typeof(Behavior_2<,>).MakeGenericType(queryType, resultType);
    services.AddTransient(pipelineType, behavior2Type);
}

services.AddMediatR(configuration =>
{
    configuration.RegisterServicesFromAssembly(typeof(Program).Assembly);
    configuration.AddOpenBehavior(typeof(Behavior_2<,>));
});

var serviceProvider = services.BuildServiceProvider();
var mediator = serviceProvider.GetRequiredService<IMediator>();

var result = await mediator.Send(new PingQuery());
Console.WriteLine(result.Succeeded);
