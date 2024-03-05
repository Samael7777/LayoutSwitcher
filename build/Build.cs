using System;
using System.IO;
using System.Linq;
using Microsoft.Build.Tasks;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.Docker;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.MSBuild;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Common.Tools.MSBuild.MSBuildTasks;
using MSBuild = Microsoft.Build.Tasks.MSBuild;
// ReSharper disable AllUnderscoreLocalParameterName

class Build : NukeBuild
{
    
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode
    
    [Solution] readonly Solution Solution;

    AbsolutePath OutputDirectory => RootDirectory / "Output";
    AbsolutePath PublishDirectory => OutputDirectory / "Publish";

    public static int Main ()
    {
        Logging.Level = Nuke.Common.LogLevel.Error;
        var result = Execute<Build>(x => x.Compile);
        Console.ReadKey();
        return result;
    }

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration =
#if DEBUG
    Configuration.Debug;
#else
    Configuration.Release;
#endif

    Target BuildCppProjects => _ => _
        .Executes(() =>
        {
            var hookProject = Solution.GetProject("NativeLangHook");
            var wrapperProject = Solution.GetProject("NativeLangHookWrapperCpp");

            var settings = new MSBuildSettings()
                .SetConfiguration(Configuration.Release)
                .SetOutDir(OutputDirectory)
                .EnableNoLogo();

            MSBuild(settings
                .SetProjectFile(hookProject)
                .SetTargetPlatform(MSBuildTargetPlatform.x86));

            MSBuild(settings
                .SetProjectFile(hookProject)
                .SetTargetPlatform(MSBuildTargetPlatform.x64));

            MSBuild(settings
                .SetProjectFile(wrapperProject)
                .SetTargetPlatform(MSBuildTargetPlatform.x86)
           );
        });

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            DotNetClean();
        });

    Target Restore => _ => _
        .Executes(() =>
        {
            DotNetRestore(s=> 
                s.SetProjectFile(Solution));
        });

    Target Compile => _ => _
        .DependsOn(Restore, BuildCppProjects)
        .Executes(() =>
        {
            var mainProject = Solution.GetProject("LayoutSwitcher");
            MSBuild(s => s
                .SetProjectFile(mainProject)
                .SetConfiguration(Configuration)
                .SetOutDir(OutputDirectory)
            );

            OutputDirectory.GlobFiles("*.lib", "*.exp").DeleteFiles();
           
            if (Configuration == Configuration.Release)
                OutputDirectory.GlobFiles("*.pdb").DeleteFiles();
        });

}
