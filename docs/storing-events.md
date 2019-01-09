# Storing events using SqlStreamStore

Events are stored in Microsoft SQL Server using SqlStreamStore.

## Creating the database

Create a new database in Microsoft SQL Server:

```sql
IF DB_ID(N'your-registry') IS NULL EXEC(N'CREATE DATABASE [your-registry];');
GO

IF SERVERPROPERTY('EngineEdition') <> 5 EXEC(N'ALTER DATABASE [your-registry] SET READ_COMMITTED_SNAPSHOT ON;');
GO

USE [your-registry]
GO

IF SCHEMA_ID(N'YourRegistry') IS NULL EXEC(N'CREATE SCHEMA [YourRegistry];');
GO
```

## Configuring the application

Next to the `appsettings.json` in `YourRegistry.Api`, create a new file called `appsettings.your-machine.json`, with your computername lower cased (e.g. `appsettings.boxy.json` when the computername is `Boxy`).

This file will contain the connection string to your newly created database:

```json
{
  "ConnectionStrings": {
    "Events": "Server=.;Database=your-registry;Trusted_Connection=True;"
  }
}
```

## Configuring SqlStreamStore

In `src/YourRegistry.Api/Infrastructure/Startup.cs` uncomment the following to allow SqlStreamStore to check and configure the database:

```csharp
StartupHelpers.EnsureSqlStreamStoreSchema<Startup>(streamStore, loggerFactory);
```

At this point when you run your Api and send the commands from [previous chapter](sending-commands.md):

* events are saved automatically;
* aggregates are fetched by identifier and have their state rebuild from existing events.
