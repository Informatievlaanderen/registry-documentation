namespace Dependencies
{
    using System.Collections.Generic;
    using System;
    using System.IO;
    using Structurizr;
    using Structurizr.Api;
    using Microsoft.Extensions.Configuration;

    public static class CustomTags
    {
        public static string Library = "Library";
    }

    public enum Library
    {
        BuildPipeline,
        DeterministicGuidGenerator,
        VersionHeaderMiddleware,
        ToStringBuilder,
        TimestampJsonConverter
    }

    public class LibraryComponent
    {
        public Library Library { get; set; }
        public string Name { get; set; }

        public Container Container { get; set; }

        public string Description { get; set; }
        public string Technology { get; set; } = "C#";

        public LibraryComponent(
            Library library,
            string name)
        {
            Library = library;
            Name = name;
        }

        public LibraryComponent(
            Library library,
            string name,
            string description) : this (library, name)
        {
            Description = description;
        }
    }

    public class Program
    {
        private const string WorkspaceUrlFormat = "https://structurizr.com/workspace/{0}";

        private static long _workspaceId;
        private static string _apiKey;
        private static string _apiSecret;

        private const string Uses = "gebruikt";

        public static Dictionary<Library, LibraryComponent> Libraries = new Dictionary<Library, LibraryComponent>
        {
            {
                Library.BuildPipeline,
                new LibraryComponent(
                    Library.BuildPipeline,
                    "build-pipeline") { Technology = "F#" }
            },

            {
                Library.DeterministicGuidGenerator,
                new LibraryComponent(
                    Library.DeterministicGuidGenerator,
                    "deterministic-guid-generator",
                    "Create a deterministic GUID based on namespace Guid, a string and an optional version.")
            },

            {
                Library.VersionHeaderMiddleware,
                new LibraryComponent(
                    Library.VersionHeaderMiddleware,
                    "version-header-middleware",
                    "ASP.NET Core MVC Middleware to add a 'x-basisregister-version' header to the response containing the assembly version.")
            },

            {
                Library.ToStringBuilder,
                new LibraryComponent(
                    Library.ToStringBuilder,
                    "tostring-builder",
                    "Easily customize ToString of objects.")
            },

            {
                Library.TimestampJsonConverter,
                new LibraryComponent(
                    Library.TimestampJsonConverter,
                    "timestamp-jsonconverter",
                    "JSON.NET converter for parsing timestamps in Zulu time.")
            }
        };

        private static void Main()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .AddJsonFile($"appsettings.{Environment.MachineName.ToLowerInvariant()}.json", optional: true, reloadOnChange: false)
                .Build();

            _workspaceId = long.Parse(configuration["Structurizr:WorkspaceId"]);
            _apiKey = configuration["Structurizr:ApiKey"];
            _apiSecret = configuration["Structurizr:ApiSecret"];

            var workspace = new Workspace("Basisregisters Vlaanderen", "Basisregisters Vlaanderen stelt u in staat om alles te weten te komen rond authentieke bronnen.")
            {
                Version = DateTime.Today.ToString("yyyy-MM-dd"),
            };

            var model = workspace.Model;
            var baseRegistries = CreateSystemBaseRegistries(model);

            foreach (var library in Libraries.Values)
            {
                library.Container = CreateLibrary(
                    baseRegistries,
                    library.Library.ToString(),
                    library.Name,
                    library.Description,
                    library.Technology);
            }

            Libraries[Library.DeterministicGuidGenerator].Container
                .Uses(Libraries[Library.BuildPipeline].Container, Uses);

            Libraries[Library.VersionHeaderMiddleware].Container
                .Uses(Libraries[Library.BuildPipeline].Container, Uses);

            Libraries[Library.ToStringBuilder].Container
                .Uses(Libraries[Library.BuildPipeline].Container, Uses);

            Libraries[Library.TimestampJsonConverter].Container
                .Uses(Libraries[Library.BuildPipeline].Container, Uses);

            var views = workspace.Views;
            CreateLibraryView(views, baseRegistries);

            ConfigureStyles(views);
            UploadWorkspaceToStructurizr(workspace);
        }

        private static SoftwareSystem CreateSystemBaseRegistries(Model model)
        {
            var baseRegistries = model
                .AddSoftwareSystem(
                    "BaseRegistries",
                    "Basisregisters Vlaanderen stelt u in staat om alles te weten te komen rond authentieke bronnen.");

            baseRegistries.Id = "BaseRegistries";

            return baseRegistries;
        }

        private static Container CreateLibrary(SoftwareSystem baseRegistries,
            string id,
            string name,
            string description,
            string technology)
        {
            var library = baseRegistries
                .AddContainer(
                    name,
                    description,
                    technology);

            library.Id = id;
            library.AddTags(CustomTags.Library);

            return library;
        }

        private static void CreateContextView(ViewSet views, SoftwareSystem baseRegistries)
        {
            var contextView = views
                .CreateSystemContextView(
                    baseRegistries,
                    "Globaal overzicht",
                    "Globaal overzicht van Basisregisters Vlaanderen.");

            contextView.Add(baseRegistries);

            contextView.PaperSize = PaperSize.A6_Portrait;
        }

        private static void CreateLibraryView(ViewSet views, SoftwareSystem baseRegistries)
        {
            var contextView = views
                .CreateContainerView(
                    baseRegistries,
                    "Globaal overzicht",
                    "Globaal overzicht van Basisregisters Vlaanderen.");

            foreach (var library in Libraries.Values)
                contextView.Add(library.Container);

            contextView.PaperSize = PaperSize.A2_Landscape;
        }

        private static void ConfigureStyles(ViewSet views)
        {
            var styles = views.Configuration.Styles;
            styles.Add(new ElementStyle(Tags.Person) { Background = "#08427b", Color = "#ffffff", Shape = Shape.Person });
            styles.Add(new ElementStyle(Tags.SoftwareSystem) { Background = "#1168bd", Color = "#ffffff" });
            styles.Add(new ElementStyle(Tags.Container) { Background = "#438dd5", Color = "#ffffff" });
            styles.Add(new ElementStyle(Tags.Component) { Background = "#85BBF0", Color = "#444444" });

            styles.Add(new RelationshipStyle(Tags.Asynchronous) { Dashed = true });
            styles.Add(new RelationshipStyle(Tags.Synchronous) { Dashed = false });

            styles.Add(new RelationshipStyle(Tags.Relationship) { Routing = Routing.Orthogonal });
        }

        private static void UploadWorkspaceToStructurizr(Workspace workspace)
        {
            var structurizrClient = new StructurizrClient(_apiKey, _apiSecret) { MergeFromRemote = false };
            structurizrClient.PutWorkspace(_workspaceId, workspace);
            Console.WriteLine($"Workspace can be viewed at {string.Format(WorkspaceUrlFormat, _workspaceId)}");
        }
    }
}
