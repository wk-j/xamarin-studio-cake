using System;
namespace XamarinStudio.Cake.Helper {
	public class SolutionHelper {
		public static String GetSolutionPath() {
			return MonoDevelop.Ide.IdeApp.ProjectOperations.CurrentSelectedSolution.BaseDirectory.FullPath;
		}
	}
}
