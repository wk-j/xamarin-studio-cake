using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace XamarinStudio.Cake.Helper {
	public class Task {
		public string Name { set; get; }
	}

	public class CakeParser {
		public static IEnumerable<Task> ParseFile(FileInfo file) {
			if (!file.Exists) return Enumerable.Empty<Task>();

			var lines = File.ReadAllLines(file.FullName);
			var tasks = lines.Select(x => x.Trim()).Where(x => x.StartsWith("Task(\""));

			return tasks.Select(x => {
				var name = x.Split('\"')[1];
				return new Task { Name = name };
			});
		}
	}
}
