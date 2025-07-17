using System.Reflection;
using ConsoleApp1;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
services.AddLogging();

services.AddMediatR(configuration =>
{
    configuration.RegisterServicesFromAssembly(typeof(Program).Assembly);
    configuration.RegisterQueryResultPipelineBehaviors();
    configuration.AddOpenBehavior(typeof(QueryResultBehavior<,>));
});

var serviceProvider = services.BuildServiceProvider();
var mediator = serviceProvider.GetRequiredService<IMediator>();

var result = await mediator.Send(new PingQuery());
Console.WriteLine(result.Succeeded);
