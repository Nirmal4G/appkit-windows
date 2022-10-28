#module nuget:?package=Cake.LongPath.Module&version=1.0.1

#addin nuget:?package=Cake.FileHelpers&version=4.0.1
#addin nuget:?package=Cake.Powershell&version=1.0.1
#addin nuget:?package=Cake.GitVersioning&version=3.4.231

#tool nuget:?package=MSTest.TestAdapter&version=2.2.6
#tool nuget:?package=vswhere&version=2.8.4

using System;
using System.Linq;
using System.Text.RegularExpressions;

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

//////////////////////////////////////////////////////////////////////
// VERSIONS
//////////////////////////////////////////////////////////////////////

string Version = null;

var inheritDocVersion = "2.5.2";

//////////////////////////////////////////////////////////////////////
// VARIABLES
//////////////////////////////////////////////////////////////////////

var baseDir = MakeAbsolute(Directory("../")).ToString();
var buildDir = baseDir + "/build";
var Solution = baseDir + "/CommunityToolkit.Windows.sln";
var toolsDir = buildDir + "/tools";

var packagesDir = baseDir + "/~packages";
var pkgBinDir = packagesDir + "/bin";

var taefBinDir = baseDir + $"/~build/UITests.Tests.TAEF/bin/{configuration}/netcoreapp3.1/win10-x64";

var styler = toolsDir + "/XamlStyler.Console/tools/xstyler.exe";
var stylerFile = baseDir + "/settings.xamlstyler";

var inheritDoc = toolsDir + "/InheritDoc/tools/InheritDoc.exe";

// Ignoring NerdBank until this is merged and we can use a new version of inheridoc:
// https://github.com/firesharkstudios/InheritDoc/pull/27
var inheritDocExclude = "Nerdbank.GitVersioning.ManagedGit.GitRepository";

//////////////////////////////////////////////////////////////////////
// METHODS
//////////////////////////////////////////////////////////////////////

void RetrieveVersion()
{
    Information("\nRetrieving version...");
    Version = GitVersioningGetVersion().NuGetPackageVersion;
    Information("\nBuild Version: " + Version);
}

public string GetMSTestAdapterPath()
{
    var nugetPaths = GetDirectories("./tools/MSTest.TestAdapter*/build/_common");
    if(nugetPaths.Count == 0)
    {
        throw new Exception(
            "Cannot locate the MSTest test adapter. " +
            "You might need to add '#tool nuget:?package=MSTest.TestAdapter&version=<latest_version>' " +
            "to the top of your build.cake file.");
    }
    return nugetPaths.Last().ToString();
}

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Description("Clean the output folder")
    .Does(() =>
{
    if(DirectoryExists(packagesDir))
    {
        Information("\nCleaning Working Directory");
        CleanDirectory(packagesDir);
    }
    else
    {
        CreateDirectory(packagesDir);
    }
});

Task("Version")
    .Description("Updates the version information in all Projects")
    .Does(() =>
{
    RetrieveVersion();
});

Task("InheritDoc")
    .Description("Updates <inheritdoc /> tags from base classes, interfaces, and similar methods")
    .IsDependentOn("Clean")
    .Does(() =>
{
    Information("\nDownloading InheritDoc...");
    var installSettings = new NuGetInstallSettings
    {
        ExcludeVersion = true,
        Version = inheritDocVersion,
        OutputDirectory = toolsDir
    };

    NuGetInstall(new[] {"InheritDoc"}, installSettings);

    var args = new ProcessArgumentBuilder()
                .AppendSwitchQuoted("-b", baseDir)
                .AppendSwitch("-o", "")
                .AppendSwitchQuoted("-x", inheritDocExclude);

    var result = StartProcess(inheritDoc, new ProcessSettings { Arguments = args });

    if (result != 0)
    {
        throw new InvalidOperationException("InheritDoc failed!");
    }

    Information("\nFinished generating documentation with InheritDoc");
});

