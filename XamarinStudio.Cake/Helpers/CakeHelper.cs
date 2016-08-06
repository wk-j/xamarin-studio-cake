using System;
using System.Diagnostics;
using System.IO;
using System.Net;

namespace XamarinStudio.Cake {
	public class CakeHelper {
		public CakeHelper() {
		}

		public static void ExecuteCmd(string task, string workingDir) {
			var process = new Process();
			//process.StartInfo.UseShellExecute = true;
			var command = $"tell app \"Terminal\" to do script \"cd {workingDir};/bin/sh build.sh --target {task}\"";
			process.StartInfo.FileName = "osascript";
			process.StartInfo.Arguments = $"-e '{command}'";
			process.StartInfo.WorkingDirectory = workingDir;
			process.Start();

			Console.WriteLine(process.StartInfo.Arguments);
		}

		public static void Init(string solutionPath) {
			Func<string, string> full = name => Path.Combine(solutionPath, name);

			Download("http://cakebuild.net/download/bootstrapper/osx", full("build.sh"));
			Download("http://cakebuild.net/download/bootstrapper/windows", full("build.ps1"));
			Download("https://raw.githubusercontent.com/wk-j/cake-init/master/files/build.cake", full("build.cake"));
			Download("https://raw.githubusercontent.com/wk-j/cake-init/master/files/build.cmd", full("build.cmd"));
		}

		private static void Download(string source, string target) {
			if (File.Exists(target)) {
				Console.WriteLine($">> File exist | {target}");
				return;
			}
			using (var client = new WebClient()) {
				Console.WriteLine($">> Dowloading | {source}");
				var uri = new Uri(source);
				client.DownloadFileAsync(uri, target);
			}
		}
	}
}
