#tool "nuget:?package=Fixie"
#addin "nuget:?package=Cake.Watch"

var solution = "XamarinStudio.Cake.sln";
var testDll = "TrySelectMany.Tests/bin/Debug/TrySelectMany.Tests.dll";

Task("build")
    .Does(() => {
            DotNetBuild(solution);
    });

var target = Argument("target", "default");
RunTarget(target);