using System;
using System.Linq;
using MonoDevelop.Ide.Gui.Components;
using MonoDevelop.Ide.Gui.Pads;
using System.IO;
using XamarinStudio.Cake.Helper;
using MonoDevelop.Ide;
using MonoDevelop.Core;

namespace XamarinStudio.Cake.Core {
	public class CakeTreeViewPad : TreeViewPad {

		static bool _firstTime = true;
		static FileSystemWatcher _watcher = new FileSystemWatcher();

		public override void Initialize(NodeBuilder[] builders, TreePadOption[] options, string menuPath) {
			base.Initialize(builders, options, menuPath);
			InitializePad();
		}

		private void InitializePad() {
			IdeApp.Workspace.SolutionLoaded += (sender, e) => {
				var solution = e.Solution.BaseDirectory.FullPath;
				ReloadTasks(solution);
				RegisterEvent(solution);
				DetectChange(solution);
			};

			if (IdeApp.Workspace.GetAllSolutions().Count() > 0) {
				var solution = SolutionHelper.GetSolutionPath();
				ReloadTasks(solution);
				RegisterEvent(solution);
				DetectChange(solution);
			}
		}

		private void DetectChange(string solution) {

			_watcher.Path = solution;
			_watcher.NotifyFilter = NotifyFilters.Size;  // | System.IO.NotifyFilters.LastAccess | System.IO.NotifyFilters.LastWrite;
			_watcher.Filter = "*.cake";
			_watcher.EnableRaisingEvents = true;
			_watcher.IncludeSubdirectories = false;

			var lastWatch = DateTime.Now;
			var locker = new object();

			Action<FileSystemEventArgs> doWatch = (e) => {
				lock(locker) {
					if((DateTime.Now - lastWatch).TotalMilliseconds > 1000) {
						lastWatch = DateTime.Now;
						Gtk.Application.Invoke((sender, ev) => {
							ReloadTasks(solution);
							_firstTime = true;
						});
					}
				}
			};

			_watcher.Changed += (s, e) => {
				doWatch(e);
			};

			_watcher.Created += (s, e) => {
				doWatch(e);
			};
		}

		private void RegisterEvent(string solution) {
			this.TreeView.SelectionChanged += (s, e) => {
				var item = treeView.GetSelectedNode();
				var label = item.NodeName;

				if (label == "Initialize") {
					CakeHelper.Init(solution);
				} else {
					if (!_firstTime) {
						CakeHelper.ExecuteCmd(label, solution);
					}
					_firstTime = false;
				}
			};
		}

		private void ReloadTasks(string solution) {
			treeView.Clear();

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
