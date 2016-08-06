using System;
using System.Linq;
using MonoDevelop.Ide.Gui.Components;
using MonoDevelop.Ide.Gui.Pads;
using System.IO;
using XamarinStudio.Cake.Helper;
using MonoDevelop.Ide;

namespace XamarinStudio.Cake.Core {
	public class CakeTreeViewPad : TreeViewPad {

		public override void Initialize(NodeBuilder[] builders, TreePadOption[] options, string menuPath) {
			base.Initialize(builders, options, menuPath);
			InitializePad();
		}

		private void InitializePad() {
			IdeApp.Workspace.SolutionLoaded += (sender, e) => {
				var solution = e.Solution.BaseDirectory.FullPath;
				ReloadTasks(solution);
				RegisterEvent(solution);
			};
		}

		private void RegisterEvent(string solution) {
			this.TreeView.SelectionChanged += (s, e) => {
				var item = treeView.GetSelectedNode();
				var label = item.NodeName;

				if (label == "Initialize") {
					CakeHelper.Init(solution);
				} else {
					CakeHelper.ExecuteCmd(label, solution);
				}
			};
		}

		private void ReloadTasks(string solution) {
			treeView.Clear();

			var init = new TreeViewItem("Initialize", Gtk.Stock.Add);
			treeView.AddChild(init);

			var cake = Path.Combine(solution, "build.cake");
			if (!File.Exists(cake)) return;

			var task = CakeParser.ParseFile(new FileInfo(cake)).ToList();
			task.ForEach(x => {
				var name = x.Name;
				var it = new TreeViewItem($"{name}", Gtk.Stock.Execute);
				treeView.AddChild(it);
			});
		}
	}
}
