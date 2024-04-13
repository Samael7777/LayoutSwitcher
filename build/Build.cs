using System;
using System.Collections.Generic;
using System.Linq;
using Nuke.Common;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.MSBuild;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Common.Tools.MSBuild.MSBuildTasks;
// ReSharper disable AllUnderscoreLocalParameterName
// ReSharper disable InconsistentNaming


class Build : NukeBuild
{
    
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode
    
    [Solution] readonly Solution Solution;

    AbsolutePath OutputDirectory => RootDirectory / "Output";
    //AbsolutePath PublishDirectory => OutputDirectory / "Publish";
	
    public static int Main ()
    {
        Logging.Level = LogLevel.Error;
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
	        var settings = new MSBuildSettings()
		        .SetConfiguration(Configuration.Release)
		        .SetOutDir(OutputDirectory)
		        .EnableNoLogo();

	        var cppProjects = GetAllCppProjects(Solution);

	        foreach (var project in cppProjects)
	        {
		        //Build for both, x86 and x64, platforms
		        MSBuild(settings
			        .SetProjectFile(project)
			        .SetTargetPlatform(MSBuildTargetPlatform.x86));
		        MSBuild(settings
			        .SetProjectFile(project)
			        .SetTargetPlatform(MSBuildTargetPlatform.x64));
	        }
        });

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            DotNetClean();
			OutputDirectory.DeleteDirectory();
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
            MSBuild(s => s
                .SetProjectFile(Solution.GetProject("LayoutSwitcher"))
                .SetConfiguration(Configuration)
                .SetOutDir(OutputDirectory)
            );

            OutputDirectory.GlobFiles("*.lib", "*.exp").DeleteFiles();
           
            if (Configuration == Configuration.Release)
                OutputDirectory.GlobFiles("*.pdb").DeleteFiles();
        });

	//	Target Pack => _ => _
	//		.DependsOn(Compile)
	//		.Executes(() => 
	//		{
	//			DotNetPack(c => c
	//			.SetProject(mainProject)
	//			.SetOutputDirectory(PublishDirectory)
	//			.SetConfiguration(Configuration)
	//			.SetNoBuild(InvokedTargets.Contains(Compile))
	//			.SetPublishSingleFile(true)
	//			.SetPublishTrimmed(true)
	//			);
	//		});

	IEnumerable<Project> GetAllCppProjects(Solution solution)
	{
		return solution.AllProjects.Where(p =>
			string.Equals(p.Path.Extension, ".vcxproj", StringComparison.OrdinalIgnoreCase));
	}
}