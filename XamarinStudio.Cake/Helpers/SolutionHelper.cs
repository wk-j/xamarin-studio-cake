using System;
using MonoDevelop.Ide;

namespace XamarinStudio.Cake.Helper {
	public class SolutionHelper {
		public static String GetSolutionPath() {
			return IdeApp.ProjectOperations.CurrentSelectedSolution.BaseDirectory.FullPath;
		}
	}
}
