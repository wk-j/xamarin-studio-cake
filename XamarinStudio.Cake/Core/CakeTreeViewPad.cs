using System;
using System.Linq;
using MonoDevelop.Ide.Gui.Components;
using MonoDevelop.Ide.Gui.Pads;
using MonoDevelop.Projects;

namespace XamarinStudio.Cake.Core {
	public class CakeTreeViewPad : TreeViewPad {
		public CakeTreeViewPad() {
		}

		public override void Initialize(NodeBuilder[] builders, TreePadOption[] options, string menuPath) {
			base.Initialize(builders, options, menuPath);

			var range = Enumerable.Range(0, 10).ToList();
			range.ForEach(x => {
				var it = new TreeViewItem($"Hello {x + 1}", Gtk.Stock.New);
				treeView.AddChild(it);
			});
		}
		protected virtual void OnOpenWorkspace(object sender, WorkspaceItemEventArgs e) {

		}

		protected virtual void OnCloseWorkspace(object sender, WorkspaceItemEventArgs e) {

		}
	}
}
