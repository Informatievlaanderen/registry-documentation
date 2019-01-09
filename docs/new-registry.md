# Starting a new registry

To get started building a new registry, you can follow these steps:

## Preparing the example registry

### Downloading the example registry

Start by [downloading the example registry](https://github.com/informatievlaanderen/example-registry/archive/master.zip) and extracting it in your new location.

### Renaming physical files to match your project

Rename the following files to your project name:

* `ExampleRegistry.sln`
* `ExampleRegistry.sln.DotSettings`

* `src\ExampleRegistry`
* `src\ExampleRegistry\ExampleRegistry.csproj`
* `src\ExampleRegistry\ExampleRegistry.csproj.DotSettings`
* `src\ExampleRegistry\ExampleRegistryException.cs`

* `src\ExampleRegistry.Api`
* `src\ExampleRegistry.Api\ExampleRegistry.Api.csproj`
* `src\ExampleRegistry.Api\ExampleRegistry.Api.csproj.DotSettings`

* `src\ExampleRegistry.Infrastructure`
* `src\ExampleRegistry.Infrastructure\ExampleRegistry.Infrastructure.csproj`

* `test\ExampleRegistry.Tests`
* `test\ExampleRegistry.Tests\ExampleRegistry.Tests.csproj`
* `test\ExampleRegistry.Tests\ExampleRegistryTest.cs`

* `docs\ExampleRegistry.Structurizr`
* `docs\ExampleRegistry.Structurizr\ExampleRegistry.Structurizr.csproj`

### Renaming code to match your project

Do a case sensitive search in all files for the following and replace it, respecting the casing:

#### Project Details

* `ExampleRegistry` to your project name.
* `example-registry` to your project name.
* `exampleregistry` to your project name.
* `exampleRegistry` to your project name.
* `Example Registry` to your project name.
* `Example Api` to your project name + `Api`.
* `Write a short summary of the business goal of this registry.` to clarify what your project is about.

#### Organisation Details

* `informatievlaanderen` to your organisation GitHub name.
* `agentschap Informatie Vlaanderen` to your organisation name.
* `Basisregisters Vlaanderen` to your organisation name.
* `Informatie Vlaanderen` to your organisation name.
* `Vlaamse overheid` to your organisation name.

* `informatie.vlaanderen@vlaanderen.be` to your organisation contact email address.
* `https://vlaanderen.be/informatie-vlaanderen` to your organisation website address.
* `"Havenlaan 88"` to your organisation street.
* `"1000"` to your organisation postal code.
* `"Brussel"` to your organisation city.

### Generate unique assembly Guids

Search for `[assembly: Guid` in every `AssemblyInfo.cs` and generate a new guid for each.

### Enabling HTTPS with a certificate

In `src\ExampleRegistry.Api\Infrastructure\Program.cs` there is a `DevelopmentCertificate` property used to configure the SSL certificate.

If you wish to use `example.pfx`, which is a self-signed certificate for `localhost`, use the password `example-registry!`
