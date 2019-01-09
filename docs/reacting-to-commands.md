# Reacting to commands

The main input of a system is commands, which are handled by command handlers. We support two different type of command handlers, regular ones and event sourced handlers.

## Regular Command Handlers

A regular command handler is simply a class which is subscribed to commands and does not need anything from the event sourcing infrastructure. This means it is not possible to easily fetch aggregates nor store events to the event store.

### Registering

To register this type of command handler, use the following syntax in the `CommandHandlerModules` registration:

```csharp
// Syntax for commandhandlers which do not use SqlStreamStore to store events
containerBuilder.RegisterType<SimpleExampleCommandHandlerModule>()
    .Named<CommandHandlerModule>(typeof(SimpleExampleCommandHandlerModule).FullName)
    .As<CommandHandlerModule>();
```

### Implementation

A handler can then simple register commands with the `For` syntax and receive the `message`:

```csharp
public sealed class SimpleExampleCommandHandlerModule : CommandHandlerModule
{
    public SimpleExampleCommandHandlerModule()
    {
        For<DoSimpleExample>()
            .Handle(message =>
            {
                Console.WriteLine($"A simple example arrived, saying {message.Command.Name.Name} in {message.Command.Name.Language}!");
            });
    }
}
```

## Event Sourced Command Handlers

Event sourced command handlers are able to easily get aggregates by id from a repository, create new aggregates as well as transparently store created events using SqlStreamStore.

### Registering

To register this type of command handler, use the following syntax in the `CommandHandlerModules` registration:

```csharp
// Syntax for EventSourcing with SqlStreamStore
containerBuilder
    .RegisterSqlStreamStoreCommandHandler<ExampleCommandHandlerModule>(
        c => handler =>
            new ExampleCommandHandlerModule(
                c.Resolve<Func<IExamples>>(),
                handler));
```

### Implementation

A handler also uses `For` to register for commands but it uses a `finalHandler` to enable auto-saving of events.

For example, this handler looks up an aggregate based on an identifier and if it does not exist, creates the aggregate and adds it to the repository:

_NOTE: At this point, this example will not work yet, since you have not set up [storing events](storing-events.md) yet._

```csharp
public sealed class ExampleCommandHandlerModule : CommandHandlerModule
{
    public ExampleCommandHandlerModule(
        Func<IExamples> getExamples,
        ReturnHandler<CommandMessage> finalHandler = null) : base(finalHandler)
    {
        For<DoExample>()
            .Handle(async (message, ct) =>
            {
                var examples = getExamples();

                var exampleId = message.Command.ExampleId;
                var possibleExample = await examples.GetOptionalAsync(exampleId, ct);

                if (!possibleExample.HasValue)
                {
                    possibleExample = new Optional<Example>(Example.Register(exampleId));
                    examples.Add(exampleId, possibleExample.Value);
                }

                var example = possibleExample.Value;

                example.DoExample(message.Command.Name);
            });
    }
}
```

The above code calls `DoExample` on an aggregate which generates events and are saved automatically when the command is handled. The aggregate looks as follows:

```csharp
public partial class Example : AggregateRootEntity
{
    public static readonly Func<Example> Factory = () => new Example();

    public static Example Register(ExampleId id)
    {
        var example = Factory();
        example.ApplyChange(new ExampleWasBorn(id));
        return example;
    }

    public void DoExample(ExampleName name)
    {
        ApplyChange(new ExampleHappened(_exampleId, name));
    }
}
```

The created events are then [stored using SqlStreamStore](storing-events.md).
