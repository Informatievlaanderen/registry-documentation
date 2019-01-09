# Sending commands to the domain

A system can receive input from various sources. Eventually all of these need to get translated into commands which are sent to the domain. The way these commands are created is up to the implementor.

Commands in their turn are sent to the domain by dispatching them on a bus. In its simplest form, a command is sent over the bus using `bus.Dispatch(commandId, command)` towards the domain.

In this example, we have an MVC controller where incoming commands are sent to as plain JSON objects for simplicity, converted into a command object and placed on the bus.

```csharp
public class CommandRequest
{
    /// <summary>Type of the command.</summary>
    [Required]
    public string Type { get; set; }

    /// <summary>The raw command.</summary>
    [Required]
    public string Command { get; set; }
}

public static class CommandRequestMapping
{
    public static dynamic Map(CommandRequest message)
    {
        var assembly = typeof(DomainAssemblyMarker).Assembly;
        var type = assembly.GetType(message.Type);

        return JsonConvert.DeserializeObject(message.Command, type);
    }
}

public class ExampleController : ExampleRegistryController
{
    /// <summary>
    /// Execute a generic command.
    /// </summary>
    /// <param name="bus"></param>
    /// <param name="commandId">Optional unique id for the request.</param>
    /// <param name="command"></param>
    /// <param name="cancellationToken"></param>
    /// <response code="202">If the request is accepted.</response>
    /// <response code="400">If the request has invalid data.</response>
    /// <response code="500">If an internal error has occured.</response>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(typeof(void), StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(BasicApiProblem), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BasicApiProblem), StatusCodes.Status500InternalServerError)]
    [SwaggerRequestExample(typeof(CommandRequest), typeof(CommandRequestExample))]
    [SwaggerResponseExample(StatusCodes.Status202Accepted, typeof(CommandResponseExamples), jsonConverter: typeof(StringEnumConverter))]
    [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestResponseExamples), jsonConverter: typeof(StringEnumConverter))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples), jsonConverter: typeof(StringEnumConverter))]
    public async Task<IActionResult> Post(
        [FromServices] ICommandHandlerResolver bus,
        [FromCommandId] Guid commandId,
        [FromBody] CommandRequest command,
        CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Normally this would be bus.Dispatch(...) but because of the example the command to dispatch is of type 'dynamic' which an extension method cannot handle.
        return Accepted(
            await CommandHandlerResolverExtensions.Dispatch(
                bus,
                commandId,
                CommandRequestMapping.Map(command),
                GetMetadata(),
                cancellationToken));
    }
}
```

At this point you can focus on the domain and [start reacting to the received commands](reacting-to-commands.md).
