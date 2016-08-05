#tool "nuget:?package=Fixie"
#addin "nuget:?package=Cake.Watch"

var solution = "XamarinStudio.Cake.sln";
var project = "XamarinStudio.Cake/XamarinStudio.Cake.csproj";
var testDll = "TrySelectMany.Tests/bin/Debug/TrySelectMany.Tests.dll";

var user = EnvironmentVariable("ghu");
var pass = EnvironmentVariable("ghp");

Task("build")
    .Does(() => {
            DotNetBuild(solution);
    });

Task("create-mpack")
    .Does(() => {
            DeleteFiles("XamarinStudio.Cake/bin/Debug/*.mpack");
            StartProcess("xbuild", new ProcessSettings {
                    Arguments = String.Format("{0} /t:PackageAddin", project)
            });
    });

Task("create-github-release")
    .IsDependentOn("create-mpack")
    .Does(() => {
        var package = new System.IO.DirectoryInfo("XamarinStudio.Cake/bin/Debug").GetFiles("*.mpack").FirstOrDefault();
        var version = package.Name
            .Replace("XamarinStudio.Cake.XamarinStudio.Cake_", String.Empty)
            .Replace(".mpack", String.Empty);

        var tag = string.Format("v{0}", version);
        var args = string.Format("tag -a {0} -m \"{0}\"", tag);
        var owner = "wk-j";
        var repo = "xamarin-studio-cake";

        StartProcess("git", new ProcessSettings {
            Arguments = args
        });

        StartProcess("git", new ProcessSettings {
            Arguments = string.Format("push https://{0}:{1}@github.com/wk-j/{2}.git {3}", user, pass, repo, tag)
        });

        GitReleaseManagerCreate(user, pass, owner , repo, new GitReleaseManagerCreateSettings {
            Name              = tag,
            InputFilePath = "RELEASE.md",
            Prerelease        = false,
            TargetCommitish   = "master",
        });
        GitReleaseManagerAddAssets(user, pass, owner, repo, tag, package.FullName);
        GitReleaseManagerPublish(user, pass, owner , repo, tag);
    });

var target = Argument("target", "default");
RunTarget(target);