namespace NuSave
{
  using System;
  using Core;
  using Microsoft.Extensions.CommandLineUtils;

  internal class Program
  {
    private const string Version = "3.0.0";
    private const string DefaultSource = "https://api.nuget.org/v3/index.json";

    private static void Main(string[] args)
    {
      CommandLineApplication app = new CommandLineApplication();

      CacheCommand(app);

      app.HelpOption("--help | -h");
      app.VersionOption("--version | -v", () => Version);

      app.Execute(args);
    }

    private static void CacheCommand(CommandLineApplication app)
    {
      CommandArgument slnPath = null;
      CommandArgument csprojPath = null;
      CommandArgument idAndVersion = null;
      CommandOption cacheDirectory = null;
      CommandOption source = null;
      CommandOption targetFramework = null;
      CommandOption allowPreRelease = null;
      CommandOption allowUnlisted = null;
      CommandOption silent = null;

      app.Command("cache", target =>
      {
        target.Command("sln", sln =>
        {
          slnPath = sln.Argument("path", "Solution file (.sln)");
          source = sln.Option("--source", "Package source", CommandOptionType.SingleValue);
          targetFramework = sln.Option("--targetFramework",
            "Target Framework, example: .NETStandard,Version=1.3, .NETFramework,Version=4.5",
            CommandOptionType.SingleValue);
          cacheDirectory = sln.Option("--cacheDir", "Cache directory", CommandOptionType.SingleValue);
          allowPreRelease = sln.Option("--allowPreRelease", "Allow pre-release packages", CommandOptionType.NoValue);
          allowUnlisted = sln.Option("--allowUnlisted", "Allow unlisted packages", CommandOptionType.NoValue);
          silent = sln.Option("--silent", "Don't write anything to stdout", CommandOptionType.NoValue);
        }).OnExecute(() =>
        {
          var cache = new Cache(cacheDirectory.Value());

          Log($"Using cache directory: {cache.Directory}", ConsoleColor.Cyan);

          var dependencyResolver = new DependencyResolver(new DependencyResolver.Options
          {
            Source = source.Value() ?? DefaultSource,
            TargetFramework = targetFramework.Value(),
            AllowPreRelease = allowPreRelease.HasValue(),
            AllowUnlisted = allowUnlisted.HasValue(),
            Silent = silent.HasValue()
          }, cache);
          dependencyResolver.ResolveBySln(slnPath.Value);

          var downloader = new Downloader(new Downloader.Options
          {
            Silent = silent.HasValue(),
            Source = source.Value() ?? DefaultSource
          }, dependencyResolver, cache);
          downloader.Download();

          return 0;
        });

        target.Command("csproj", csproj =>
        {
          csprojPath = csproj.Argument("path", "Project file (.csproj)");
          source = csproj.Option("--source", "Package source", CommandOptionType.SingleValue);
          targetFramework = csproj.Option("--targetFramework",
            "Target Framework, example: .NETStandard,Version=1.3, .NETFramework,Version=4.5",
            CommandOptionType.SingleValue);
          cacheDirectory = csproj.Option("--cacheDir", "Cache directory", CommandOptionType.SingleValue);
          allowPreRelease = csproj.Option("--allowPreRelease", "Allow pre-release packages", CommandOptionType.NoValue);
          allowUnlisted = csproj.Option("--allowUnlisted", "Allow unlisted packages", CommandOptionType.NoValue);
          silent = csproj.Option("--silent", "Don't write anything to stdout", CommandOptionType.NoValue);
        }).OnExecute(() =>
        {
          var cache = new Cache(cacheDirectory.Value());

          Log($"Using cache directory: {cache.Directory}", ConsoleColor.Cyan);

          var dependencyResolver = new DependencyResolver(new DependencyResolver.Options
          {
            Source = source.Value() ?? DefaultSource,
            TargetFramework = targetFramework.Value(),
            AllowPreRelease = allowPreRelease.HasValue(),
            AllowUnlisted = allowUnlisted.HasValue(),
            Silent = silent.HasValue()
          }, cache);
          dependencyResolver.ResolveByCsProj(csprojPath.Value);

          var downloader = new Downloader(new Downloader.Options
          {
            Silent = silent.HasValue(),
            Source = source.Value() ?? DefaultSource
          }, dependencyResolver, cache);
          downloader.Download();

          return 0;
        });

        target.Command("package", package =>
        {
          idAndVersion = package.Argument("id@version", "Package ID and Version, example: System.Collections@4.3.0");
          source = package.Option("--source", "Package source", CommandOptionType.SingleValue);
          targetFramework = package.Option("--targetFramework",
            "Target Framework, example: .NETStandard,Version=1.3, .NETFramework,Version=4.5",
            CommandOptionType.SingleValue);
          cacheDirectory = package.Option("--cacheDir", "Cache directory", CommandOptionType.SingleValue);
          allowPreRelease = package.Option("--allowPreRelease", "Allow pre-release packages", CommandOptionType.NoValue);
          allowUnlisted = package.Option("--allowUnlisted", "Allow unlisted packages", CommandOptionType.NoValue);
          silent = package.Option("--silent", "Don't write anything to stdout", CommandOptionType.NoValue);
        }).OnExecute(() =>
        {
          var cache = new Cache(cacheDirectory.Value());

          Log($"Using cache directory: {cache.Directory}", ConsoleColor.Cyan);

          var dependencyResolver = new DependencyResolver(new DependencyResolver.Options
          {
            Source = source.Value() ?? DefaultSource,
            TargetFramework = targetFramework.Value(),
            AllowPreRelease = allowPreRelease.HasValue(),
            AllowUnlisted = allowUnlisted.HasValue(),
            Silent = silent.HasValue()
          }, cache);
          dependencyResolver.ResolveByIdAndVersion(idAndVersion.Value.Split("@")[0],
            idAndVersion.Value.Split("@")[1]);

          var downloader = new Downloader(new Downloader.Options
          {
            Silent = silent.HasValue(),
            Source = source.Value() ?? DefaultSource
          }, dependencyResolver, cache);
          downloader.Download();

          return 0;
        });
      });
    }

    private static void Log(string message, ConsoleColor consoleColor)
    {
      Console.ForegroundColor = consoleColor;
      Console.WriteLine(message);
      Console.ResetColor();
    }
  }
}