using ConsoleApp1;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
services.AddLogging();
services.AddMediatR(configuration =>
{
    configuration.RegisterServicesFromAssembly(typeof(Program).Assembly);
    configuration.AddOpenBehavior(typeof(Behavior_1<,>));
    configuration.AddOpenBehavior(typeof(Behavior_2<,>));
});

var serviceProvider = services.BuildServiceProvider();
var mediator = serviceProvider.GetRequiredService<IMediator>();

var result = await mediator.Send(new PingQuery());
Console.WriteLine(result.Succeeded);
