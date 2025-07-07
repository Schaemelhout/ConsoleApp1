using ConsoleApp1;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
services.AddLogging();
services.AddMediatR(configuration =>
{
    configuration.RegisterServicesFromAssembly(typeof(Program).Assembly);
    
    // Solution 1: Using marker interface (WORKS!)
    configuration.AddOpenBehavior(typeof(LoggingBehavior<,>));
    configuration.AddOpenBehavior(typeof(QueryValidationBehavior<,>));
    
    // Solution 2: QueryResultValidationBehavior conflicts with LoggingBehavior
    // configuration.AddOpenBehavior(typeof(QueryResultValidationBehavior<,>));
});

var serviceProvider = services.BuildServiceProvider();
var mediator = serviceProvider.GetRequiredService<IMediator>();

Console.WriteLine("Final test - Both behaviors working:");
var result = await mediator.Send(new PingQuery());
Console.WriteLine($"Result: {result.Succeeded}");
Console.WriteLine($"Error: {result.Error}");