Task("Test")
    .Description("Runs all Unit Tests")
    .Does(() =>
{
    Information("\nRunning Unit Tests");
    var vswhere = VSWhereLatest(new VSWhereLatestSettings
    {
        IncludePrerelease = false
    });

    var testSettings = new VSTestSettings
    {
        ToolPath = vswhere + "/Common7/IDE/CommonExtensions/Microsoft/TestWindow/vstest.console.exe",
        TestAdapterPath = GetMSTestAdapterPath(),
        ArgumentCustomization = arg => arg.Append("/logger:trx;LogFileName=VsTestResultsUwp.trx /framework:FrameworkUap10"),
    };

    VSTest(baseDir + $"/**/{configuration}/**/UnitTests.*.appxrecipe", testSettings);
}).DoesForEach(GetFiles(baseDir + "/**/UnitTests.*NetCore.csproj"), (file) =>
{
    Information("\nRunning NetCore Unit Tests");
    var testSettings = new DotNetCoreTestSettings
    {
        Configuration = configuration,
        NoBuild = true,
        Loggers = new[] { "trx;LogFilePrefix=VsTestResults" },
        Verbosity = DotNetCoreVerbosity.Normal,
        ArgumentCustomization = arg => arg.Append($"-s {baseDir}/.runsettings /p:Platform=AnyCPU"),
    };
    DotNetCoreTest(file.FullPath, testSettings);
}).DeferOnError();

Task("UITest")
    .Description("Runs all UI Tests")
    .DoesForEach(GetFiles(taefBinDir + "/**/UITests.Tests.TAEF.dll"), (file) =>
{
    Information("\nRunning TAEF Interaction Tests");

    var result = StartProcess(System.IO.Path.GetDirectoryName(file.FullPath) + "/TE.exe", file.FullPath + " /screenCaptureOnError /enableWttLogging /logFile:UITestResults.wtl");
    if (result != 0)
    {
        throw new InvalidOperationException("TAEF Tests failed!");
    }
}).DeferOnError();

Task("SmokeTest")
    .Description("Runs all Smoke Tests")
    .IsDependentOn("Version")
    .Does(() =>
{
    // Need to do full NuGet restore here to grab proper UWP dependencies...
    NuGetRestore(baseDir + "/SmokeTests/SmokeTest.csproj");

    var buildSettings = new MSBuildSettings()
    {
        Restore = true,
    }
    .WithProperty("NuGetPackageVersion", Version);

    MSBuild(baseDir + "/SmokeTests/SmokeTests.proj", buildSettings);
}).DeferOnError();

Task("MSTestUITest")
    .Description("Runs UITests using MSTest")
    .DoesForEach(GetFiles(baseDir + "/**/UITests.*.MSTest.csproj"), (file) =>
{
    Information("\nRunning UI Interaction Tests");

    var testSettings = new DotNetCoreTestSettings
    {
        Configuration = configuration,
        NoBuild = true,
        Loggers = new[] { "trx;LogFilePrefix=VsTestResults" },
        Verbosity = DotNetCoreVerbosity.Normal
    };
    DotNetCoreTest(file.FullPath, testSettings);
});

Task("StyleXaml")
    .Description("Ensures XAML Formatting is Clean")
    .Does(() =>
{
    Information("\nDownloading XamlStyler...");
    var installSettings = new NuGetInstallSettings
    {
        ExcludeVersion  = true,
        OutputDirectory = toolsDir
    };

    NuGetInstall(new[] {"xamlstyler.console"}, installSettings);

    Func<IFileSystemInfo, bool> exclude_objDir =
        fileSystemInfo => !fileSystemInfo.Path.Segments.Contains("obj");

    var files = GetFiles(baseDir + "/**/*.xaml", new GlobberSettings { Predicate = exclude_objDir });
    Information("\nChecking " + files.Count() + " file(s) for XAML Structure");
    foreach(var file in files)
    {
        StartProcess(styler, "-f \"" + file + "\" -c \"" + stylerFile + "\"");
    }
});

//////////////////////////////////////////////////////////////////////
// DEFAULT TASK
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("InheritDoc")
    .IsDependentOn("Test")
    .IsDependentOn("UITest");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
